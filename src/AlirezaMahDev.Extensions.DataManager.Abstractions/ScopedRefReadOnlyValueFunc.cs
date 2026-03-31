namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ref readonly TRef ScopedRefReadOnlyValueFunc<TSource, TRef>(scoped ref readonly TSource arg)
    where TSource : allows ref struct
    where TRef : allows ref struct;