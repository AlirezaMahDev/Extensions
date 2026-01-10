using System.Buffers;
using System.IO.MemoryMappedFiles;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

unsafe class DataMapFilePart : MemoryManager<byte>
{
    private readonly MemoryMappedViewAccessor _accessor;
    private readonly byte* _pointer;
    private int _pinCount;

    public DataMapFilePart(MemoryMappedViewAccessor accessor)
    {
        _accessor = accessor;

        _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref _pointer);
        _pointer += _accessor.PointerOffset;
    }

    public override Span<byte> GetSpan()
    {
        return new(_pointer, DataDefaults.PartSize);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        Interlocked.Increment(ref _pinCount);
        return new(_pointer + elementIndex);
    }

    public override void Unpin()
    {
        Interlocked.Decrement(ref _pinCount);
    }

    public void Flush()
    {
        _accessor.Flush();
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return ValueTask.FromCanceled(cancellationToken);

        _accessor.Flush();
        return ValueTask.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        if (_pinCount > 0)
            throw new InvalidOperationException("Cannot dispose while memory is pinned.");

        if (disposing)
        {
            _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            _accessor.Dispose();
        }
    }
}