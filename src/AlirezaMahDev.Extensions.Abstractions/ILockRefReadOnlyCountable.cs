namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefReadOnlyCountable<TSelf, T, TLockRefReadOnlyEnumerator> : ILockRefReadOnlyEnumerable<TSelf, T, TLockRefReadOnlyEnumerator>, IRefLength
    where TSelf : ILockRefReadOnlyCountable<TSelf, T, TLockRefReadOnlyEnumerator>, allows ref struct
    where TLockRefReadOnlyEnumerator : ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>, allows ref struct;