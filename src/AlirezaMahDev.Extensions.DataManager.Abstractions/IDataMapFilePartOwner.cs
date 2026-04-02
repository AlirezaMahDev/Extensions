namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataMapFilePartOwner
{
    ref byte GetRef(scoped in DataOffset offset);
    Span<byte> GetSpan(scoped in DataOffset offset);
}
