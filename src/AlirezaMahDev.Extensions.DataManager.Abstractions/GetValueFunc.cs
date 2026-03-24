namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TValue GetValueFunc<TSource, out TValue>(ref TSource arg)
    where TSource : allows ref struct
    where TValue : allows ref struct;