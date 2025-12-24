using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public class ThinkResult<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    private readonly Lock _lock = new();

    public Think<TData, TLink>? Think { get; set; }

    public void Add(Think<TData, TLink> think)
    {
        using var scope = _lock.EnterScope();
        if (CanAdd(think))
            Think = think;
    }

    public bool CanAdd(Think<TData, TLink> think) =>
        Think is null || Think.CompareTo(think) > 0;
}