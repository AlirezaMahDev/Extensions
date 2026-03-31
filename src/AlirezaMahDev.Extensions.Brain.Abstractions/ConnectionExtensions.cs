namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionExtensions
{
    extension<TData, TLink>(in Connection connection)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellWrap<ConnectionValue<TLink>, TData, TLink> NewWrap(INerve<TData, TLink> nerve)
        {
            DataLocation<ConnectionValue<TLink>>.Read(nerve.Access, connection.Offset, out var location);
            return new(nerve, location);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public CellWrap<ConnectionValue<TLink>, TData, TLink> NewWrap<TValue>(
            ref CellWrap<TValue, TData, TLink> wrap)
            where TValue : unmanaged, ICellValue<TValue>
        {
            DataLocation<ConnectionValue<TLink>>.Read(wrap.Nerve.Access, connection.Offset, out var location);
            return new(wrap.Nerve, location);
        }
    }
}