namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionsWrapMemoryListExtensions
{
    extension<TData, TLink>(MemoryList<CellWrap<ConnectionValue<TLink>, TData, TLink>> memory)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryList<Memory<CellWrap<ConnectionValue<TLink>, TData, TLink>>> NearConnection(
            ref ThinkValueRef<TData, TLink> pair,
            int depth)
        {
            return memory.Memory.Near(ref pair,
                static (scoped ref readonly x) =>
                    new(
                        x.NeuronWrap.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Data),
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Link),
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Score),
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Weight)
                    ),
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
                    new PredictValueRef<TLink>(
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Link),
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Score),
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Weight)
                    ),
                NerveHelper<TData, TLink>.NearNextComparisons);
            return memoryList.Memory
                .Near(ref link,
                    (scoped ref readonly x) =>
                        new(
                            x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Link),
                            x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Score),
                            x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Weight)
                        ),
                    NerveHelper<TData, TLink>.NearNextComparisons,
                    depth);
        }
    }
}