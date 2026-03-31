namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapMemoryExtensions
{
    extension<TData, TLink>(Memory<CellWrap<ConnectionValue<TLink>, TData, TLink>> memory)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryList<Memory<CellWrap<ConnectionValue<TLink>, TData, TLink>>> NearConnection(
            ref ThinkValueRef<TData, TLink> pair,
            int depth)
        {
            return memory.Near(ref pair,
                static (scoped ref readonly x) =>
                    new(in x.NeuronWrap.Location.UnsafeRefReadOnlyValue.Data,
                        in x.Location.UnsafeRefReadOnlyValue.Link,
                        x.Location.UnsafeRefReadOnlyValue.Score,
                        x.Location.UnsafeRefReadOnlyValue.Weight),
                NerveHelper<TData, TLink>.SleepComparisons,
                depth);
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryList<Memory<CellWrap<ConnectionValue<TLink>, TData, TLink>>> NearConnection(
            ref PredictValueRef<TLink> link,
            int depth)
        {
            using MemoryList<CellWrap<ConnectionValue<TLink>, TData, TLink>> memoryList = memory;
            memoryList.Memory.Span.Sort((scoped ref readonly x) =>
                    new PredictValueRef<TLink>(in x.Location.UnsafeRefReadOnlyValue.Link, x.Location.UnsafeRefReadOnlyValue.Score, x.Location.UnsafeRefReadOnlyValue.Weight),
                NerveHelper<TData, TLink>.NearNextComparisons);
            return memoryList.Memory
                .Near(ref link,
                    (scoped ref readonly x) =>
                        new(in x.Location.UnsafeRefReadOnlyValue.Link, x.Location.UnsafeRefReadOnlyValue.Score, x.Location.UnsafeRefReadOnlyValue.Weight),
                    NerveHelper<TData, TLink>.NearNextComparisons,
                    depth);
        }
    }
}