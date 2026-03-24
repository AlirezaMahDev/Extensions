namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapExtensions
{
    extension<TCell, TValue, TData, TLink>(ref readonly CellWrap<TCell, TValue, TData, TLink> cellWrap)
        where TCell : ICell
        where TValue : unmanaged, ICellValue<TValue>
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [MustDisposeResource]
        public DataLockDisposable<TValue> Lock()
        {
            return cellWrap.Location.Lock();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Lock(DataWrapAction<TValue> action)
        {
            cellWrap.Location.Lock(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult Lock<TResult>(DataWrapFunc<TValue, TResult> func)
        {
            return cellWrap.Location.Lock(func);
        }
    }
}