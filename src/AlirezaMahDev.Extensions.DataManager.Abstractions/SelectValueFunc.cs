namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult SelectValueFunc<in T, out TResult>(T arg)
    where T : allows ref struct
    where TResult : allows ref struct;