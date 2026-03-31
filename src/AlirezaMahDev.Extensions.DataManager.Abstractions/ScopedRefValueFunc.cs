namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ref TRef ScopedRefValueFunc<TSource, TRef>(scoped ref TSource arg)
    where TSource : allows ref struct
    where TRef : allows ref struct;