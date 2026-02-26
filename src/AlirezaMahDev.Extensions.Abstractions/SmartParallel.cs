namespace AlirezaMahDev.Extensions.Abstractions;

public static class SmartParallel
{
    private static readonly SemaphoreSlim Semaphore = new(Environment.ProcessorCount, Environment.ProcessorCount);

    private static readonly AsyncLocal<MemoryValue<int>> HeldSlots = new();

    public static ValueTask ForAsync(
        int fromInclusive,
        int toExclusive,
        CancellationToken cancellationToken,
        Func<int, CancellationToken, ValueTask> body)
    {
        if (fromInclusive >= toExclusive)
            return ValueTask.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();

        int count = toExclusive - fromInclusive;
        int desired = Math.Min(count, Environment.ProcessorCount);

        int acquired = 0;
        try
        {
            for (int i = 0; i < desired; i++)
            {
                if (!Semaphore.Wait(0, cancellationToken))
                    break;

                acquired++;
            }
        }
        catch
        {
            if (acquired > 0)
                Semaphore.Release(acquired);

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
                Semaphore.Release(acquired);
            return RunSequentialAsync(fromInclusive, toExclusive, body, cancellationToken);
        }

        return RunParallelAsync(fromInclusive, toExclusive, acquired, acquiredSlots, body, cancellationToken);
    }

    private static async ValueTask RunSequentialAsync(
        int fromInclusive,
        int toExclusive,
        Func<int, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        for (int i = fromInclusive; i < toExclusive; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await body(i, cancellationToken);
        }
    }

    private static async ValueTask RunParallelAsync(
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
                workers[w] = RunParallelWorkerAsync(nextIndex, toExclusive, body, cancellationToken);
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

                throw new AggregateException(
                    workers
                        .Where(t => t.IsFaulted)
                        .Select(t => t.Exception)
                        .Cast<Exception>()
                );
            }
        }
        finally
        {
            Interlocked.Add(ref HeldSlots.Value.Value, -acquiredSlots);
            Semaphore.Release(acquired);
        }
    }

    private static async Task RunParallelWorkerAsync(
        MemoryValue<int> nextIndex,
        int toExclusive,
        Func<int, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            int index = Interlocked.Increment(ref nextIndex.Value) - 1;

            if (index >= toExclusive)
                return;

            cancellationToken.ThrowIfCancellationRequested();

            await body(index, cancellationToken);
        }
    }
}