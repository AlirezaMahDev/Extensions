namespace AlirezaMahDev.Extensions.Abstractions;

public static class ScoreSortMemoryExtensions
{
    extension<T>(Memory<T> memory)
        where T : notnull
    {
        public ScoreSortMemoryWrap<T> AsScoreSort() =>
            new(memory);
    }

    extension<T>(Span<T> input)
        where T : IScoreSortItem
    {
        public Span<T> TakeBestScoreSort(params ReadOnlySpan<Comparison<T>> comparisons) =>
            input.TakeBestScoreSort(0, comparisons);

        public Span<T> TakeBestScoreSort(int depth, params ReadOnlySpan<Comparison<T>> comparisons) =>
            input[..input.BestScoreSort(depth, comparisons)];

        public int BestScoreSort(params ReadOnlySpan<Comparison<T>> comparisons) =>
            input.BestScoreSort(0, comparisons);

        public int BestScoreSort(int depth, params ReadOnlySpan<Comparison<T>> comparisons)
        {
            input.ScoreSort(comparisons);
            ref var target = ref input[0];
            var count = 1;
            for (int i = 1; i < input.Length; i++)
            {
                ref var current = ref input[i];
                if (target.Score != current.Score)
                {
                    if (depth == 0)
                    {
                        break;
                    }

                    target = ref current;
                    depth--;
                }

                count++;
            }

            return count;
        }

        public void ScoreSort(params ReadOnlySpan<Comparison<T>> comparisons)
        {
            input.InitializeSort();

            foreach (ref readonly var comparison in comparisons)
            {
                Span<T>.ComparisonSort(input, comparison);
            }

            Span<T>.FinalSort(input);
        }

        private void InitializeSort()
        {
            foreach (ref var t in input)
            {
                t.Score = 0;
            }
        }

        private static void ComparisonSort(Span<T> span, Comparison<T> comparison)
        {
            span.Sort(comparison);
            var score = 0;
            ref var target = ref span[0];
            for (int i = 1; i < span.Length; i++)
            {
                ref var current = ref span[i];
                var comparable = comparison(target, current);
                if (comparable == 0)
                {
                    current.Score += score;
                }
                else
                {
                    score++;
                    current.Score += score;
                    target = ref current;
                }
            }
        }

        private static void FinalSort(Span<T> span)
        {
            span.Sort(ScoreSortItemComparer<T>.Default);
        }
    }
}