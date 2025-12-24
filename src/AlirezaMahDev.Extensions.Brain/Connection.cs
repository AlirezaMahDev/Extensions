using System.Collections;
using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;


namespace AlirezaMahDev.Extensions.Brain;

class Connection<TData, TLink> : IConnection<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    protected internal readonly Nerve<TData, TLink> _nerve;

    protected internal Neuron<TData, TLink>? _neuron;
    private Connection<TData, TLink>? _previous;
    private Connection<TData, TLink>? _next;
    private Connection<TData, TLink>? _subConnection;
    private Connection<TData, TLink>? _nextSubConnection;


    public virtual DataLocation<ConnectionValue<TLink>> Location { get; }

    public Connection(ConnectionArgs<TData, TLink> args)
    {
        _nerve = args.Nerve;
        Location = args.Location;
        _neuron = _nerve.NeuronFactory.GetOrCreate(RefValue.Neuron);
    }

    public long Offset => Location.Offset;
    public ref readonly ConnectionValue<TLink> RefValue => ref Location.RefValue;

    public ref readonly TLink RefLink => ref RefValue.Link;

    public void Update(UpdateDataLocationAction<ConnectionValue<TLink>> action)
    {
        Location.Update(action);
        CheckCache();
    }

    public async ValueTask UpdateAsync(UpdateDataLocationAsyncAction<ConnectionValue<TLink>> action,
        CancellationToken cancellationToken = default)
    {
        await Location.UpdateAsync(action, cancellationToken);
        CheckCache();
    }

    private void CheckCache()
    {
        if ((_previous?.Offset ?? -1L) != RefValue.Previous)
            _previous = null;
        if ((_next?.Offset ?? -1L) != RefValue.Next)
            _next = null;
        if ((_subConnection?.Offset ?? -1L) != RefValue.SubConnection)
            _subConnection = null;
        if ((_nextSubConnection?.Offset ?? -1L) != RefValue.NextSubConnection)
            _nextSubConnection = null;
    }

    public virtual INeuron<TData, TLink> GetNeuron() =>
        _neuron ??= _nerve.NeuronFactory.GetOrCreate(RefValue.Neuron);

    public virtual async ValueTask<INeuron<TData, TLink>>
        GetNeuronAsync(CancellationToken cancellationToken = default) =>
        _neuron ??= await _nerve.NeuronFactory.GetOrCreateAsync(RefValue.Neuron, cancellationToken);

    public IConnection<TData, TLink>? GetPrevious() =>
        _previous ??= RefValue.Previous != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.Previous)
            : null;

    public async ValueTask<IConnection<TData, TLink>?>
        GetPreviousAsync(CancellationToken cancellationToken = default) =>
        _previous ??= RefValue.Previous != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.Previous, cancellationToken)
            : null;

    public IConnection<TData, TLink>? GetNext() =>
        _next ??= RefValue.Next != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.Next)
            : null;

    public async ValueTask<IConnection<TData, TLink>?> GetNextAsync(CancellationToken cancellationToken = default) =>
        _next ??= RefValue.Next != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.Next, cancellationToken)
            : null;

    public IConnection<TData, TLink>? GetSubConnection() =>
        _subConnection ??= RefValue.SubConnection != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.SubConnection)
            : null;

    public async ValueTask<IConnection<TData, TLink>?> GetSubConnectionAsync(CancellationToken cancellationToken =
        default) =>
        _subConnection ??= RefValue.SubConnection != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.SubConnection, cancellationToken)
            : null;

    public IConnection<TData, TLink>? GetNextSubConnection() =>
        _nextSubConnection ??= RefValue.NextSubConnection != -1
            ? _nerve.ConnectionFactory.GetOrCreate(RefValue.NextSubConnection)
            : null;

    public async ValueTask<IConnection<TData, TLink>?> GetNextSubConnectionAsync(CancellationToken cancellationToken =
        default) =>
        _nextSubConnection ??= RefValue.NextSubConnection != -1
            ? await _nerve.ConnectionFactory.GetOrCreateAsync(RefValue.NextSubConnection, cancellationToken)
            : null;


    public int CompareTo(DataPairLink<TData, TLink> other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other.Link));
        if (link != 0)
            return link;

        var data = Math.Abs(Comparer<TData>.Default.Compare(GetNeuron().RefData, other.Data));
        if (data != 0)
            return data;

        return 0;
    }

    public int CompareTo(TLink other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other));
        if (link != 0)
            return link;

        return 0;
    }

    public virtual IEnumerator<IConnection<TData, TLink>> GetEnumerator()
    {
        var current = GetSubConnection();
        while (current is not null)
        {
            yield return current;
            current = current.GetNextSubConnection();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public async IAsyncEnumerator<IConnection<TData, TLink>>
        GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var current = await GetSubConnectionAsync(cancellationToken);
        while (current is not null)
        {
            yield return current;
            current = await current.GetNextSubConnectionAsync(cancellationToken);
        }
    }
}