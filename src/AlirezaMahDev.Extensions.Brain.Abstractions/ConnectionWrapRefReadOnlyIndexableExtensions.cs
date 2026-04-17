namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapRefReadOnlyIndexableExtensions
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
                .AsRefReadOnlyBlock<ConnectionWrapRefReadOnlyIndexable<TData, TLink>, DataOffset>()
                .Near<RefReadOnlyBlock<ConnectionWrapRefReadOnlyIndexable<TData, TLink>, DataOffset>, DataOffset, ThinkValue<TData, TLink>>(ref pair, (scoped ref readonly x) =>
                    {
                        DataLocation<ConnectionValue<TLink>>.Read(readOnlyIndexable.Nerve.Access, x, out var location);
                        var cellWrap = new CellWrap<ConnectionValue<TLink>, TData, TLink>(
                            readOnlyIndexable.Nerve, location);
                        return cellWrap.Location.UnsafeAccessRefReadOnly(
                        (scoped ref readonly connectionValue) =>
                            new ThinkValue<TData, TLink>(
                                default,
                                connectionValue.Link,
                                connectionValue.Score,
                                connectionValue.Weight
                            )) with
                        {
                            Data = cellWrap.NeuronWrap.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Data)
                        };
                    },
                    NerveHelper<TData, TLink>.SleepComparisons,
                    depth);
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public NativeRefList<Range> NearConnection(
            ref PredictValue<TLink> link,
            int depth)
        {
            if (readOnlyIndexable.Length == 0)
            {
                return NativeRefList<Range>.Create();
            }

            using var memoryList = readOnlyIndexable.GetCellWraps(0, readOnlyIndexable.Length);
            memoryList.Memory.Span.Sort((scoped ref readonly x) =>
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) =>
                            new PredictValue<TLink>(value.Link, value.Score, value.Weight)),
                    NerveHelper<TData, TLink>.NearNextComparisons);
            return memoryList.Memory.Span.AsReadOnlySpanWrap()
                .Near<ReadOnlySpanWrap<CellWrap<ConnectionValue<TLink>, TData, TLink>>,
                    CellWrap<ConnectionValue<TLink>, TData, TLink>, PredictValue<TLink>>(ref link,
                    (scoped ref readonly x) =>
                        x.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) =>
                            new PredictValue<TLink>(value.Link, value.Score, value.Weight)),
                    NerveHelper<TData, TLink>.NearNextComparisons,
                    depth);
        }
    }
}