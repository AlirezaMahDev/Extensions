namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefEnumerableCore<TLockRefEnumeratorCore>
    where TLockRefEnumeratorCore : allows ref struct
{
    TLockRefEnumeratorCore GetEnumerator();
}