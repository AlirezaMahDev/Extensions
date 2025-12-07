using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerve<TData>
    where TData : unmanaged
{
    string Name { get; }
    DataLocation<DataPath> Location { get; }
    IConnection<TData> Root { get; }
    void Learn(params ReadOnlySpan<TData> data);
    void Sleep();
    TData? Think(params ReadOnlySpan<TData> data);
}