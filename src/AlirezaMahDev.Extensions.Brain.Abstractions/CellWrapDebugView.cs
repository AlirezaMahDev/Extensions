namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public class CellWrapDebugView<TCell, TValue, TData, TLink>(CellWrap<TCell, TValue, TData, TLink> wrap)
    where TCell : ICell
    where TValue : unmanaged, ICellValue<TValue>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public CellWrap<TCell, TValue, TData, TLink> Wrap { get; } = wrap;

    public IEnumerable<object> SubWraps =>
        Wrap switch
        {
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection => connection.GetConnectionsWrap()
                .Cast<object>(),
            CellWrap<Neuron, NeuronValue<TData>, TData, TLink> neuron => neuron.GetConnectionsWrap().Cast<object>(),
            _ => []
        };
}