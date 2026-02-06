using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class ConnectionWrapMemoryExtensions
{
    extension<TData, TLink>(Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> memory)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> NearConnection(
            DataPairLink<TData, TLink> pair,
            int depth) =>
            memory.Near(pair,
                x => new(x.NeuronWrap.RefData, x.RefLink),
                NerveHelper<TData, TLink>.NearComparisons.Wrap(), depth)
                .ToArray();

        public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> NearConnection(TLink link, int depth) =>
            memory.Near(link,
                x => x.RefLink,
                NerveHelper<TData, TLink>.NearNextComparisons.Wrap(), depth)
                .ToArray();
    }
}