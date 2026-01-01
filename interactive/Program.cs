#!/usr/local/share/dotnet/dotnet run
#:sdk Microsoft.NET.Sdk

using System.Buffers;

var c = new CustomMemoryManager();

var a = c.Memory;
var b = c.GetSpan();

class CustomMemoryManager : MemoryManager<int>
{
    private readonly Memory<int> _test = new([1, 2, 3]);

    public override Span<int> GetSpan()
    {
        return _test.Span;
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        return _test.Pin();
    }

    public override void Unpin()
    {
        var t = 0;
    }

    protected override void Dispose(bool disposing)
    {
        var d = 0;
    }
}
