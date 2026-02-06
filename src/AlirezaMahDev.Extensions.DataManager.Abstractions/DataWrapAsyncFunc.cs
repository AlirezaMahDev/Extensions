namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ValueTask<TResult> DataWrapAsyncFunc<TValue, TResult>(DataWrap<TValue> wrap,
    CancellationToken cancellationToken = default)
    where TValue : unmanaged, IDataValue<TValue>;