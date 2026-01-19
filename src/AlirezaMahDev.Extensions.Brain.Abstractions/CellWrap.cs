using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public readonly record struct CellWrap<TCell, TValue, TData, TLink>(INerve<TData, TLink> Nerve, TCell Cell)
    : ICellWrap<TCell, TValue, TData, TLink>
    where TCell : ICell
    where TValue : unmanaged, ICellValue<TValue>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public INerve<TData, TLink> Nerve { get; } = Nerve;
    public TCell Cell { get; } = Cell;

    public DataWrap<TValue> Location => new(Nerve.Access, new(Cell.Offset));
    public ref readonly TValue RefValue => ref Location.RefValue;
    public void Lock(DataLocationAction<TValue> action) => Location.Lock(action);

    public TResult Lock<TResult>(DataLocationFunc<TValue, TResult> func) =>
        Location.Lock(func);

    public async ValueTask LockAsync(DataLocationAction<TValue> action,
        CancellationToken cancellationToken = default) =>
        await Location.LockAsync(action, cancellationToken);

    public async ValueTask LockAsync(DataLocationAsyncAction<TValue> action,
        CancellationToken cancellationToken = default) =>
        await Location.LockAsync(action, cancellationToken);

    public ValueTask<TResult> LockAsync<TResult>(DataLocationFunc<TValue, TResult> func,
        CancellationToken cancellationToken = default) =>
        Location.LockAsync(func, cancellationToken);

    public ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<TValue, TResult> func,
        CancellationToken cancellationToken = default) =>
        Location.LockAsync(func, cancellationToken);

    public override string ToString()
    {
        return $"{Cell} {RefValue}";
    }
}