namespace AlirezaMahDev.Extensions.Abstractions;

public delegate void ScopedRefAction<T>(scoped ref T arg)
    where T : allows ref struct;