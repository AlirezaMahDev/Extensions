namespace AlirezaMahDev.Extensions.Abstractions;

public static class BinarySearchRangeExtensions
{
    extension(ref BinarySearchRange binarySearchRange)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryGetRange(out Range range)
        {
            if (binarySearchRange.Start < 0)
            {
                range = default;
                return false;
            }

            range = new(binarySearchRange.Start, binarySearchRange.End + 1);
            return true;
        }
    }
}