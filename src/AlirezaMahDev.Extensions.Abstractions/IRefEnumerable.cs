namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefEnumerable<TSelf, T, TRefEnumerator> : IRefEnumerableCore<TRefEnumerator>
    where TSelf : IRefEnumerable<TSelf, T, TRefEnumerator>, allows ref struct
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct;

public ref struct RefStructWrap<TSelf, TImplementation>(TSelf self)
    where TSelf : allows ref struct
{
    public TSelf Self = self;
}

public interface IRefStructWrap<TSelf, TImplementation>
    where TSelf : allows ref struct
{
    public static virtual RefStructWrap<TSelf, TImplementation> Create(TSelf self) =>
        new(self);
}

// public static class RefStructWrapExtensions
// {
//     extension<TSelf>(TSelf self)
//     {
//         public RefStructWrap<TSelf, TImplementation> AsWrap<TImplementation>()
//             where TSelf : IRefStructWrap<TSelf, TImplementation>, allows ref struct =>
//                 IRefStructWrap<TSelf, TImplementation>.Create(self);
//     }
// }


public static class RefEnumerableExtensions
{
    extension<TSelf, T, TRefEnumerator>(TSelf self)
        where TSelf : IRefEnumerable<TSelf, T, TRefEnumerator>, allows ref struct
        where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct
    {
        public void CopyTo(Span<T> values)
        {
            var index = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext())
            {
                values[index++] = enumerator.Current;
            }
        }

    }
}
public static class RefReadOnlyEnumerableExtensions
{
    extension<TSelf, T, TRefReadOnlyEnumerator>(TSelf self)
        where TSelf : IRefReadOnlyEnumerable<TSelf, T, TRefReadOnlyEnumerator>, allows ref struct
        where TRefReadOnlyEnumerator : IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>, allows ref struct
    {
        public void CopyTo(Span<T> values)
        {
            var index = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext())
            {
                values[index++] = enumerator.Current;
            }
        }
    }
}