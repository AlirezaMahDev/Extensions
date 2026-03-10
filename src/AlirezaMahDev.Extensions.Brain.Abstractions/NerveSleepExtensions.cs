using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.Progress.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveSleepExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public ValueTask SleepAsync(
            IProgressLogger progressLogger,
            CancellationToken cancellationToken = default) =>
                INerve<TData, TLink>.SleepAsyncCore(progressLogger,
                    nerve.ConnectionWrap,
                    NerveHelper<TData, TLink>.SleepComparisons,
                    cancellationToken);

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

            var cellEnumerable = cellWrap.GetConnectionsWrap();

            if (cellEnumerable.Count == 0)
            {
                return;
            }

            using var cellMemory = cellEnumerable.ToCellMemory();

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
                async (_) =>
                {
                    progressLogger.IncrementLength();
                    if (cellWrap.RefValue.Child != cellMemory.Memory.Span[0].Cell.Offset)
                    {
                        await cellWrap.LockAsync(location =>
                            {
                                location.RefValue.Child = cellMemory.Memory.Span[0].Cell.Offset;
                                progressLogger.IncrementCount();
                            },
                            CancellationToken.None);
                    }
                },
                async (_) =>
                {
                    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> second = default;
                    for (var index = 0; index < cellMemory.Count - 1; index++)
                    {
                        var first = cellMemory.Memory.Span[index];
                        second = cellMemory.Memory.Span[index + 1];

                        progressLogger.IncrementLength();
                        if (first.RefValue.Next != second.Cell.Offset ||
                            first.RefValue.NextCount != cellMemory.Count - index - 1)
                        {
                            await first.LockAsync(location =>
                                {
                                    location.RefValue.Next = second.Cell.Offset;
                                    location.RefValue.NextCount = cellMemory.Count - index - 1;
                                    progressLogger.IncrementCount();
                                },
                                CancellationToken.None);
                        }
                    }

                    progressLogger.IncrementLength();
                    if (second.RefValue.Next != DataOffset.Null ||
                        second.RefValue.NextCount != 0)
                    {
                        await second.LockAsync(location =>
                            {
                                location.RefValue.Next = DataOffset.Null;
                                location.RefValue.NextCount = 0;
                                progressLogger.IncrementCount();
                            },
                            CancellationToken.None);
                    }
                }
            );
        }

        private static ValueTask SubSleep(IProgressLogger progressLogger,
            ComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> cellMemory,
            CancellationToken cancellationToken) =>
                SmartParallel.ForAsync(0,
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