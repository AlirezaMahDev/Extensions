namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveSleepExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ValueTask SleepAsync(
            IProgressLogger progressLogger,
            CancellationToken cancellationToken = default)
        {
            return INerve<TData, TLink>.SleepAsyncCore(progressLogger,
                nerve.RootConnectionWrap,
                NerveHelper<TData, TLink>.SleepComparisons,
                cancellationToken);
        }

        private static async ValueTask SleepAsyncCore(
            IProgressLogger progressLogger,
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> cellWrap,
            ComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var cellEnumerable =
                cellWrap.GetConnectionsWrap();

            if (cellEnumerable.Count == 0)
            {
                return;
            }

            using var cellMemory =
                cellEnumerable.ToCellMemory();

            if (cellMemory.Count == 1)
            {
                await INerve<TData, TLink>.SubSleep(progressLogger, comparisonChain, cellMemory, cancellationToken);
            }
            else
            {
                await SmartParallel.InvokeAsync(cancellationToken,
                    (token) =>
                        INerve<TData, TLink>.SelfSleep(progressLogger, cellWrap, comparisonChain, cellMemory, token),
                    (token) =>
                        INerve<TData, TLink>.SubSleep(progressLogger, comparisonChain, cellMemory, token)
                );
            }
        }

        private static ValueTask SelfSleep(IProgressLogger progressLogger,
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> cellWrap,
            ComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> cellMemory,
            CancellationToken cancellationToken = default)
        {
            cellMemory.Memory.Span.Sort(wrap => new(
                    in wrap.NeuronWrap.RefData,
                    in wrap.RefLink,
                    in wrap.RefValue.RefScore,
                    in wrap.RefValue.RefWeight),
                comparisonChain.Comparison);
            return SmartParallel.InvokeAsync(cancellationToken,
                (_) =>
                {
                    progressLogger.IncrementLength();
                    if (cellWrap.RefValue.Child != cellMemory.Memory.Span[0].RefCell.Offset)
                    {
                        cellWrap.Lock((ref readonly location) =>
                            {
                                location.RefValue.Child = cellMemory.Memory.Span[0].RefCell.Offset;
                                progressLogger.IncrementCount();
                            });
                    }

                    return ValueTask.CompletedTask;
                },
                (_) =>
                {
                    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> second = default;
                    for (var index = 0; index < cellMemory.Count - 1; index++)
                    {
                        var
                            first = cellMemory.Memory.Span[index];
                        second = cellMemory.Memory.Span[index + 1];

                        progressLogger.IncrementLength();
                        if (first.RefValue.Next != second.RefCell.Offset ||
                            first.RefValue.NextCount != cellMemory.Count - index - 1)
                        {
                            first.Lock((ref readonly location) =>
                                {
                                    location.RefValue.Next = second.RefCell.Offset;
                                    location.RefValue.NextCount = cellMemory.Count - index - 1;
                                    progressLogger.IncrementCount();
                                });
                        }
                    }

                    progressLogger.IncrementLength();
                    if (second.RefValue.Next != DataOffset.Null ||
                        second.RefValue.NextCount != 0)
                    {
                        second.Lock((ref readonly location) =>
                            {
                                location.RefValue.Next = DataOffset.Null;
                                location.RefValue.NextCount = 0;
                                progressLogger.IncrementCount();
                            });
                    }

                    return ValueTask.CompletedTask;
                }
            );
        }

        private static ValueTask SubSleep(IProgressLogger progressLogger,
            ComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> cellMemory,
            CancellationToken cancellationToken)
        {
            return SmartParallel.ForAsync(0,
                cellMemory.Count,
                cancellationToken,
                (index, token) =>
                    INerve<TData, TLink>.SleepAsyncCore(progressLogger,
                        cellMemory.Memory.Span[index],
                        comparisonChain,
                        token)
            );
        }
    }
}