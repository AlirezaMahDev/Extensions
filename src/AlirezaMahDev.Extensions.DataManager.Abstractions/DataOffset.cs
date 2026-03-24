using System.Runtime.Intrinsics;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct DataOffset(in int fileId, in int partIndex, in int offset, in int length)
    : IInEquatable<DataOffset>, IInEqualityOperators<DataOffset, DataOffset, bool>
{
    public readonly int FileId = fileId;
    public readonly int PartIndex = partIndex;
    public readonly int Offset = offset;
    public readonly int Length = length;

    public static readonly DataOffset Null = new(-1, -1, -1, -1);
    public static readonly DataOffset Default = new();
    private static readonly Vector128<byte> NullVector = Vector128.Create((byte)0xFF);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private ref byte AsByteRef()
        => ref Unsafe.As<DataOffset, byte>(ref Unsafe.AsRef(in this));

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
    public bool Equals(in DataOffset other)
        => Vector128.LoadUnsafe(ref AsByteRef()) ==
           Vector128.LoadUnsafe(ref Unsafe.As<DataOffset, byte>(ref Unsafe.AsRef(in other)));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static DataOffset Create(in long offset, in int length)
        => new(DataHelper.FileId(in offset),
            DataHelper.PartIndex(in offset),
            DataHelper.PartOffset(in offset),
            length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(DataOffset other) => Equals(in other);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj) =>
        obj is DataOffset dataOffset && Equals(in dataOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode() =>
        XxHash3.Combine(in FileId, in PartIndex, in Offset, in Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(in DataOffset left, in DataOffset right) => left.Equals(in right);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(in DataOffset left, in DataOffset right) => !left.Equals(in right);
}