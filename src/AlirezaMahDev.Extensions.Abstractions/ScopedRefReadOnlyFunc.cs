namespace AlirezaMahDev.Extensions.Abstractions;

public delegate TResult ScopedRefReadOnlyFunc<T, out TResult>(scoped ref readonly T arg)
    where TResult : allows ref struct
    where T : allows ref struct;