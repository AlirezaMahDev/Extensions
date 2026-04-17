using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;


public sealed record class ThinkContextOptions<TData, TLink>(
    Channel<ThinkContext<TData, TLink>> Channel,
    int Depth,
    Func<ReadOnlyMemory<TData>, TLink> LinkFunc,
    ReadOnlyMemory<TData> Input,
    ThinkResult<TData, TLink> ResultThink
)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public int ActiveContext;
    public readonly Channel<ThinkContext<TData, TLink>> Channel = Channel;
    public readonly int Depth = Depth;
    public readonly Func<ReadOnlyMemory<TData>, TLink> LinkFunc = LinkFunc;
    public readonly ReadOnlyMemory<TData> Input = Input;
    public readonly ThinkResult<TData, TLink> ResultThink = ResultThink;
}

public struct ThinkContextComparer<TData, TLink> : IComparer<ThinkContext<TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly int Compare(ThinkContext<TData, TLink> x, ThinkContext<TData, TLink> y)
    {
        return y.InputIndex.CompareTo(x.InputIndex);
    }
}

[StructLayout(LayoutKind.Auto)]
public readonly record struct ThinkContext<TData, TLink> : IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly ThinkContextOptions<TData, TLink> Options;
    public readonly int InputIndex;
    public readonly Think<TData, TLink> CurrentThink;

    public ThinkContext(
        ThinkContextOptions<TData, TLink> options,
        int inputIndex,
        Think<TData, TLink> currentThink)
    {
        Options = options;
        InputIndex = inputIndex;
        CurrentThink = currentThink;
        Interlocked.Increment(ref options.ActiveContext);
    }

    public void Dispose()
    {
        Interlocked.Decrement(ref Options.ActiveContext);
    }
}

public static class NerveThinkExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public async ValueTask<Memory<Think<TData, TLink>>> ThinkAsync(
            int depth,
            Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ScopedRefReadOnlyFunc<Think<TData, TLink>, bool> check,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using ThinkResult<TData, TLink> result = new(depth, check);
            var channel = Channel.CreateUnboundedPrioritized(
                    new UnboundedPrioritizedChannelOptions<ThinkContext<TData, TLink>>
                    {
                        Comparer = new ThinkContextComparer<TData, TLink>()
                    }
                );

            var options = new ThinkContextOptions<TData, TLink>(
                channel,
                depth,
                linkFunc,
                data,
                result
            );

            await channel.Writer.WriteAsync(
                new ThinkContext<TData, TLink>(
                options,
                0,
                Think<TData, TLink>.Create(nerve.RootConnectionWrap)
                ), cancellationToken);

            await SmartParallel.ForEachAsync(channel, cancellationToken, INerve<TData, TLink>.ThinkCoreAsync);

            return result.GetBestThinks();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static async ValueTask ThinkCoreAsync(
            ThinkContext<TData, TLink> context,
            CancellationToken cancellationToken = default)
        {
            using (context)
            {
                await ThinkCoreAsyncCore(context, cancellationToken);
            }

            if (Volatile.Read(ref context.Options.ActiveContext) == 0)
            {
                context.Options.Channel.Writer.Complete();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static async ValueTask ThinkCoreAsyncCore(ThinkContext<TData, TLink> context, CancellationToken cancellationToken)
        {
            var previousData = context.Options.Input[..context.InputIndex];
            var nextData = context.Options.Input[context.InputIndex..];

            if (nextData.IsEmpty)
            {
                if (context.CurrentThink.ConnectionWrap.ChildWrap.HasValue)
                {
                    context.Options.ResultThink.TryAdd(context.CurrentThink);
                    return;
                }

                return;
            }

            ReadOnlyMemoryValue<TLink> linkValue = context.Options.LinkFunc(previousData);
            var dataValue = nextData.ElementAt(0);

            ThinkValue<TData, TLink> pair = new(dataValue.Value, linkValue.Value);

            var readOnlyIndexable = context.CurrentThink.ConnectionWrap
                .GetConnectionWrapRefReadOnlyIndexable();
            if (readOnlyIndexable.IsEmpty)
            {
                return;
            }

            using var list = readOnlyIndexable.NearConnection(ref pair, context.Options.Depth);
            if (list.IsEmpty)
            {
                return;
            }

            for (var memoryIndex = 0; memoryIndex < list.Length; memoryIndex++)
            {
                var range = list[memoryIndex];
                for (var readOnlyMemoryIndex = range.Start.Value;
                 readOnlyMemoryIndex < range.End.Value;
                 readOnlyMemoryIndex++)
                {
                    var nextThink = context.CurrentThink.Append(dataValue,
                        linkValue,
                        readOnlyIndexable.GetCellWrap(readOnlyMemoryIndex));
                    if (context.Options.ResultThink.CanAdd(nextThink))
                    {
                        await context.Options.Channel.Writer.WriteAsync(new(
                            context.Options,
                            context.InputIndex + 1,
                            nextThink
                        ), cancellationToken);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}