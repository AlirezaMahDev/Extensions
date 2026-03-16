#pragma warning disable CA1068

using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class SmartParallel
{
    private static readonly SemaphoreSlim Semaphore = new(Environment.ProcessorCount, Environment.ProcessorCount);
    private static readonly AsyncLocal<MemoryValue<int>> AsyncLocalReservedCount = new();

    private static ref int CurrentReservedCount
    {
        get
        {
            if (!AsyncLocalReservedCount.Value.HasValue)
            {
                AsyncLocalReservedCount.Value = new(0);
            }

            return ref AsyncLocalReservedCount.Value.Value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static async ValueTask ForEachAsync<T>(IAsyncEnumerable<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await using IAsyncEnumerator<T> asyncEnumerator = values.GetAsyncEnumerator(cancellationToken);
        using SemaphoreSlim semaphoreSlim = new(1, 1);
        await InvokeAsyncCore(Environment.ProcessorCount,
            (asyncEnumerator, semaphoreSlim, body),
            static async (_, state, token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    T current;
                    try
                    {
                        await state.semaphoreSlim.WaitAsync(token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }

                    try
                    {
                        if (!await state.asyncEnumerator.MoveNextAsync())
                        {
                            return;
                        }

                        current = state.asyncEnumerator.Current;
                    }
                    finally
                    {
                        state.semaphoreSlim.Release();
                    }

                    await state.body(current, token);
                }
            },
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static async ValueTask ForEachAsync<T>(IEnumerable<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        using IEnumerator<T> asyncEnumerator = values.GetEnumerator();
        using SemaphoreSlim semaphoreSlim = new(1, 1);
        await InvokeAsyncCore(Environment.ProcessorCount,
            (asyncEnumerator, semaphoreSlim, body),
            static async (_, state, token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    T current;
                    try
                    {
                        await state.semaphoreSlim.WaitAsync(token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }

                    try
                    {
                        if (!state.asyncEnumerator.MoveNext())
                        {
                            return;
                        }

                        current = state.asyncEnumerator.Current;
                    }
                    finally
                    {
                        state.semaphoreSlim.Release();
                    }

                    await state.body(current, token);
                }
            },
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask ForEachAsync<T>(T[] values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        return InvokeAsyncCore(values.Length,
            (values, body),
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state.body(state.values[index], token),
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask ForEachAsync<T>(IReadonlyMemoryList<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        return InvokeAsyncCore(values.Count,
            (values, body),
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state.body(state.values[index], token),
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask ForEachAsync<T>(Memory<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        return InvokeAsyncCore(values.Length,
            (values, body),
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state.body(state.values.Span[index], token),
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask ForEachAsync<T>(ReadOnlyMemory<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        return InvokeAsyncCore(values.Length,
            (values, body),
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state.body(state.values.Span[index], token),
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask ForEachAsync<T>(IReadOnlyList<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        return InvokeAsyncCore(values.Count,
            (values, body),
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state.body(state.values[index], token),
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask ForAsync(int fromInclusive,
        int toExclusive,
        CancellationToken cancellationToken,
        Func<int, CancellationToken, ValueTask> body)
    {
        return InvokeAsyncCore(Math.Max(0, toExclusive - fromInclusive),
            (fromInclusive, body),
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state.body(state.fromInclusive + index, token),
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask InvokeAsync(CancellationToken cancellationToken,
        params Func<CancellationToken, ValueTask>[] actions)
    {
        return InvokeAsyncCore(actions.Length,
            actions,
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state[index](token),
            cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ValueTask InvokeAsyncCore<TState>(int count,
        TState state,
        Func<int, TState, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        return cancellationToken.IsCancellationRequested || count == 0
            ? ValueTask.CompletedTask
            : Volatile.Read(ref CurrentReservedCount) == 0
                ? InvokeAsyncCoreNeedReserve(count, state, body, cancellationToken)
                : InvokeAsyncCoreHasReserve(count, state, body, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    private static async ValueTask InvokeAsyncCoreNeedReserve<TState>(int count,
        TState state,
        Func<int, TState, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        await Semaphore.WaitAsync(cancellationToken);
        Interlocked.Increment(ref CurrentReservedCount);
        try
        {
            await InvokeAsyncCoreHasReserve(count, state, body, cancellationToken);
        }
        finally
        {
            Interlocked.Decrement(ref CurrentReservedCount);
            Semaphore.Release();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    private static async ValueTask InvokeAsyncCoreHasReserve<TState>(int count,
        TState state,
        Func<int, TState, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        int reserveCount = 1;
        int index = 0;

        MemoryList<Task>? tasks = null;
        CancellationTokenSource? cancellationTokenSource = null;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int maxWorkerCountNeed = count - Volatile.Read(ref index) - Volatile.Read(ref reserveCount);
                if (maxWorkerCountNeed > 0)
                {
                    for (int i = 0; i < maxWorkerCountNeed; i++)
                    {
                        if (!Semaphore.Wait(0, CancellationToken.None))
                        {
                            break;
                        }

                        cancellationTokenSource ??= CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        tasks ??= [];

                        Interlocked.Increment(ref reserveCount);
                        tasks.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        int funcIndex = Interlocked.Increment(ref index) - 1;
                                        if (funcIndex >= count)
                                        {
                                            break;
                                        }

                                        await body(funcIndex, state, cancellationTokenSource.Token);
                                    }
                                }
                                finally
                                {
                                    Interlocked.Decrement(ref reserveCount);
                                    Semaphore.Release();
                                }
                            },
                            CancellationToken.None));
                    }
                }

                int funcIndex = Interlocked.Increment(ref index) - 1;
                if (funcIndex >= count)
                {
                    break;
                }

                await body(funcIndex, state, cancellationToken);
            }

            if (tasks is not null && cancellationTokenSource is not null)
            {
                await Task.WhenAll(tasks.Memory.Span);
            }
        }
        catch
        {
            if (tasks is not null && cancellationTokenSource is not null)
            {
                await cancellationTokenSource.CancelAsync();
                await Task.WhenAll(tasks.Memory.Span).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }

            throw;
        }
        finally
        {
            cancellationTokenSource?.Dispose();
            tasks?.Dispose();
        }
    }
}