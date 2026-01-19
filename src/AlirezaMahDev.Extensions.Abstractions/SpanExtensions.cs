using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanExtensions
{
    extension<T>(Span<T> span)
    {
        public ref T First()
        {
            if (span.IsEmpty)
            {
                throw new("span is empty");
            }

            return ref span[0];
        }

        public ref T First(InFunc<T, bool> predicate)
        {
            if (span.IsEmpty)
            {
                throw new("span is empty");
            }

            foreach (ref var item in span)
            {
                if (predicate.Invoke(in item))
                {
                    return ref item;
                }
            }

            throw new("not found item");
        }

        public ref T FirstOrDefault()
        {
            if (span.IsEmpty)
            {
                return ref Unsafe.NullRef<T>();
            }

            return ref span[0];
        }


        public ref T FirstOrDefault(InFunc<T, bool> predicate)
        {
            if (span.IsEmpty)
            {
                return ref Unsafe.NullRef<T>();
            }

            foreach (ref var item in span)
            {
                if (predicate.Invoke(in item))
                {
                    return ref item;
                }
            }

            return ref Unsafe.NullRef<T>();
        }
    }
}