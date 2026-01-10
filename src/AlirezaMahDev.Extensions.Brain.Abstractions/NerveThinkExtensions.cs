using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveThinkExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public Think<TData, TLink>? Think(ReadOnlyMemoryValue<TLink> link, ReadOnlyMemory<TData> data)
        {
            var result = new ThinkResult<TData, TLink>();

            INerve<TData, TLink>.ThinkCore(link,
                data,
                new(default, link.Value, nerve.ConnectionWrap, null),
                result
            );

            return result.Think;
        }

        public async ValueTask<Think<TData, TLink>?> ThinkAsync(ReadOnlyMemoryValue<TLink> link,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            var result = new ThinkResult<TData, TLink>();

            await INerve<TData, TLink>.ThinkCoreAsync(link,
                data,
                new(default, link.Value, nerve.ConnectionWrap, null),
                result,
                cancellationToken);

            return result.Think;
        }

        private static void ThinkCore(
            ReadOnlyMemoryValue<TLink> link,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result)
        {
            if (INerve<TData, TLink>.CheckResultAvailable(link, input, think, result))
            {
                return;
            }

            Thread.Yield();

            if (INerve<TData, TLink>.FindNextConnections(link,
                    input,
                    think,
                    out TData data,
                    out Memory<ConnectionWrap<TData, TLink>> memory))
            {
                return;
            }

            Thread.Yield();

            var nextInput = input[1..];
            Parallel.ForEach(MemoryMarshal.ToEnumerable<ConnectionWrap<TData, TLink>>(memory)
                    .Select(innerConnection => think.Append(data, link.Value, innerConnection))
                    .TakeWhile(result.CanAdd),
                innerThink =>
                    INerve<TData, TLink>.ThinkCore(link, nextInput, innerThink, result));
        }

        private static async Task ThinkCoreAsync(
            ReadOnlyMemoryValue<TLink> link,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);

            if (INerve<TData, TLink>.CheckResultAvailable(link, input, think, result))
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                await Task.FromCanceled(cancellationToken);
            await Task.Yield();

            if (INerve<TData, TLink>.FindNextConnections(link,
                    input,
                    think,
                    out TData data,
                    out Memory<ConnectionWrap<TData, TLink>> memory))
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
                if (result.CanAdd(innerThink))
                {
                    tasks.Memory.Span[i] =
                        INerve<TData, TLink>.ThinkCoreAsync(link, nextInput, innerThink, result, cancellationToken);
                    taskCount++;
                }
                else
                {
                    break;
                }
            }

            await Task.WhenAll(tasks.Memory.Span[..taskCount]);
        }

        private static bool FindNextConnections(ReadOnlyMemoryValue<TLink> link,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            out TData data,
            out Memory<ConnectionWrap<TData, TLink>> memory)
        {
            data = input.Span[0];

            var pain = new DataPairLink<TData, TLink>(data, link.Value);
            var connection = think.Connection;
            memory = connection.GetConnectionsWrap().ToArray().AsMemory();
            memory.Span.Sort((a, b) =>
                a.CompareTo(pain).CompareTo(b.CompareTo(pain)));

            if (memory.IsEmpty)
                return true;

            memory = memory[..Math.Min(1, memory.Length)];
            return false;
        }

        private static bool CheckResultAvailable(ReadOnlyMemoryValue<TLink> link,
            ReadOnlyMemory<TData> input,
            Think<TData, TLink> think,
            ThinkResult<TData, TLink> result)
        {
            if (input.IsEmpty)
            {
                var next = think.Connection.GetConnectionsWrap()
                    .Min(Comparer<ConnectionWrap<TData, TLink>>.Create((a, b) =>
                        a.CompareTo(link.Value).CompareTo(b.CompareTo(link.Value))))
                    .NullWhenDefault();
                if (!next.HasValue)
                    return true;

                var nextWrap = next.Value;
                result.Add(think.Append(nextWrap.NeuronWrap.RefData,
                    nextWrap.RefLink,
                    nextWrap));
                return true;
            }

            return false;
        }
    }
}