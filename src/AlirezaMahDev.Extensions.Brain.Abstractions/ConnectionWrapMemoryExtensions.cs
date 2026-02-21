using AlirezaMahDev.Extensions.Abstractions;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapMemoryExtensions
{
    extension<TData, TLink>(Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MustDisposeResource]
        public IReadonlyMemoryList<ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
            NearConnection(
                ThinkValueRef<TData, TLink> pair,
                int depth) =>
            memory.Near(pair,
                x =>
                    new(in x.NeuronWrap.RefData,
                        in x.RefLink,
                        in x.RefValue.RefScore,
                        in x.RefValue.RefWeight),
                NerveHelper<TData, TLink>.SleepComparisons,
                depth);

        [MustDisposeResource]
        public IReadonlyMemoryList<ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
            NearConnection(PredictValueRef<TLink> link, int depth)
        {
            using MemoryList<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memoryList = memory;
            var cloneMemory = memoryList.Memory;
            cloneMemory.Span.Sort(x =>
                    new PredictValueRef<TLink>(in x.RefLink, in x.RefValue.RefScore, in x.RefValue.RefWeight),
                NerveHelper<TData, TLink>.NearNextComparisons);
            return cloneMemory
                .Near(link,
                    x => new(in x.RefLink, in x.RefValue.RefScore, in x.RefValue.RefWeight),
                    NerveHelper<TData, TLink>.NearNextComparisons,
                    depth);
        }
    }
}