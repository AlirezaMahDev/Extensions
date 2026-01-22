namespace AlirezaMahDev.Extensions.Abstractions;

public static class ScoreSortExtensions
{
    extension<T>(Memory<T> memory)
        where T : notnull
    {
        public ScoreSortComparer<T> AsScoreSort() =>
            new(memory);
    }

    extension<T>(Span<T> input)
        where T : IScoreSortItem
    {
        public Span<T> TakeBestScoreSort(int depth, ComparisonBuilder<ComparisonCollectionChain<T>, T> builder) =>
            input[..input.BestScoreSort(depth, builder)];

        public int BestScoreSort(int depth, ComparisonBuilder<ComparisonCollectionChain<T>, T> builder)
        {
            var collectionChain =input.ScoreSortCore(builder);
            var comparer = Span<T>.FinalSort(input, collectionChain);
            return input.BestScoreSortCore(depth, comparer);
        }

        private int BestScoreSortCore(int depth, Comparison<T> comparer)
        {
            ref var target = ref input[0];
            var count = 1;
            for (int i = 1; i < input.Length; i++)
            {
                ref var current = ref input[i];
                if (comparer(target, current) != 0)
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

        public void ScoreSort(ComparisonBuilder<ComparisonCollectionChain<T>, T> builder)
        {
            var collectionChain = input.ScoreSortCore(builder);
            Span<T>.FinalSort(input,collectionChain);
        }

        public void ScoreSortChain(ComparisonBuilder<ComparisonCollectionChain<T>, T> builder)
        {
            var collectionChain =  input.ScoreSortCore(builder);
            Span<T>.FinalSort(input, collectionChain);
        }


        private ComparisonCollectionChain<T> ScoreSortCore(ComparisonBuilder<ComparisonCollectionChain<T>, T> builder)
        {
            input.InitializeSort();

            var collectionChain = builder(ComparisonCollectionChain<T>.OrderBy(ScoreSortHelper<T>.Comparison).Wrap())
                .UnWrap;
            foreach (var comparison in collectionChain.Enumerable)
            {
                Span<T>.ComparisonSort(input, comparison);
            }

            return collectionChain;
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

        private static Comparison<T> FinalSort(Span<T> span, ComparisonCollectionChain<T> collectionChain)
        {
            var comparison = collectionChain.Comparison;
            span.Sort(comparison);
            return comparison;
        }
    }
}