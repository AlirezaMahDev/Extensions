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
            scoreSortMemoryWrap.Sort(NerveHelper<TData, TLink>.ConnectionComparisons(pair));
        }

        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> Near(DataPairLink<TData, TLink> pair,
            int depth)
        {
            using var scoreSortMemoryWrap = memory.AsScoreSort();
            return scoreSortMemoryWrap.TakeBestSort(depth, NerveHelper<TData, TLink>.ConnectionComparisons(pair));
        }

        public void Sort(TLink link)
        {
            using var scoreSortMemoryWrap = memory.AsScoreSort();
            scoreSortMemoryWrap.Sort(NerveHelper<TData, TLink>.NextConnectionComparisons(link));
        }

        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> Near(TLink link, int depth)
        {
            using var scoreSortMemoryWrap = memory.AsScoreSort();
            return scoreSortMemoryWrap.TakeBestSort(depth, NerveHelper<TData, TLink>.NextConnectionComparisons(link));
        }
    }
}