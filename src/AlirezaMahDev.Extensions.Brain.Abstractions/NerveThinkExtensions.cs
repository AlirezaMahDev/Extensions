using System.Buffers;

using AlirezaMahDev.Extensions.Abstractions;

using CommunityToolkit.HighPerformance.Buffers;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveThinkExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public async ValueTask<Memory<Think<TData, TLink>>> ThinkAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            var result = new ThinkResult<TData, TLink>();

            Think<TData, TLink> think = new(nerve.ConnectionWrap, null);
            await INerve<TData, TLink>.ThinkCoreAsync(depth,
                    linkFunc,
                    data,
                    0,
                    think,
                    result,
                    cancellationToken)
                .AsTaskRun()
                .ConfigureAwait(false);

            return result.GetBestThinks(depth);
        }

        private static async Task<bool> ThinkCoreAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> data,
            int index,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);

            var previousData = data[..index];
            var nextData = data[index..];

            if (nextData.IsEmpty)
            {
                if (think.ConnectionWrap.ChildWrap.HasValue)
                {
                    if (result.Add(think, depth))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);
            await Task.Yield();

            ReadOnlyMemoryValue<TLink> linkValue = linkFunc(previousData);
            ReadOnlyMemoryValue<TData> dataValue = nextData.ElementAt(0);

            var pair = new ThinkValue<TData, TLink>(dataValue, linkValue);
            var cellMemory = think.ConnectionWrap.GetConnectionsWrapCache();
            Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory =
                cellMemory.Memory.NearConnection(pair, depth);

            if (memory.IsEmpty)
            {
                return false;
            }

            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);
            await Task.Yield();

            using var tasks = MemoryPool<Task<bool>>.Shared.Rent(memory.Length);
            var tasksSpan = tasks.Memory[..memory.Length].Span;
            var taskCount = 0;
            for (var i = 0; i < memory.Length; i++)
            {
                var nextConnection = memory.Span[i];
                var nextThink = think.Append(dataValue, linkValue, nextConnection);
                if (result.CanAdd(nextThink, depth))
                {
                    tasksSpan[i] = INerve<TData, TLink>.ThinkCoreAsync(depth,
                        linkFunc,
                        data,
                        index + 1,
                        nextThink,
                        result,
                        cancellationToken);
                    taskCount++;
                }
                else
                {
                    break;
                }
            }

            var whenAll = await Task.WhenAll(tasksSpan[..taskCount]);
            return whenAll.Any(x => x);
        }
    }
}