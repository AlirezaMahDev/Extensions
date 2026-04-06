namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ref readonly TRef RefReadOnlyValueFunc<TSource, TRef>(ref readonly TSource arg)
    where TSource : allows ref struct
    where TRef : allows ref struct;