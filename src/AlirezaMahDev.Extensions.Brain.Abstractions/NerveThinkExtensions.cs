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

            await INerve<TData, TLink>.ThinkCoreAsync(depth,
                linkFunc,
                data,
                0,
                Think<TData, TLink>.Create(
                    new(default(TData)),
                    new(default(TLink)),
                    nerve.ConnectionWrap),
                result,
                cancellationToken);

            return result.GetBestThinks();
        }

        private static ValueTask ThinkCoreAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> input,
            int inputIndex,
            Think<TData, TLink> currentThink,
            ThinkResult<TData, TLink> resultThink,
            CancellationToken cancellationToken = default)
        {
            var previousData = input[..inputIndex];
            var nextData = input[inputIndex..];

            if (nextData.IsEmpty)
            {
                if (currentThink.ConnectionWrap.ChildWrap.HasValue)
                {
                    resultThink.Add(currentThink);
                }

                return ValueTask.CompletedTask;
            }

            ReadOnlyMemoryValue<TLink> linkValue = linkFunc(previousData);
            var dataValue = nextData.ElementAt(0);

            var pair = new ThinkValueRef<TData, TLink>(in dataValue.Value, in linkValue.Value);
            var cellMemory = currentThink.ConnectionWrap.GetConnectionsWrapCache();

            if (cellMemory.Count == 0)
            {
                return ValueTask.CompletedTask;
            }

            using var memoryList = cellMemory.Memory.NearConnection(pair, depth);
            if (memoryList.Count == 0)
            {
                return ValueTask.CompletedTask;
            }

            return SmartParallel.ForAsync(0,
                memoryList.Count,
                cancellationToken,
                (memoryIndex, token) => INerve<TData, TLink>.ThinkCoreAsyncCore(depth,
                    linkFunc,
                    input,
                    inputIndex,
                    currentThink,
                    resultThink,
                    memoryList[memoryIndex],
                    dataValue,
                    linkValue,
                    token));
        }

        private static async ValueTask ThinkCoreAsyncCore(int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> input,
            int inputIndex,
            Think<TData, TLink> currentThink,
            ThinkResult<TData, TLink> resultThink,
            ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> readOnlyMemory,
            ReadOnlyMemoryValue<TData> dataValue,
            ReadOnlyMemoryValue<TLink> linkValue,
            CancellationToken token)
        {
            for (int readOnlyMemoryIndex = 0;
                 readOnlyMemoryIndex < readOnlyMemory.Length;
                 readOnlyMemoryIndex++)
            {
                var nextConnection = readOnlyMemory.Span[readOnlyMemoryIndex];
                var nextThink = currentThink.Append(dataValue, linkValue, nextConnection);
                if (resultThink.CanAdd(nextThink))
                {
                    await INerve<TData, TLink>.ThinkCoreAsync(depth,
                        linkFunc,
                        input,
                        inputIndex + 1,
                        nextThink,
                        resultThink,
                        token);
                }
                else
                {
                    break;
                }
            }
        }
    }
}