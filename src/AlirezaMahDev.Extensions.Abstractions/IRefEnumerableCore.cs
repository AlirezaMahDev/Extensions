namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefEnumerableCore<TRefEnumeratorCore>
    where TRefEnumeratorCore : allows ref struct
{
    TRefEnumeratorCore GetEnumerator();
}
