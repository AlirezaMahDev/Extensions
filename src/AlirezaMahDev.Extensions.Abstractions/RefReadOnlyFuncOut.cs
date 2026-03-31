namespace AlirezaMahDev.Extensions.Abstractions;

public delegate void RefReadOnlyFuncOut<T, TResult>(ref readonly T arg, out TResult result)
    where TResult : allows ref struct
    where T : allows ref struct;