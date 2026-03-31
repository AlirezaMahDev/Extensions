namespace AlirezaMahDev.Extensions.Abstractions;

public delegate TResult RefReadOnlyFunc<T, out TResult>(ref readonly T arg)
    where TResult : allows ref struct
    where T : allows ref struct;