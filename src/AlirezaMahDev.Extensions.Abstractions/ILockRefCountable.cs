namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefCountable<TSelf, T, TLockRefEnumerator> : ILockRefEnumerable<TSelf, T, TLockRefEnumerator>, IRefLength
    where TSelf : ILockRefCountable<TSelf, T, TLockRefEnumerator>, allows ref struct
    where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct;