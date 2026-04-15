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
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using ThinkResult<TData, TLink> result = new(depth);

            await INerve<TData, TLink>.ThinkCoreAsync(depth,
                linkFunc,
                data,
                0,
                Think<TData, TLink>.Create(nerve.RootConnectionWrap),
                result,
                cancellationToken);

            return result.GetBestThinks();
        }

        private static async ValueTask ThinkCoreAsync(
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

                return;
            }

            ReadOnlyMemoryValue<TLink> linkValue = linkFunc(previousData);
            var dataValue = nextData.ElementAt(0);

            ThinkValue<TData, TLink> pair = new(dataValue.Value, linkValue.Value);

            var readOnlyIndexable = currentThink.ConnectionWrap.GetConnectionWrapRefReadOnlyIndexable();
            if (readOnlyIndexable.IsEmpty)
            {
                return;
            }

            using var list = readOnlyIndexable.NearConnection(ref pair, depth);
            if (list.IsEmpty)
            {
                return;
            }

            await SmartParallel.ForAsync(0,
                list.Length,
                cancellationToken,
                (memoryIndex, token) => INerve<TData, TLink>.ThinkCoreAsyncCore(depth,
                    linkFunc,
                    input,
                    inputIndex,
                    currentThink,
                    resultThink,
                    readOnlyIndexable.Memory[list[memoryIndex]],
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
            ReadOnlyMemory<CellWrap<ConnectionValue<TLink>, TData, TLink>> readOnlyMemory,
            ReadOnlyMemoryValue<TData> dataValue,
            ReadOnlyMemoryValue<TLink> linkValue,
            CancellationToken token)
        {
            for (var readOnlyMemoryIndex = 0;
                 readOnlyMemoryIndex < readOnlyMemory.Length;
                 readOnlyMemoryIndex++)
            {
                var nextConnection =
                    readOnlyMemory.Span[readOnlyMemoryIndex];
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