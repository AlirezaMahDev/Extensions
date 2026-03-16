using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class ScoreSortExtensions
{
    extension<T>(Memory<T> memory)
        where T : notnull
    {
        [MustDisposeResource]
        public ScoreSortComparer<T> AsScoreSort()
        {
            return new(memory);
        }
    }

    extension<T>(Span<T> input)
        where T : IScoreSortItem
    {
        public Span<T> TakeBestScoreSort(int depth, ComparisonBuilder<ComparisonCollectionChain<T>, T> builder)
        {
            return input[..input.BestScoreSort(depth, builder)];
        }

        public int BestScoreSort(int depth, ComparisonBuilder<ComparisonCollectionChain<T>, T> builder)
        {
            ComparisonCollectionChain<T> collectionChain = input.ScoreSortCore(builder);
            Comparison<T> comparer = Span<T>.FinalSort(input, collectionChain);
            return input.BestScoreSortCore(depth, comparer);
        }

        private int BestScoreSortCore(int depth, Comparison<T> comparer)
        {
            ref T target = ref input[0];
            int count = 1;
            for (int i = 1; i < input.Length; i++)
            {
                ref T current = ref input[i];
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
            ComparisonCollectionChain<T> collectionChain = input.ScoreSortCore(builder);
            Span<T>.FinalSort(input, collectionChain);
        }

        private ComparisonCollectionChain<T> ScoreSortCore(ComparisonBuilder<ComparisonCollectionChain<T>, T> builder)
        {
            input.InitializeSort();

            ComparisonCollectionChain<T> collectionChain =
                builder(ComparisonCollectionChain<T>.OrderBy(ScoreSortHelper<T>.Comparison).Wrap())
                    .UnWrap;
            foreach (Comparison<T> comparison in collectionChain.Enumerable)
            {
                Span<T>.ComparisonSort(input, comparison);
            }

            return collectionChain;
        }

        private void InitializeSort()
        {
            foreach (ref T t in input)
            {
                t.Score = 0;
            }
        }

        private static void ComparisonSort(Span<T> span, Comparison<T> comparison)
        {
            span.Sort(comparison);
            int score = 0;
            ref T target = ref span[0];
            for (int i = 1; i < span.Length; i++)
            {
                ref T current = ref span[i];
                int comparable = comparison(target, current);
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
            Comparison<T> comparison = collectionChain.Comparison;
            span.Sort(comparison);
            return comparison;
        }
    }
}