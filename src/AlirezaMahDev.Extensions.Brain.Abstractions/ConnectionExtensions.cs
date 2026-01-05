using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionExtensions
{
    extension<TData, TLink>(Connection<TData, TLink> connection)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public ConnectionWrap<TData, TLink> Wrap(INerve<TData, TLink> nerve) =>
            new(nerve, connection);

        public ConnectionWrap<TData, TLink> Wrap<TWrap>(TWrap wrap)
            where TWrap : ICellWrap<TData, TLink> =>
            new(wrap.Nerve, connection);
    }
}