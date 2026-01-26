namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ValueTask DataWrapAsyncAction<TValue>(DataWrap<TValue> wrap,
    CancellationToken cancellationToken = default)
    where TValue : unmanaged, IDataValue<TValue>;