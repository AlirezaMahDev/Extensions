using System.Buffers;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal class DataBlockAccessor : MemoryManager<byte>, IDataBlockAccessor
{
    private readonly Lock _lockInitialize = new();
    private readonly Lock _lockSave = new();
    private readonly DataBlock _block;
    private MemoryMappedViewAccessor _accessor = null!;
    private unsafe byte* _pointer;

    public DataBlockAccessor(DataBlockAccessorArgs args)
    {
        _block = args.DataBlock;
        Address = args.Address;
        _lockInitialize.Enter();
        Initialize();
    }

    public DataAddress Address { get; }

    public void Save()
    {
        using var scopeInitialize = _lockInitialize.EnterScope();
        using var scopeSave = _lockSave.EnterScope();
        _accessor.Flush();
    }

    public unsafe void Initialize()
    {
        _accessor = _block.CreateViewAccessor(Address);
        _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref _pointer);
        _pointer += _accessor.PointerOffset;
        _lockInitialize.Exit();
    }

    public unsafe void UnInitialize()
    {
        _lockInitialize.Enter();
        _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        _pointer = null;
        _accessor.Flush();
        _accessor.Dispose();
    }

    public override unsafe Span<byte> GetSpan()
    {
        return new(_pointer, Address.Length);
    }

    public override unsafe MemoryHandle Pin(int elementIndex = 0)
    {
        _lockInitialize.Enter();
        return new(_pointer + elementIndex);
    }

    public override void Unpin()
    {
        _lockInitialize.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        UnInitialize();
    }

    public override string ToString()
    {
        return Memory.Length.ToString();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}