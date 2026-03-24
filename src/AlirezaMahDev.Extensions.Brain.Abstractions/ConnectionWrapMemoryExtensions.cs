namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapMemoryExtensions
{
    extension<TData, TLink>(Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryList<ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
            NearConnection(
                ThinkValueRef<TData, TLink> pair,
                int depth)
        {
            return memory.Near(pair,
                x =>
                    new(in x.NeuronWrap.RefData,
                        in x.RefLink,
                        in x.RefValue.RefScore,
                        in x.RefValue.RefWeight),
                NerveHelper<TData, TLink>.SleepComparisons,
                depth);
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryList<ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
            NearConnection(PredictValueRef<TLink> link, int depth)
        {
            using MemoryList<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memoryList = memory;
            memoryList.Memory.Span.Sort(x =>
                    new PredictValueRef<TLink>(in x.RefLink, in x.RefValue.RefScore, in x.RefValue.RefWeight),
                NerveHelper<TData, TLink>.NearNextComparisons);
            return memoryList.Memory
                .Near(link,
                    x => new(in x.RefLink, in x.RefValue.RefScore, in x.RefValue.RefWeight),
                    NerveHelper<TData, TLink>.NearNextComparisons,
                    depth);
        }
    }
}