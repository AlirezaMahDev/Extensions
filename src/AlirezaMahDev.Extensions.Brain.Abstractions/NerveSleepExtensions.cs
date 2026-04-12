namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveSleepExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public void Sleep(
            IProgressLogger progressLogger,
            CancellationToken cancellationToken = default)
        {
            INerve<TData, TLink>.SleepCore(progressLogger,
                nerve.RootConnectionWrap,
                NerveHelper<TData, TLink>.SleepComparisons,
                cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private static void SleepCore(
            IProgressLogger progressLogger,
            CellWrap<ConnectionValue<TLink>, TData, TLink> cellWrap,
            ScopedComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            using var cellMemory = cellWrap.GetConnectionsWrapMemory();

            if (cellMemory is null)
            {
                return;
            }

            if (cellMemory.Count == 1)
            {
                INerve<TData, TLink>.SubSleep(progressLogger, comparisonChain, cellMemory, cancellationToken);
                return;
            }

            SmartParallel.Invoke(cancellationToken,
                [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            (_) => INerve<TData, TLink>.SelfSleep(progressLogger,
                    cellWrap,
                    comparisonChain,
                    cellMemory,
                    cancellationToken),
                [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            (_) => INerve<TData, TLink>.SubSleep(progressLogger, comparisonChain, cellMemory, cancellationToken));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private static void SelfSleep(IProgressLogger progressLogger,
            CellWrap<ConnectionValue<TLink>, TData, TLink> cellWrap,
            ScopedComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            MemoryList<CellWrap<ConnectionValue<TLink>, TData, TLink>> cellMemory,
            CancellationToken cancellationToken = default)
        {
            cellMemory.Memory.Span.Sort((scoped ref readonly wrap) => new(
                    wrap.NeuronWrap.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Data),
                    wrap.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Link),
                    wrap.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Score),
                    wrap.Location.UnsafeAccessRefReadOnly((scoped ref readonly value) => value.Weight)
                ),
                comparisonChain.Comparison);
            SmartParallel.Invoke(cancellationToken,
                [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            (token) =>
                {
                    progressLogger.IncrementLength();
                    if (cellWrap.Location.ReadLock((scoped ref readonly x) =>
                                x.Child.Offset != cellMemory.Memory.Span[0].Location.Offset,
                            token))
                    {
                        cellWrap.Location.WriteLock((scoped ref value) =>
                            {
                                value.Child = new(cellMemory.Memory.Span[0].Location.Offset);
                                progressLogger.IncrementCount();
                            },
                            token);
                    }
                },
                [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            (token) =>
                {
                    CellWrap<ConnectionValue<TLink>, TData, TLink> second = default;
                    for (var index = 0; index < cellMemory.Count - 1; index++)
                    {
                        var first = cellMemory.Memory.Span[index];
                        second = cellMemory.Memory.Span[index + 1];

                        progressLogger.IncrementLength();
                        if (first.Location.ReadLock((scoped ref readonly x) =>
                                    x.Next.Offset != second.Location.Offset,
                                token))
                        {
                            first.Location.WriteLock((scoped ref location) =>
                                {
                                    location.Next = new(second.Location.Offset);
                                    progressLogger.IncrementCount();
                                },
                                token);
                        }
                    }

                    progressLogger.IncrementLength();
                    if (second.Location.ReadLock((scoped ref readonly x) =>
                                x.Next.Offset != DataOffset.Null,
                            token))
                    {
                        second.Location.WriteLock((scoped ref location) =>
                            {
                                location.Next = new(DataOffset.Null);
                                progressLogger.IncrementCount();
                            },
                            token);
                    }
                }
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private static void SubSleep(IProgressLogger progressLogger,
            ScopedComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            MemoryList<CellWrap<ConnectionValue<TLink>, TData, TLink>> cellMemory,
            CancellationToken cancellationToken)
        {
            SmartParallel.For(0,
                cellMemory.Count,
                cancellationToken,
                [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            (index, token) =>
                    INerve<TData, TLink>.SleepCore(progressLogger,
                        cellMemory.Memory.Span[index],
                        comparisonChain,
                        token)
            );
        }
    }
}