namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapMemoryExtensions
{
    extension<TData, TLink>(Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> Near(DataPairLink<TData, TLink> pair,
            int depth)
        {
            var comparable = new DataPairLinkComparable<TData, TLink>(pair,
                NerveHelper<TData, TLink>.NearComparisons.Comparison);
            var findIndex = memory.Span.BinarySearch(comparable);
            var targetIndex = findIndex < 0 ? ~findIndex : findIndex;
            return memory[Math.Max(0, targetIndex - depth)..Math.Min(memory.Length, targetIndex + depth + 1)];
        }

        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> Near(TLink link, int depth)
        {
            var comparable = new LinkComparable<TData, TLink>(link,
                NerveHelper<TData, TLink>.NearNextComparisons.Comparison);
            var findIndex = memory.Span.BinarySearch(comparable);
            var targetIndex = findIndex < 0 ? ~findIndex : findIndex;
            return memory[Math.Max(0, targetIndex - depth)..Math.Min(memory.Length, targetIndex + depth + 1)];
        }
    }
}