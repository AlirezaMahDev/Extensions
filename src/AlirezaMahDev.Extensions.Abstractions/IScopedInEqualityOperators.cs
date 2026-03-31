using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IScopedInEqualityOperators<TSelf, TOther, TResult> : IEqualityOperators<TSelf, TOther, TResult>
    where TSelf : IScopedInEqualityOperators<TSelf, TOther, TResult>?
{
    public static abstract TResult operator ==(scoped in TSelf? left,scoped in TOther? right);

    static TResult IEqualityOperators<TSelf, TOther, TResult>.operator ==(TSelf? left, TOther? right)
    {
        ref var refLeft = ref left;
        ref var refRight = ref right;
        return refLeft == refRight;
    }

    public static abstract TResult operator !=(scoped in TSelf? left, scoped in TOther? right);

    static TResult IEqualityOperators<TSelf, TOther, TResult>.operator !=(TSelf? left, TOther? right)
    {
        ref var refLeft = ref left;
        ref var refRight = ref right;
        return refLeft != refRight;
    }
}