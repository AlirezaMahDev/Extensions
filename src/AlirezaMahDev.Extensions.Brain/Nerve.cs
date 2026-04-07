using AlirezaMahDev.Extensions.Abstractions;

using Microsoft.Extensions.Options;

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

    public ref readonly Neuron RootNeuron
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _neuron;
    }

    public ref readonly CellWrap<NeuronValue<TData>, TData, TLink> RootNeuronWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _rootNeuronWrap;
    }

    public ref readonly Connection RootConnection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _connection;
    }

    public ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> RootConnectionWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _rootConnectionWrap;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Nerve(IDataManager dataManager, string name, IOptions<DataManagerOptions> options)
    {
        _cache = new(options.Value.DirectoryPath);

        MemoryCache = new();
        Name = name;
        Access = Name.StartsWith("temp:") ? dataManager.OpenTemp() : dataManager.Open(name);
        var root = Access.Root;
        _location = root.Wrap(x => x.Dictionary()).GetOrAdd(".nerve");
        var rootDictionary = _location.Wrap(x => x.Dictionary());
        _connectionLocation = rootDictionary.GetOrAdd(".connection");
        _neuronLocation = rootDictionary.GetOrAdd(".neuron");

        var neuron = _neuronLocation
            .Wrap(x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = _connectionLocation
            .Wrap(x => x.Storage())
            .GetOrCreateData(ConnectionValue<TLink>.Default with { Neuron = new(neuron.Offset) });

        _neuron = new(neuron.Offset);
        _rootNeuronWrap = _neuron.NewWrap(this);
        _connection = new(connection.Offset);
        _rootConnectionWrap = _connection.NewWrap(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        FlushCore();
        Access.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void ClearMemoryCache()
    {
        SmartParallel.ForEach(MemoryCache.Where(x => x.Value.IsValueCreated),
            CancellationToken.None,
            (pair, _) => pair.Value.Value.Dispose());
        MemoryCache.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void FlushCore()
    {
        ClearMemoryCache();
        _cache.Flush();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        FlushCore();
        _cache.Dispose();
    }
}