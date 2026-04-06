using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

internal sealed unsafe class DataMapFilePartCache : SafeHandle, IDataMapFilePartCache
{
    private ulong _accessCount;

    public DataMapFilePart Part
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public ulong AccessCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Volatile.Read(ref _accessCount);
    }

    public bool Accessed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => AccessCount > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataMapFilePartCache(DataMapFilePart part) : base(IntPtr.Zero, true)
    {
        Part = part;

        Part.File.Map.Alloc();
        try
        {
            void* pointer = NativeMemory.AlignedAlloc(DataDefaults.PartSize, 64);
            NativeMemory.Clear(pointer, DataDefaults.PartSize);
            SetHandle((IntPtr)pointer);
            GC.AddMemoryPressure(DataDefaults.PartSize);
            try
            {
                AccessSpan(0,
                    (scoped ref readonly span) =>
                    {
                        RandomAccess.Read(part.File.SafeFileHandle, span, part.FilePartOffset);
                        LastHash = GenerateHash(span);
                    });
            }
            catch
            {
                GC.RemoveMemoryPressure(DataDefaults.PartSize);
                SetHandleAsInvalid();
                NativeMemory.AlignedFree(pointer);
                throw;
            }
        }
        catch
        {
            part.File.Map.Free();
            throw;
        }
    }

    public override bool IsInvalid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => handle == IntPtr.Zero;
    }

    public ulong LastHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private set;
    }

    public ulong CurrentHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => AccessSpan(0, (scoped ref readonly span) => GenerateHash(span));
    }

    private ulong CurrentHashCore
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => AccessSpanCore(0, (scoped ref readonly span) => GenerateHash(span));
    }

    public bool HasChange
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return CurrentHash != LastHash;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected override bool ReleaseHandle()
    {
        var currentHash = CurrentHashCore;
        if (LastHash != currentHash)
        {
            AccessSpanCore(0,
                (scoped ref readonly span) =>
                {
                    RandomAccess.Write(Part.File.SafeFileHandle, span, Part.FilePartOffset);
                    LastHash = currentHash;
                });
        }

        NativeMemory.AlignedFree((void*)DangerousGetHandle());
        Part.File.Map.Free();
        GC.RemoveMemoryPressure(DataDefaults.PartSize);
        SetHandle(IntPtr.Zero);
        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void AccessRefByte(int offset, ScopedRefAction<byte> action)
    {
        bool success = false;
        DangerousAddRef(ref success);
        Interlocked.Increment(ref _accessCount);
        try
        {
            action(ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset));
        }
        finally
        {
            DangerousRelease();
            Interlocked.Decrement(ref _accessCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void AccessRefByteCore(int offset, ScopedRefAction<byte> action)
    {
        Interlocked.Increment(ref _accessCount);
        try
        {
            action(ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset));
        }
        finally
        {
            Interlocked.Decrement(ref _accessCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public TResult AccessRefByte<TResult>(int offset, ScopedRefFunc<byte, TResult> func)
    {
        bool success = false;
        DangerousAddRef(ref success);
        Interlocked.Increment(ref _accessCount);
        try
        {
            return func(ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset));
        }
        finally
        {
            DangerousRelease();
            Interlocked.Decrement(ref _accessCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public TResult AccessRefByteCore<TResult>(int offset, ScopedRefFunc<byte, TResult> func)
    {
        Interlocked.Increment(ref _accessCount);
        try
        {
            return func(ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset));
        }
        finally
        {
            Interlocked.Decrement(ref _accessCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void AccessSpan(int offset, ScopedRefReadOnlyAction<Span<byte>> action)
    {
        AccessRefByte(offset,
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (
                scoped ref refByte) =>
            {
                var span = MemoryMarshal.CreateSpan(ref refByte, DataDefaults.PartSize);
                action(ref span);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void AccessSpanCore(int offset, ScopedRefReadOnlyAction<Span<byte>> action)
    {
        AccessRefByteCore(offset,
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (
                scoped ref refByte) =>
            {
                var span = MemoryMarshal.CreateSpan(ref refByte, DataDefaults.PartSize);
                action(ref span);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public TResult AccessSpan<TResult>(int offset, ScopedRefReadOnlyFunc<Span<byte>, TResult> func)
    {
        return AccessRefByte(offset,
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (scoped ref refByte) =>
            {
                var span = MemoryMarshal.CreateSpan(ref refByte, DataDefaults.PartSize);
                return func(ref span);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private TResult AccessSpanCore<TResult>(int offset, ScopedRefReadOnlyFunc<Span<byte>, TResult> func)
    {
        return AccessRefByteCore(offset,
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (scoped ref refByte) =>
            {
                var span = MemoryMarshal.CreateSpan(ref refByte, DataDefaults.PartSize);
                return func(ref span);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void AccessRefReadOnlyByte(int offset, ScopedRefReadOnlyAction<byte> action)
    {
        bool success = false;
        DangerousAddRef(ref success);
        Interlocked.Increment(ref _accessCount);
        try
        {
            action(ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset));
        }
        finally
        {
            DangerousRelease();
            Interlocked.Decrement(ref _accessCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public TResult AccessRefReadOnlyByte<TResult>(int offset, ScopedRefReadOnlyFunc<byte, TResult> func)
    {
        bool success = false;
        DangerousAddRef(ref success);
        Interlocked.Increment(ref _accessCount);
        try
        {
            return func(ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset));
        }
        finally
        {
            DangerousRelease();
            Interlocked.Decrement(ref _accessCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void AccessReadOnlySpan(int offset, ScopedRefReadOnlyAction<ReadOnlySpan<byte>> action)
    {
        AccessRefReadOnlyByte(offset,
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (
                scoped ref readonly refByte) =>
            {
                var span = MemoryMarshal.CreateReadOnlySpan(in refByte, DataDefaults.PartSize);
                action(ref span);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public TResult AccessReadOnlySpan<TResult>(int offset, ScopedRefReadOnlyFunc<ReadOnlySpan<byte>, TResult> func)
    {
        return AccessRefReadOnlyByte(offset,
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (scoped ref readonly refByte) =>
            {
                var span = MemoryMarshal.CreateReadOnlySpan(in refByte, DataDefaults.PartSize);
                return func(ref span);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref byte EnterAccessRefByte(int offset)
    {
        bool success = false;
        DangerousAddRef(ref success);
        Interlocked.Increment(ref _accessCount);
        return ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Span<byte> EnterAccessSpan(int offset)
    {
        return MemoryMarshal.CreateSpan(ref EnterAccessRefByte(offset), DataDefaults.PartSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref readonly byte EnterAccessRefReadOnlyByte(int offset)
    {
        bool success = false;
        DangerousAddRef(ref success);
        Interlocked.Increment(ref _accessCount);
        return ref Unsafe.AsRef<byte>((byte*)DangerousGetHandle() + offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlySpan<byte> EnterAccessReadOnlySpan(int offset)
    {
        return MemoryMarshal.CreateReadOnlySpan(in EnterAccessRefReadOnlyByte(offset), DataDefaults.PartSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void ExitAccess()
    {
        DangerousRelease();
        Interlocked.Decrement(ref _accessCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ulong GenerateHash(Span<byte> span) => XxHash3.HashToUInt64(span);
}