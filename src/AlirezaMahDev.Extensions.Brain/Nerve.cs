namespace AlirezaMahDev.Extensions.Brain;

internal class Nerve<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
TData,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
TLink> : INerve<TData, TLink>, IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly DataLocation<DataPath> _location;
    private readonly DataLocation<DataPath> _connectionLocation;
    private readonly DataLocation<DataPath> _neuronLocation;
    private readonly Neuron _neuron;
    private readonly CellWrap<NeuronValue<TData>, TData, TLink> _rootNeuronWrap;
    private readonly Connection _connection;
    private readonly CellWrap<ConnectionValue<TLink>, TData, TLink> _rootConnectionWrap;
    private readonly DataLocation<DataPath> _counterLocation;
    private readonly DataLocation<NerveCounter> _counter;
    private readonly NerveCache _cache;

    public ConcurrentDictionary<DataOffset,
        Lazy<CellMemory<CellWrap<ConnectionValue<TLink>, TData, TLink>>>> MemoryCache
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public IDataAccess Access
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public INerveCache Cache
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _cache;
    }

    public string Name { get; }

    public ref readonly DataLocation<DataPath> Location
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _location;
    }

    public ref readonly DataLocation<DataPath> ConnectionLocation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _connectionLocation;
    }

    public ref readonly DataLocation<DataPath> NeuronLocation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _neuronLocation;
    }

    public ref readonly Neuron Neuron
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _neuron;
    }

    public ref readonly CellWrap<NeuronValue<TData>, TData, TLink> RootNeuronWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _rootNeuronWrap;
    }

    public ref readonly Connection Connection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _connection;
    }

    public ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> RootConnectionWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _rootConnectionWrap;
    }

    public ref readonly DataLocation<DataPath> CounterLocation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _counterLocation;
    }

    public ref readonly DataLocation<NerveCounter> Counter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _counter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Nerve(IDataManager dataManager, string name)
    {
        _cache = new();

        MemoryCache = new();
        Name = name;
        Access = Name.StartsWith("temp:") ? dataManager.OpenTemp() : dataManager.Open(name);
        var root = Access.Root;
        _location = root.Wrap(Access, x => x.Dictionary()).GetOrAdd(".nerve");
        _connectionLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".connection");
        _neuronLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".neuron");
        _counterLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".counter");

        var neuron = NeuronLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = ConnectionLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(ConnectionValue<TLink>.Default with { Neuron = new(neuron.Offset) });
        var dataWrap = _counterLocation.Wrap(Access, x => x.Storage());
        _counter = dataWrap.GetOrCreateData(NerveCounter.Default);

        _neuron = new(neuron.Offset);
        _rootNeuronWrap = Neuron.NewWrap(this);
        _connection = new(connection.Offset);
        _rootConnectionWrap = Connection.NewWrap(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        Access.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _cache.Dispose();
        this.CleanThink();
    }
}