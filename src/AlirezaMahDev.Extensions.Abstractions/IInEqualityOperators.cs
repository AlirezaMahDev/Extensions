using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IInEqualityOperators<TSelf, TOther, TResult> : IEqualityOperators<TSelf, TOther, TResult>
    where TSelf : IInEqualityOperators<TSelf, TOther, TResult>?
{
    public static abstract TResult operator ==(in TSelf? left, in TOther? right);

    static TResult IEqualityOperators<TSelf, TOther, TResult>.operator ==(TSelf? left, TOther? right)
    {
        ref var refLeft = ref left;
        ref var refRight = ref right;
        return refLeft == refRight;
    }

    public static abstract TResult operator !=(in TSelf? left, in TOther? right);

    static TResult IEqualityOperators<TSelf, TOther, TResult>.operator !=(TSelf? left, TOther? right)
    {
        ref var refLeft = ref left;
        ref var refRight = ref right;
        return refLeft != refRight;
    }
}