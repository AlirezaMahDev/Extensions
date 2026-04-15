namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefLength
{
    int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}