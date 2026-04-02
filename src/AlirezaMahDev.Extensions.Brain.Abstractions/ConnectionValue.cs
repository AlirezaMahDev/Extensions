namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ConnectionValue<TLink> :
    ICellValueDefault<ConnectionValue<TLink>>,
    ICellScoreValue,
    ICellWeightValue
    where TLink : unmanaged, ICellLink<TLink>
{
    public TLink Link;

    public Neuron Neuron;
    public Connection Child;
    public Connection Next;
    public Connection Previous;
    private DataLock _lock;

    public int Count;
    private uint _weight;
    private float _score;


    public readonly ref DataLock Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._lock;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly ConnectionValue<TLink> other)
    {
        return Link.Equals(in other.Link) &&
               Neuron == other.Neuron &&
               Child == other.Child &&
               Next == other.Next &&
               Previous == other.Previous &&
               Count == other.Count &&
               _weight == other._weight &&
               _score == other._score;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ConnectionValue<TLink> connectionValue && Equals(ref connectionValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override readonly int GetHashCode()
    {
        using var xxHash3Builder = XxHash3.Builder();
        xxHash3Builder.Add(in Link);
        xxHash3Builder.Add(in Neuron);
        xxHash3Builder.Add(in Child);
        xxHash3Builder.Add(in Next);
        xxHash3Builder.Add(in Previous);
        xxHash3Builder.Add(in Count);
        xxHash3Builder.Add(in _weight);
        xxHash3Builder.Add(in _score);
        return xxHash3Builder.ToHashCode();
    }

    private static readonly ConnectionValue<TLink> DefaultField = new()
    {
        Neuron = Neuron.Null,
        Next = Connection.Null,
        Count = 0,
        Child = Connection.Null,
        Previous = Connection.Null,
        _score = 1f,
        _weight = 0u,
        Link = default
    };

    public static ref readonly ConnectionValue<TLink> Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
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
        return $"{Link}";
    }

    public static bool operator ==(ConnectionValue<TLink> left, ConnectionValue<TLink> right)
    {
        return left.Equals(in right);
    }

    public static bool operator !=(ConnectionValue<TLink> left, ConnectionValue<TLink> right)
    {
        return !(left == right);
    }
}