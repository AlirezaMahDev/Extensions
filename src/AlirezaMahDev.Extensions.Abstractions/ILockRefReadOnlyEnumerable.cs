namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefReadOnlyEnumerable<TSelf, T, TLockRefReadOnlyEnumerator> : ILockRefEnumerableCore<TLockRefReadOnlyEnumerator>
    where TSelf : ILockRefReadOnlyEnumerable<TSelf, T, TLockRefReadOnlyEnumerator>, allows ref struct
    where TLockRefReadOnlyEnumerator : ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>, allows ref struct;