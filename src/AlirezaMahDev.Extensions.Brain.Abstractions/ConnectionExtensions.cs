namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionExtensions
{
    extension<TData, TLink>(Connection connection)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> Wrap(INerve<TData, TLink> nerve)
        {
            return new(nerve, connection);
        }

        public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> Wrap<TCell, TValue>(
            in CellWrap<TCell, TValue, TData, TLink> wrap)
            where TCell : ICell
            where TValue : unmanaged, ICellValue<TValue>
        {
            return new(wrap.Nerve, connection);
        }
    }
}