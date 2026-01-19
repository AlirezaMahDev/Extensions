namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapStaticExtensions
{
    extension<TData, TLink>(CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
    {
        public static Comparison<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> ComparerOnData(TData data) =>
            (a, b) =>
            {
                var aData = NerveHelper.Difference(a.NeuronWrap.RefData, data);
                var bData = NerveHelper.Difference(b.NeuronWrap.RefData, data);
                return aData.CompareTo(bData);
            };

        public static Comparison<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> ComparerOnLink(TLink link) =>
            (a, b) =>
            {
                var aLink = NerveHelper.Difference(a.RefLink, link);
                var bLink = NerveHelper.Difference(b.RefLink, link);
                return aLink.CompareTo(bLink);
            };
    }
}
