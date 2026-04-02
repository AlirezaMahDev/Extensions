namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct NeuronValue<TData> :
    ICellValueDefault<NeuronValue<TData>>,
    ICellScoreValue,
    ICellWeightValue
    where TData : unmanaged, ICellData<TData>
{
    public static readonly NeuronValue<TData> DefaultField = new()
    {
        Next = Neuron.Null,
        Data = default,
        _score = 1f,
        _weight = 0u
    };

    public static ref readonly NeuronValue<TData> Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }

    public TData Data;
    public Neuron Next;
    private DataLock _lock;

    private uint _weight;
    private float _score;


    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(scoped ref readonly NeuronValue<TData> other)
    {
        return Data == other.Data &&
               Next == other.Next &&
               _weight == other._weight &&
               _score == other._score;
    }

    public readonly ref float Score
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._score;
    }

    public readonly ref uint Weight
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._weight;
    }

    public override readonly string ToString()
    {
        return $"{Data}";
    }
}