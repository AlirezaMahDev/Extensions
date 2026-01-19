using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapMemoryExtensions
{
    extension<TData, TLink>(Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
    {
        public void Sort(DataPairLink<TData, TLink> pair)
        {
            using var scoreSortMemoryWrap = memory.AsScoreSort();
            scoreSortMemoryWrap.Sort(NerveHelper<TData, TLink>.GetConnectionComparisons(pair).Span);
        }

        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> Near(DataPairLink<TData, TLink> pair, int depth)
        {
            using var scoreSortMemoryWrap = memory.AsScoreSort();
            return scoreSortMemoryWrap.TakeBestSort(depth, NerveHelper<TData, TLink>.GetConnectionComparisons(pair).Span);
        }

        public void Sort(TLink link)
        {
            using var scoreSortMemoryWrap = memory.AsScoreSort();
            scoreSortMemoryWrap.Sort(CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>.ComparerOnLink(link));
        }

        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> Near(TLink link, int depth)
        {
            using var scoreSortMemoryWrap = memory.AsScoreSort();
            return scoreSortMemoryWrap.TakeBestSort(depth, CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>.ComparerOnLink(link));
        }
    }
}