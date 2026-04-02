namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct RefEnumeratorToRefReadOnlyEnumerator<TSelf, T>(TSelf refEnumerator)
    : IRefReadOnlyEnumerator<RefEnumeratorToRefReadOnlyEnumerator<TSelf, T>, T>
    where TSelf : IRefEnumerator<TSelf, T>, allows ref struct
{
    private readonly TSelf _refEnumerator = refEnumerator;
    public ref readonly T Current => ref _refEnumerator.Current;

    public bool MoveNext() => _refEnumerator.MoveNext();
}