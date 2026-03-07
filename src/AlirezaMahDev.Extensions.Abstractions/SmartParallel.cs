#pragma warning disable CA1068 

using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class SmartParallel
{
    private static readonly SemaphoreSlim Semaphore = new(Environment.ProcessorCount, Environment.ProcessorCount);

    private static readonly AsyncLocal<MemoryValue<int>> HeldSlots = new();

    public static async ValueTask ForEachAsync<T>(IAsyncEnumerable<T> values, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> body)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            await ValueTask.FromCanceled(cancellationToken);
            return;
        }
        await using var enumerator = values.GetAsyncEnumerator();
        var innerSemaphoreSlim = new SemaphoreSlim(1, 1);
        using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await ForAsync(0, Environment.ProcessorCount, cancellationTokenSource.Token, async (_, token) =>
        {
            while (!token.IsCancellationRequested)
            {
                await innerSemaphoreSlim.WaitAsync(token);
                if (token.IsCancellationRequested)
                {
                    await ValueTask.FromCanceled(token);
                    return;
                }
                if (!await enumerator.MoveNextAsync())
                {
                    cancellationTokenSource.Cancel();
                    return;
                }
                var item = enumerator.Current;
                innerSemaphoreSlim.Release();

                await body(item, token);
            }
        });
    }

    public static async ValueTask ForEachAsync<T>(IEnumerable<T> values, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> body)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            await ValueTask.FromCanceled(cancellationToken);
            return;
        }
        using var enumerator = values.GetEnumerator();
        var innerSemaphoreSlim = new SemaphoreSlim(1, 1);
        using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await ForAsync(0, Environment.ProcessorCount, cancellationTokenSource.Token, async (_, token) =>
        {
            while (!token.IsCancellationRequested)
            {
                await innerSemaphoreSlim.WaitAsync(token);
                if (token.IsCancellationRequested)
                {
                    await ValueTask.FromCanceled(token);
                    return;
                }
                if (!enumerator.MoveNext())
                {
                    cancellationTokenSource.Cancel();
                    return;
                }
                var item = enumerator.Current;
                innerSemaphoreSlim.Release();

                await body(item, token);
            }
        });
    }

    public static ValueTask InvokeAsync(CancellationToken cancellationToken, params Func<CancellationToken, ValueTask>[] func) =>
        ForAsync(0, func.Length, cancellationToken, (index, token) => func[index](token));

    public static ValueTask ForAsync(
        int fromInclusive,
        int toExclusive,
        CancellationToken cancellationToken,
        Func<int, CancellationToken, ValueTask> body)
    {

        if (fromInclusive >= toExclusive)
        {
            return ValueTask.CompletedTask;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled(cancellationToken);
        }

        int count = toExclusive - fromInclusive;
        int desired = Math.Min(count, Environment.ProcessorCount);

        int acquired = 0;
        try
        {
            for (int i = 0; i < desired; i++)
            {
                if (!Semaphore.Wait(0, cancellationToken))
                {
                    break;
                }

                acquired++;
            }
        }
        catch
        {
            if (acquired > 0)
            {
                Semaphore.Release(acquired);
            }

            throw;
        }

        if (!HeldSlots.Value.HasValue)
        {
            HeldSlots.Value = new(0);
        }

        int acquiredSlots = acquired + (Volatile.Read(ref HeldSlots.Value.Value) > 0 ? 1 : 0);

        if (acquiredSlots <= 1)
        {
            if (acquired > 0)
            {
                Semaphore.Release(acquired);
            }

            return ForAsyncRunSequentialAsync(fromInclusive, toExclusive, body, cancellationToken);
        }

        return ForAsyncRunParallelAsync(fromInclusive, toExclusive, acquired, acquiredSlots, body, cancellationToken);
    }

    private static async ValueTask ForAsyncRunSequentialAsync(
        int fromInclusive,
        int toExclusive,
        Func<int, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        for (int i = fromInclusive; i < toExclusive; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            await body(i, cancellationToken);
        }
    }

    private static async ValueTask ForAsyncRunParallelAsync(
        int fromInclusive,
        int toExclusive,
        int acquired,
        int acquiredSlots,
        Func<int, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        if (!HeldSlots.Value.HasValue)
        {
            HeldSlots.Value = new(0);
        }

        Interlocked.Add(ref HeldSlots.Value.Value, acquiredSlots);
        try
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var workers = MemoryList<Task>.Create(acquiredSlots);
            MemoryValue<int> nextIndex = fromInclusive;

            for (int w = 0; w < acquiredSlots; w++)
            {
                workers[w] = ForAsyncRunParallelAsyncWorkerAsync(nextIndex, toExclusive, body, cancellationToken);
            }

            Task whenAll = Task.WhenAll(workers);

            try
            {
                await whenAll.ConfigureAwait(false);
            }
            catch
            {
                await cancellationTokenSource.CancelAsync().ConfigureAwait(false);
                await whenAll.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

                var exceptions = workers
                    .Where(t => t.IsFaulted)
                    .Where(x => !x.IsCanceled)
                    .Select(t => t.Exception)
                    .Cast<Exception>()
                    .ToArray();

                if (exceptions.Length == 1)
                {
                    throw exceptions[0];
                }
                else
                {
                    throw new AggregateException(exceptions);
                }
            }
        }
        finally
        {
            Interlocked.Add(ref HeldSlots.Value.Value, -acquiredSlots);
            Semaphore.Release(acquired);
        }
    }

    private static async Task ForAsyncRunParallelAsyncWorkerAsync(
        MemoryValue<int> nextIndex,
        int toExclusive,
        Func<int, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            int index = Interlocked.Increment(ref nextIndex.Value) - 1;

            if (index >= toExclusive)
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await body(index, cancellationToken);
        }
    }
}