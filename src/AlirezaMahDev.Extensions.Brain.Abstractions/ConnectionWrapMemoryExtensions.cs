using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapMemoryExtensions
{
    extension<TData, TLink>(Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> NearConnection(
            ThinkValue<TData, TLink> pair,
            int depth) =>
            memory.Near(pair,
                    x =>
                        new(x.NeuronWrap.RefData,
                            x.RefLink,
                            x.RefValue.RefScore,
                            x.RefValue.RefWeight),
                    NerveHelper<TData, TLink>.NearComparisons,
                    depth)
                .ToArray();

        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>
            NearConnection(PredictValue<TLink> link, int depth)
        {
            var asMemory = memory.ToArray().AsMemory();
            asMemory.Span.Sort(x =>
                    new PredictValue<TLink>(x.RefLink, x.RefValue.RefScore, x.RefValue.RefWeight),
                NerveHelper<TData, TLink>.NearNextComparisons);
            return asMemory
                .Near(link,
                    x => new(x.RefLink, x.RefValue.RefScore, x.RefValue.RefWeight),
                    NerveHelper<TData, TLink>.NearNextComparisons,
                    depth)
                .ToArray();
        }
    }
}