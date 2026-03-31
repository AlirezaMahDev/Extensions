using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Win32.SafeHandles;

namespace AlirezaMahDev.Extensions.DataManager;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
internal unsafe class DataMapFilePart(DataMapFile file, SafeFileHandle safeFileHandle, int fileOffset) : IDisposable
{
    private readonly ReaderWriterLockSlim _pointerLock = new();
    private readonly ReaderWriterLockSlim _ownerLock = new();
    private readonly DataMapFile _file = file;
    private readonly SafeFileHandle _safeFileHandle = safeFileHandle;
    private readonly int _fileOffset = fileOffset;
    private DataMapFilePartOwner? _owner;
    private ulong _lastHash;
    private nint _pointer;
    private bool _disposed;

    public long AccessCount;

    public bool Cached
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _pointer != 0;
        }
    }

    public bool Changed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return Cached && _lastHash != CurrentHash;
        }
    }

    public ulong CurrentHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return GenerateHash(Span);
        }
    }

    public Span<byte> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return new Span<byte>((byte*)_pointer, DataDefaults.PartSize);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ulong GenerateHash(Span<byte> span)
    {
        return XxHash3.HashToUInt64(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private byte* GetPointer()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _pointerLock.EnterReadLock();
        try
        {
            if (_pointer != 0)
            {
                return (byte*)_pointer;
            }
        }
        finally
        {
            _pointerLock.ExitReadLock();
        }

        _pointerLock.EnterUpgradeableReadLock();
        try
        {
            if (_pointer != 0)
            {
                return (byte*)_pointer;
            }

            _pointerLock.EnterWriteLock();
            try
            {
                _file.Map.Alloc();

                try
                {
                    _pointer = (nint)NativeMemory.AlignedAlloc(DataDefaults.PartSize, 64);
                    try
                    {
                        GC.AddMemoryPressure(DataDefaults.PartSize);
                        var cache = Span;
                        RandomAccess.Read(_safeFileHandle, cache, _fileOffset);
                        _lastHash = GenerateHash(cache);
                        return (byte*)_pointer;
                    }
                    catch
                    {
                        NativeMemory.AlignedFree((void*)_pointer);
                        GC.RemoveMemoryPressure(DataDefaults.PartSize);
                        _pointer = 0;
                        throw;
                    }
                }
                catch
                {
                    _file.Map.Free();
                    throw;
                }
            }
            finally
            {
                _pointerLock.ExitWriteLock();
            }
        }
        finally
        {
            _pointerLock.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFilePartOwner GetOwner()
    {
        _ownerLock.EnterReadLock();
        try
        {
            return _owner ??= new(this);
        }
        finally
        {
            _ownerLock.ExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref byte GetRef(int offset)
    {
        return ref Unsafe.AsRef<byte>(GetPointer() + offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_pointerLock.IsWriteLockHeld)
        {
            return;
        }

        _ownerLock.EnterWriteLock();
        try
        {
            if (_owner != null)
            {
                var gen = GC.GetGeneration(_owner);
                _owner = null;
                GC.Collect(gen, GCCollectionMode.Forced, false);
                GC.WaitForPendingFinalizers();
            }

            _pointerLock.EnterWriteLock();
            try
            {
                if (_pointer == 0)
                {
                    return;
                }

                var cache = Span;
                var currentHash = GenerateHash(cache);
                if (_lastHash != currentHash)
                {
                    RandomAccess.Write(_safeFileHandle, cache, _fileOffset);
                    _lastHash = currentHash;
                }

                if (Volatile.Read(ref AccessCount) == 0)
                {
                    NativeMemory.AlignedFree((void*)_pointer);
                    _file.Map.Free();
                    GC.RemoveMemoryPressure(DataDefaults.PartSize);
                    _pointer = 0;
                }
            }
            finally
            {
                _pointerLock.ExitWriteLock();
            }
        }
        finally
        {
            _ownerLock.ExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Flush();
        _pointerLock.Dispose();
        _disposed = true;
    }
}