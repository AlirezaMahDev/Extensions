using System.Buffers;

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
        public async Task SleepAsync(
            IProgressLogger progressLogger,
            CancellationToken cancellationToken = default)
        {
            await INerve<TData, TLink>.SleepAsyncCore(progressLogger,
                nerve.ConnectionWrap,
                NerveHelper<TData, TLink>.SleepComparisons,
                cancellationToken);
        }

        private static async Task SleepAsyncCore(
            IProgressLogger progressLogger,
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> cellWrap,
            ComparisonChain<ThinkValueRef<TData, TLink>> comparisonChain,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            using var cellMemory = cellWrap.GetConnectionsWrap().ToCellMemory();
            switch (cellWrap.Length)
            {
                case 0:
                    return;
                case 1:
                    break;
                default:
                    cellMemory.Memory.Span.Sort(wrap => new(
                            in wrap.NeuronWrap.RefData,
                            in wrap.RefLink,
                            in wrap.RefValue.RefScore,
                            in wrap.RefValue.RefWeight),
                        comparisonChain.Comparison);
                    if (cellWrap.RefValue.Child != cellMemory.Memory.Span[0].Cell.Offset)
                    {
                        await cellWrap.LockAsync(location =>
                            {
                                location.RefValue.Child = cellMemory.Memory.Span[0].Cell.Offset;
                                progressLogger.IncrementCount();
                            },
                            CancellationToken.None);
                    }

                    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> second = default;
                    for (int index = 0; index < cellMemory.Count - 1; index++)
                    {
                        var first = cellMemory.Memory.Span[index];
                        second = cellMemory.Memory.Span[index + 1];

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

                    break;
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            using var memoryOwner = MemoryPool<Task>.Shared.Rent(cellMemory.Count);
            var tasks = memoryOwner.Memory[..cellMemory.Count].Span;
            tasks.Fill(Task.CompletedTask);
            for (int index = 0; index < cellMemory.Count; index++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                tasks[index] = INerve<TData, TLink>.SleepAsyncCore(progressLogger,
                    cellMemory.Memory.Span[index],
                    comparisonChain,
                    cancellationToken);
            }

            await Task.WhenAll(tasks);
        }
    }
}