// namespace AlirezaMahDev.Extensions.Brain.Abstractions;
//
// [StructLayout(LayoutKind.Auto)]
// [method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
// public readonly ref struct CacheBlockManagerRefDictionary(ref readonly CacheBlockManagerRefList managerRefList)
//     : IRefDictionary<CacheBlockManagerRefDictionary, UInt128, DataOffset>
// {
//     public int Length { get; }
//
//     public bool TryGet(in UInt128 key, out DataOffset value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryAdd(in UInt128 key, in DataOffset value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryAdd(in UInt128 key, ScopedRefReadOnlyFunc<UInt128, DataOffset> func)
//     {
//         throw new NotImplementedException();
//     }
//
//     public DataOffset GetOrAdd(in UInt128 key, in DataOffset value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public DataOffset GetOrAdd(in UInt128 key, ScopedRefReadOnlyFunc<UInt128, DataOffset> func)
//     {
//         throw new NotImplementedException();
//     }
//
//     public bool TryRemove(in UInt128 key)
//     {
//         throw new NotImplementedException();
//     }
// }