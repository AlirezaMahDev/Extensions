namespace AlirezaMahDev.Extensions.Abstractions;

// ReSharper disable once UnusedTypeParameter
public struct ComparisonWrap<TComparisonTarget, T>(TComparisonTarget unWrap)
    where T : allows ref struct
{
    public TComparisonTarget UnWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set;
    } = unWrap;
}