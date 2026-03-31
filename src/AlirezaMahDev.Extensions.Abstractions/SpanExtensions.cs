namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanExtensions
{
    extension<T>(Span<T> span)
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<TOut> Select<TOut>(RefFunc<T, TOut> refFunc)
        {
            var result = new TOut[span.Length];
            for (var index = 0; index < span.Length; index++)
            {
                result[index] = refFunc(ref span[index]);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ref T First()
        {
            if (span.IsEmpty)
            {
                throw new("span is empty");
            }

            return ref span[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ref T First(RefFunc<T, bool> predicate)
        {
            if (span.IsEmpty)
            {
                throw new("span is empty");
            }

            foreach (ref var item in span)
            {
                if (predicate.Invoke(ref item))
                {
                    return ref item;
                }
            }

            throw new("not found item");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ref T FirstOrDefault()
        {
            return ref span.IsEmpty ? ref Unsafe.NullRef<T>() : ref span[0];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ref T FirstOrDefault(RefFunc<T, bool> predicate)
        {
            if (span.IsEmpty)
            {
                return ref Unsafe.NullRef<T>();
            }

            foreach (ref var item in span)
            {
                if (predicate.Invoke(ref item))
                {
                    return ref item;
                }
            }

            return ref Unsafe.NullRef<T>();
        }
    }
}