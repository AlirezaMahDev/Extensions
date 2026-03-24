namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ref TRef GetRefValueFunc<TSource, TRef>(ref TSource arg)
    where TSource : allows ref struct
    where TRef : allows ref struct;