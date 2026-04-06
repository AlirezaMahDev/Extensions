// namespace AlirezaMahDev.Extensions.Brain.Abstractions;
//
// [StructLayout(LayoutKind.Auto)]
// [method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
// public ref struct CacheBlockManagerRefEnumerator(ref readonly DataWrap<CacheBlockValue, DataEmptyWrap> wrap)
//     : IRefReadOnlyEnumerator<CacheBlockManagerRefEnumerator, DataWrap<CacheBlockValue, DataEmptyWrap>>
// {
//     private bool _initialize;
//     private readonly ref readonly DataWrap<CacheBlockValue, DataEmptyWrap> _wrap = ref wrap;
//
//     private DataWrap<CacheBlockValue, DataEmptyWrap> _current = default;
//
//     public ref readonly DataWrap<CacheBlockValue, DataEmptyWrap> Current
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//         get => ref Unsafe.AsRef(in this)._current;
//     }
//
//     public bool MoveNext()
//     {
//         if (!_initialize)
//         {
//             _initialize = true;
//             _current = _wrap;
//             return true;
//         }
//
//         var currentNextWrap = _current.NextWrap;
//         if (currentNextWrap.HasValue)
//         {
//             _current = currentNextWrap.Value;
//             return true;
//         }
//
//         _current = default;
//         return false;
//     }
// }