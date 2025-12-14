using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerve<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    string Name { get; }
    DataLocation<DataPath> Location { get; }
    INeuron<TData, TLink> RootNeuron { get; }
    IConnection<TData, TLink> RootConnection { get; }

    void Learn(TLink link, params ReadOnlySpan<TData> data);
    // ValueTask LearnAsync(TLink link, ReadOnlySpan<TData> data, CancellationToken cancellationToken = default);

    void Sleep();
    // ValueTask SleepAsync(CancellationToken cancellationToken = default);

    Think<TData, TLink>? Think(TLink link, params TData[] data);

    ValueTask<Think<TData, TLink>?> ThinkAsync(TLink link,
        CancellationToken cancellationToken = default,
        params TData[] data);
    // ValueTask<TData?> ThinkAsync(TLink link, ReadOnlySpan<TData> data, CancellationToken cancellationToken = default);

    public void Save();
    public ValueTask SaveAsync(CancellationToken cancellationToken = default);
}