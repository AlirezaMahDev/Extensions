namespace AlirezaMahDev.Extensions.Abstractions;

public delegate TResult RefFunc<T, out TResult>(ref T arg)
    where TResult : allows ref struct
    where T : allows ref struct;