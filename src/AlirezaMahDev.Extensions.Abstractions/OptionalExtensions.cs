namespace AlirezaMahDev.Extensions.Abstractions;

public static class OptionalExtensions
{
    extension<TValue>(in Optional<TValue> value)
        where TValue : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<TResult> WhenNotNull<TResult>(ScopedRefReadOnlyFunc<TValue, TResult> func)
            where TResult : struct =>
            !value.HasValue ? Optional<TResult>.Null : func(in value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<TResult> WhenNotNull<TResult, TState>(ScopedRefReadOnlyFunc<TValue, TState, TResult> func,
            scoped in TState state)
            where TResult : struct =>
            !value.HasValue ? Optional<TResult>.Null : func(in value.Value, in state);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<TValue> WhenNull(ScopedRefReadOnlyFunc<TValue> func) =>
            value.HasValue ? value : func();
    }
}