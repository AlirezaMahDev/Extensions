using System.Buffers;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveThinkExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public void CleanThink()
        {
            Parallel.ForEach(nerve.MemoryCache.Where(x => x.Value.IsValueCreated),
                pair => pair.Value.Value.Dispose());
            nerve.MemoryCache.Clear();
        }

        public async ValueTask<Memory<Think<TData, TLink>>> ThinkAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            using var result = new ThinkResult<TData, TLink>(depth);

            Think<TData, TLink> think = new(default, default, nerve.ConnectionWrap, null);
            await INerve<TData, TLink>.ThinkCoreAsync(depth,
                    linkFunc,
                    data,
                    0,
                    think,
                    result,
                    cancellationToken)
                .AsTaskRun()
                .ConfigureAwait(false);

            return result.GetBestThinks();
        }

        private static async Task<bool> ThinkCoreAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> input,
            int inputIndex,
            Think<TData, TLink> currentThink,
            ThinkResult<TData, TLink> resultThink,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);

            var previousData = input[..inputIndex];
            var nextData = input[inputIndex..];

            if (nextData.IsEmpty)
            {
                if (currentThink.ConnectionWrap.ChildWrap.HasValue)
                {
                    if (resultThink.Add(currentThink))
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
            var dataValue = nextData.ElementAt(0);

            var pair = new ThinkValueRef<TData, TLink>(in dataValue.Value, in linkValue.Value);
            var cellMemory = currentThink.ConnectionWrap.GetConnectionsWrapCache();
            using var memory = cellMemory.Memory.NearConnection(pair, depth);

            if (memory.Count == 0)
            {
                return false;
            }

            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);
            await Task.Yield();

            var returnResult = false;
            foreach (var readOnlyMemory in memory)
            {
                if (cancellationToken.IsCancellationRequested)
                    await Task.FromCanceled(cancellationToken);

                using var tasks = MemoryPool<Task<bool>>.Shared.Rent(readOnlyMemory.Length);
                var tasksSpan = tasks.Memory[..readOnlyMemory.Length].Span;
                for (var i = 0; i < readOnlyMemory.Length; i++)
                {
                    var nextConnection = readOnlyMemory.Span[i];
                    var nextThink = currentThink.Append(dataValue, linkValue, nextConnection);
                    if (resultThink.CanAdd(nextThink))
                    {
                        tasksSpan[i] = INerve<TData, TLink>.ThinkCoreAsync(depth,
                            linkFunc,
                            input,
                            inputIndex + 1,
                            nextThink,
                            resultThink,
                            cancellationToken);
                    }
                }

                var all = await Task.WhenAll(tasksSpan);
                returnResult = returnResult || all.Any(x => x);
            }

            return returnResult;
        }
    }
}