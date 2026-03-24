namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct NeuronValue<TData> :
    ICellValueDefault<NeuronValue<TData>>,
    ICellScoreValue,
    ICellWeightValue
    where TData : unmanaged, ICellData<TData>
{
    public TData Data;

    public DataOffset Connection;

    public uint Weight;
    public float Score;
    public int _lock;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in NeuronValue<TData> other)
    {
        return Data.Equals(in other.Data) &&
               Connection == other.Connection &&
               Weight == other.Weight &&
               Score == other.Score;
    }

    public static readonly NeuronValue<TData> DefaultField = new()
    {
        Connection = DataOffset.Null,
        Data = default,
        Score = 1f,
        Weight = 0u
    };

    public static ref readonly NeuronValue<TData> Default
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