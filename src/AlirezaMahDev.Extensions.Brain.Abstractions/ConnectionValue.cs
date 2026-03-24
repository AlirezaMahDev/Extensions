namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ConnectionValue<TLink> :
    ICellValueDefault<ConnectionValue<TLink>>,
    ICellScoreValue,
    ICellWeightValue
    where TLink : unmanaged, ICellLink<TLink>
{
    public TLink Link;

    public DataOffset Neuron;
    public DataOffset Child;
    public DataOffset Next;
    public DataOffset Previous;

    public int NextCount;
    public uint Weight;
    public float Score;
    public int _lock;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in ConnectionValue<TLink> other)
    {
        return Link.Equals(in other.Link) &&
               Neuron == other.Neuron &&
               Child == other.Child &&
               Next == other.Next &&
               Previous == other.Previous &&
               NextCount == other.NextCount &&
               Weight == other.Weight &&
               Score == other.Score;
    }

    private static readonly ConnectionValue<TLink> DefaultField = new()
    {
        Neuron = DataOffset.Null,
        Next = DataOffset.Null,
        NextCount = 0,
        Child = DataOffset.Null,
        Previous = DataOffset.Null,
        Score = 1f,
        Weight = 0u,
        Link = default
    };

    public static ref readonly ConnectionValue<TLink> Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref DefaultField;
    }


    public readonly ref int Lock
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._lock;
        }
    }

    public readonly ref float RefScore
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this).Score;
        }
    }

    public readonly ref uint RefWeight
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this).Weight;
        }
    }
}