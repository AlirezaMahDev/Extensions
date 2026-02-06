using System.Buffers;
using System.Diagnostics;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveThinkExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        // public Memory<Think<TData, TLink>> Think(int depth,
        //     Func<ReadOnlyMemory<TData>, TLink> linkFunc,
        //     ReadOnlyMemory<TData> data)
        // {
        //     var result = new ThinkResult<TData, TLink>();
        //
        //     INerve<TData, TLink>.ThinkCore(
        //         depth,
        //         linkFunc,
        //         data,
        //         new(default, linkFunc(data), nerve.ConnectionWrap, null),
        //         result
        //     );
        //
        //     return result.Thinks;
        // }
        //
        // private static void ThinkCore(
        //     int depth,
        //     Func<ReadOnlyMemory<TData>, TLink> linkFunc,
        //     ReadOnlyMemory<TData> input,
        //     Think<TData, TLink> think,
        //     ThinkResult<TData, TLink> result)
        // {
        //     if (INerve<TData, TLink>.CheckResultAvailable(depth, input, think, result))
        //     {
        //         return;
        //     }
        //
        //     Thread.Yield();
        //
        //     var link = linkFunc(input).AsReadonlyMemoryValue();
        //     if (INerve<TData, TLink>.FindNextConnections(
        //             depth,
        //             link,
        //             input,
        //             think,
        //             out TData data,
        //             out Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory))
        //     {
        //         return;
        //     }
        //
        //     Thread.Yield();
        //
        //     var nextInput = input[1..];
        //     Parallel.ForEach(MemoryMarshal
        //             .ToEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>(memory)
        //             .Select(innerConnection => think.Append(data, link.Value, innerConnection))
        //             .TakeWhile(result.CanAdd),
        //         innerThink =>
        //             INerve<TData, TLink>.ThinkCore(depth, linkFunc, nextInput, innerThink, result));
        // }

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
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);

            if (input.IsEmpty)
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

            var link = linkFunc(input).AsReadonlyMemoryValue();
            var data = input.Span[0];

            var pair = new DataPairLink<TData, TLink>(data, link.Value);
            var cellMemory = think.ConnectionWrap.GetConnectionsWrapCache();
            Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory = cellMemory.Memory.NearConnection(pair, depth);

            if (memory.IsEmpty)
            {
                return false;
            }

            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);
            await Task.Yield();

            var nextInput = input[1..];
            using var tasks = MemoryPool<Task<bool>>.Shared.Rent(memory.Length);
            var tasksSpan = tasks.Memory[..memory.Length].Span;
            var taskCount = 0;
            for (var i = 0; i < memory.Length; i++)
            {
                var innerConnection = memory.Span[i];
                var innerThink = think.Append(data, link.Value, innerConnection);
                if (result.CanAdd(innerThink, depth))
                {
                    tasksSpan[i] = INerve<TData, TLink>.ThinkCoreAsync(depth,
                        linkFunc,
                        nextInput,
                        innerThink,
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
            if (whenAll.Any(x => x))
            {
                return true;
            }

            return false;
        }
    }
}