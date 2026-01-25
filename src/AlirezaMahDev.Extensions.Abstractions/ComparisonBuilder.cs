namespace AlirezaMahDev.Extensions.Abstractions;

public delegate ComparisonWrap<TComparisonTarget, T> ComparisonBuilder<TComparisonTarget, T>(
    ComparisonWrap<TComparisonTarget, T> comparisonChain);