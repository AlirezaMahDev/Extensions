using System.Runtime.Intrinsics;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Size = 16, Pack = 1)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct DataOffset(int fileId, int partIndex, int offset, int length)
    : IScopedRefReadOnlyEquatable<DataOffset>, IScopedInEqualityOperators<DataOffset, DataOffset, bool>
{
    public readonly int FileId = fileId;
    public readonly int PartIndex = partIndex;
    public readonly int Offset = offset;
    public readonly int Length = length;

    public static readonly DataOffset Null = new(-1, -1, -1, -1);
    public static readonly DataOffset Default;
    private static readonly Vector128<byte> NullVector = Vector128.Create((byte)0xFF);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private ref byte AsByteRef()
    {
        return ref Unsafe.As<DataOffset, byte>(ref Unsafe.AsRef(in this));
    }

    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Vector128.LoadUnsafe(ref AsByteRef()) == NullVector;
    }

    public bool IsDefault
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Vector128.LoadUnsafe(ref AsByteRef()) == Vector128<byte>.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly DataOffset other)
    {
        return Vector128.LoadUnsafe(ref AsByteRef()) ==
               Vector128.LoadUnsafe(ref other.AsByteRef());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static DataOffset Create(long offset, int length)
    {
        return new(DataHelper.FileId(offset),
            DataHelper.PartIndex(offset),
            DataHelper.PartOffset(offset),
            length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(DataOffset other)
    {
        return Equals(ref other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is DataOffset dataOffset && Equals(ref dataOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return XxHash3.Combine(in FileId, in PartIndex, in Offset, in Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(scoped in DataOffset left, scoped in DataOffset right)
    {
        return left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(scoped in DataOffset left, scoped in DataOffset right)
    {
        return !left.Equals(in right);
    }

    public override string ToString()
    {
        return $"{FileId}_{PartIndex}_{Offset}_{Length}";
    }
}