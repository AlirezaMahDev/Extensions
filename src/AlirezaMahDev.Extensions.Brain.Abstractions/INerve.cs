using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerve<TData, TLink>
    where TData : unmanaged
    where TLink : unmanaged
{
    string Name { get; }
    DataLocation<DataPath> Location { get; }
    IConnection<TData, TLink> Root { get; }

    void Learn(TLink link, ReadOnlySpan<TData> data);
    // ValueTask LearnAsync(TLink link, ReadOnlySpan<TData> data, CancellationToken cancellationToken = default);

    void Sleep();
    // ValueTask SleepAsync(CancellationToken cancellationToken = default);

    ThinkResult<TData, TLink> Think(TLink link, ReadOnlySpan<TData> readOnlySpan);
    // ValueTask<TData?> ThinkAsync(TLink link, ReadOnlySpan<TData> data, CancellationToken cancellationToken = default);

    public void Save();
    public ValueTask SaveAsync(CancellationToken cancellationToken = default);
}