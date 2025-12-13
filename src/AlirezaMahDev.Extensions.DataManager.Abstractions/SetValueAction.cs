namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void SetValueAction<TSource, in TValue>(ref TSource source, TValue value)
    where TSource : allows ref struct
    where TValue : allows ref struct;