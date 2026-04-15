namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapRefReadOnlyBlockExtensions
{
    extension<TData, TLink>(ConnectionWrapRefReadOnlyIndexable<TData, TLink> readOnlyIndexable)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public NativeRefList<Range> NearConnection(
            ref ThinkValue<TData, TLink> pair,
            int depth)
        {
            return readOnlyIndexable
                .AsRefReadOnlyBlock<ConnectionWrapRefReadOnlyIndexable<TData, TLink>,
                    CellWrap<ConnectionValue<TLink>, TData, TLink>>()
                .Near<RefReadOnlyBlock<ConnectionWrapRefReadOnlyIndexable<TData, TLink>,
                        CellWrap<ConnectionValue<TLink>, TData, TLink>>, CellWrap<ConnectionValue<TLink>, TData, TLink>,
                    ThinkValue<TData, TLink>>(ref pair,
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
        public NativeRefList<Range> NearConnection(
            ref PredictValue<TLink> link,
            int depth)
        {
            Memory<CellWrap<ConnectionValue<TLink>, TData, TLink>> memory = readOnlyIndexable.Memory.ToArray();
            memory.Span.Sort((scoped ref readonly x) =>
                    new PredictValue<TLink>(
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Link),
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Score),
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Weight)
                    ),
                NerveHelper<TData, TLink>.NearNextComparisons);
            return memory.Span.AsReadOnlySpanWrap()
                .Near<ReadOnlySpanWrap<CellWrap<ConnectionValue<TLink>, TData, TLink>>,
                    CellWrap<ConnectionValue<TLink>, TData, TLink>, PredictValue<TLink>>(ref link,
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