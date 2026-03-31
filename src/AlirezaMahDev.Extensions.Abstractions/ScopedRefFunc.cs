namespace AlirezaMahDev.Extensions.Abstractions;

public delegate TResult ScopedRefFunc<T, out TResult>(scoped ref T arg)
    where TResult : allows ref struct
    where T : allows ref struct;