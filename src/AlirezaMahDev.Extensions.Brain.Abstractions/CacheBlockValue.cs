// namespace AlirezaMahDev.Extensions.Brain.Abstractions;
//
// [StructLayout(LayoutKind.Sequential, Pack = 16)]
// public struct CacheBlockValue : IDataValue<CacheBlockValue>,
//     IDataStorage<CacheBlockValue>,
//     IDataValueDefault<CacheBlockValue>,
//     IDataCollectionItem<CacheBlockValue>
// {
//     private DataOffset _data;
//     private DataOffset _next;
//     private DataLock _lock;
//     public ushort Count;
//     public ushort Capacity;
//
//     public ref DataLock Lock
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//         get => ref Unsafe.AsRef(in this)._lock;
//     }
//
//     public ref DataOffset Data
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//         get => ref Unsafe.AsRef(in this)._data;
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//     public bool Equals(scoped ref readonly CacheBlockValue other)
//     {
//         throw new NotImplementedException();
//     }
//
//     private static readonly CacheBlockValue DefaultField = new()
//     {
//         _data = DataOffset.Null,
//         Next = DataOffset.Null,
//         Count = 0,
//         Capacity = 0,
//     };
//
//     public static ref readonly CacheBlockValue Default
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//         get => ref DefaultField;
//     }
//
//     public ref DataOffset Next
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//         get => ref Unsafe.AsRef(in this)._next;
//     }
// }