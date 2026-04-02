namespace AlirezaMahDev.Extensions.Abstractions;

public delegate TResult ScopedRefReadOnlyFunc<out TResult>()
    where TResult : allows ref struct;

public delegate TResult ScopedRefReadOnlyFunc<T, out TResult>(scoped ref readonly T arg)
    where TResult : allows ref struct
    where T : allows ref struct;

public delegate TResult ScopedRefReadOnlyFunc<T1, T2, out TResult>(scoped ref readonly T1 t1,
    scoped ref readonly T2 t2)
    where TResult : allows ref struct
    where T1 : allows ref struct
    where T2 : allows ref struct;

public delegate void ScopedRefReadOnlyAction<T>(scoped ref readonly T arg)
    where T : allows ref struct;

public delegate void ScopedRefReadOnlyAction<T1, T2>(scoped ref readonly T1 t1, scoped ref readonly T2 t2)
    where T1 : allows ref struct
    where T2 : allows ref struct;