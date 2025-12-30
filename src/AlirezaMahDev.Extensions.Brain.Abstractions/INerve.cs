using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
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
    IRootNeuron<TData, TLink> RootNeuron { get; }
    IConnection<TData, TLink> RootConnection { get; }

    void Learn(ReadOnlyMemoryValue<TLink> link, ReadOnlyMemory<TData> data);

    ValueTask LearnAsync(ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> data,
        CancellationToken cancellationToken = default);

    void Sleep();
    ValueTask SleepAsync(CancellationToken cancellationToken = default);

    Think<TData, TLink>? Think(ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> data);

    ValueTask<Think<TData, TLink>?> ThinkAsync(ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> data,
        CancellationToken cancellationToken = default);

    public void Flush();
    public ValueTask FlushAsync(CancellationToken cancellationToken = default);
}