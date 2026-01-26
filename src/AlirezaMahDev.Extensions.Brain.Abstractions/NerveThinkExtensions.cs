using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveThinkExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Memory<Think<TData, TLink>> Think(int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> data)
        {
            var result = new ThinkResult<TData, TLink>();

            INerve<TData, TLink>.ThinkCore(
                depth,
                linkFunc,
                data,
                new(default, linkFunc(data), nerve.ConnectionWrap, null),
                result
            );

            return result.Thinks;
        }

        private static void ThinkCore(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result)
        {
            if (INerve<TData, TLink>.CheckResultAvailable(depth, input, think, result))
            {
                return;
            }

            Thread.Yield();

            var link = linkFunc(input).AsReadonlyMemoryValue();
            if (INerve<TData, TLink>.FindNextConnections(
                    depth,
                    link,
                    input,
                    think,
                    out TData data,
                    out Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory))
            {
                return;
            }

            Thread.Yield();

            var nextInput = input[1..];
            Parallel.ForEach(MemoryMarshal
                    .ToEnumerable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>(memory)
                    .Select(innerConnection => think.Append(data, link.Value, innerConnection))
                    .TakeWhile(result.CanAdd),
                innerThink =>
                    INerve<TData, TLink>.ThinkCore(depth, linkFunc, nextInput, innerThink, result));
        }

        public async ValueTask<Memory<Think<TData, TLink>>> ThinkAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            var result = new ThinkResult<TData, TLink>();

            await INerve<TData, TLink>.ThinkCoreAsync(depth,
                    linkFunc,
                    data,
                    new(default, linkFunc(data), nerve.ConnectionWrap, null),
                    result,
                    cancellationToken)
                .AsTaskRun()
                .ConfigureAwait(false);

            return result.GetBestThinks(depth);
        }


        private  async Task ThinkInitializeAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> input,
            ThinkResult<TData, TLink> result,
            CancellationToken cancellationToken = default)
        {
            nerve.NeuronWrap.GetConnectionsWrap();
        }

        private static async Task ThinkCoreAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);

            if (INerve<TData, TLink>.CheckResultAvailable(depth, input, think, result))
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);
            await Task.Yield();

            var link = linkFunc(input).AsReadonlyMemoryValue();
            if (INerve<TData, TLink>.FindNextConnections(
                    depth,
                    link,
                    input,
                    think,
                    out TData data,
                    out Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory))
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);
            await Task.Yield();

            var nextInput = input[1..];
            using var tasks = MemoryPool<Task>.Shared.Rent(memory.Length);
            var taskCount = 0;
            for (var i = 0; i < memory.Length; i++)
            {
                var innerConnection = memory.Span[i];
                var innerThink = think.Append(data, link.Value, innerConnection);
                if (result.CanAdd(innerThink, depth))
                {
                    tasks.Memory.Span[i] = INerve<TData, TLink>.ThinkCoreAsync(depth,
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

            await Task.WhenAll(tasks.Memory.Span[..taskCount]);
        }

        private static bool FindNextConnections(int depth,
            ReadOnlyMemoryValue<TLink> link,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            out TData data,
            out Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory)
        {
            data = input.Span[0];

            var pair = new DataPairLink<TData, TLink>(data, link.Value);
            memory = think.Connection
                .GetConnectionsWrap()
                .ToArray()
                .AsMemory()
                .Near(pair, depth);

            if (memory.IsEmpty)
                return true;

            Debug.WriteLine($"found new neuron path {memory.Length}");
            return false;
        }

        private static bool CheckResultAvailable(
            int depth,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result)
        {
            if (input.IsEmpty)
            {
                if (!think.Connection.GetConnectionsWrap().Any())
                    return true;

                var addResult = result.Add(think, depth);
                Debug.WriteLineIf(addResult, $"think found {think}");
                return true;
            }

            return false;
        }
    }
}