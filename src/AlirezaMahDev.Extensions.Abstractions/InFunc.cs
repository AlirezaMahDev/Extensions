namespace AlirezaMahDev.Extensions.Abstractions;

public delegate TResult InFunc<T, out TResult>(in T arg);
