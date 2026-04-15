namespace AlirezaMahDev.Extensions.Abstractions;

public interface
    IRefReadOnlyCountable<TSelf, T, TRefReadOnlyEnumerator> : IRefReadOnlyEnumerable<TSelf, T, TRefReadOnlyEnumerator>,
    IRefLength
    where TSelf : IRefReadOnlyCountable<TSelf, T, TRefReadOnlyEnumerator>, allows ref struct
    where TRefReadOnlyEnumerator : IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>, allows ref struct;