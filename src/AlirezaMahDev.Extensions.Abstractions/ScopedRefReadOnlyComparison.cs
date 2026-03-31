namespace AlirezaMahDev.Extensions.Abstractions;

public delegate int ScopedRefReadOnlyComparison<T>(scoped ref readonly T x, scoped ref readonly T y)
    where T : allows ref struct;