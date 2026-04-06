#pragma warning disable CA1068

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class SmartParallel
{
    private sealed class Work(
        CancellationToken cancellationToken,
        int length,
        bool async,
        Func<int, CancellationToken, ValueTask>? bodyAsync,
        Action<int, CancellationToken>? bodySync)
    {
        public readonly CancellationToken CancellationToken = cancellationToken;

        public int NextIndex;
        public int ActiveWorkers;

        public readonly int Length = length;
        public readonly bool Async = async;
        public readonly Func<int, CancellationToken, ValueTask>? BodyAsync = bodyAsync;
        public readonly Action<int, CancellationToken>? BodySync = bodySync;

        public readonly CancellationTokenSource CancellationTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        public readonly ConcurrentBag<Exception> Exceptions = [];
        public readonly TaskCompletionSource TaskCompletionSource = new();
    }

    private static readonly AsyncLocal<bool> IsWorker = new();

    private static readonly Channel<Work> WorkChannel =
        Channel.CreateUnbounded<Work>();

    private static readonly Task[] Tasks =
    [
        .. Enumerable.Range(0, Environment.ProcessorCount).Select(_ => Task.Run(Worker))
    ];

    private static async Task Worker()
    {
        IsWorker.Value = true;
        await foreach (var work in WorkChannel.Reader.ReadAllAsync())
        {
            try
            {
                if (work.Async)
                {
                    while (!work.CancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var index = Interlocked.Increment(ref work.NextIndex) - 1;
                        if (index >= work.Length) break;

                        try
                        {
                            await work.BodyAsync!(index, work.CancellationTokenSource.Token);
                        }
                        catch (OperationCanceledException operationCanceledException)
                        {
                            if (operationCanceledException.CancellationToken != work.CancellationTokenSource.Token &&
                                operationCanceledException.CancellationToken != work.CancellationToken)
                            {
                                work.Exceptions.Add(operationCanceledException);
                                work.CancellationTokenSource.Cancel();
                            }

                            break;
                        }
                        catch (Exception e)
                        {
                            work.Exceptions.Add(e);
                            work.CancellationTokenSource.Cancel();
                            break;
                        }
                    }
                }
                else
                {
                    while (!work.CancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var index = Interlocked.Increment(ref work.NextIndex) - 1;
                        if (index >= work.Length) break;

                        try
                        {
                            work.BodySync!(index, work.CancellationTokenSource.Token);
                        }
                        catch (OperationCanceledException operationCanceledException)
                        {
                            if (operationCanceledException.CancellationToken != work.CancellationTokenSource.Token &&
                                operationCanceledException.CancellationToken != work.CancellationToken)
                            {
                                work.Exceptions.Add(operationCanceledException);
                                work.CancellationTokenSource.Cancel();
                            }

                            break;
                        }
                        catch (Exception e)
                        {
                            work.Exceptions.Add(e);
                            work.CancellationTokenSource.Cancel();
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (Interlocked.Decrement(ref work.ActiveWorkers) == 0)
                {
                    try
                    {
                        if (!work.Exceptions.IsEmpty)
                        {
                            work.TaskCompletionSource.TrySetException(
                                new AggregateException(work.Exceptions)
                                    .Flatten());
                        }
                        else if (work.CancellationToken.IsCancellationRequested)
                        {
                            work.TaskCompletionSource.TrySetCanceled(work.CancellationToken);
                        }
                        else
                        {
                            work.TaskCompletionSource.TrySetResult();
                        }
                    }
                    finally
                    {
                        work.CancellationTokenSource.Dispose();
                    }
                }
            }
        }

        IsWorker.Value = false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static async ValueTask ForEachAsync<T>(IAsyncEnumerable<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await using var asyncEnumerator = values.GetAsyncEnumerator(cancellationToken);
        using SemaphoreSlim semaphoreSlim = new(1, 1);
        await InvokeAsyncCore(Environment.ProcessorCount,
            (asyncEnumerator, semaphoreSlim, body),
            static async (_, state, token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    T current;
                    await state.semaphoreSlim.WaitAsync(token);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static async ValueTask ForEachAsync<T>(IEnumerable<T> values,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, ValueTask> body)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        using var asyncEnumerator = values.GetEnumerator();
        using SemaphoreSlim semaphoreSlim = new(1, 1);
        await InvokeAsyncCore(Environment.ProcessorCount,
            (asyncEnumerator, semaphoreSlim, body),
            static async (_, state, token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    T current;
                    await state.semaphoreSlim.WaitAsync(token);

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


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void ForEach<T>(IEnumerable<T> values,
        CancellationToken cancellationToken,
        Action<T, CancellationToken> body)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        using var asyncEnumerator = values.GetEnumerator();
        using SemaphoreSlim semaphoreSlim = new(1, 1);
        InvokeCore(Environment.ProcessorCount,
            (asyncEnumerator, semaphoreSlim, body),
            static (_, state, token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    T current;
                    state.semaphoreSlim.Wait(token);

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

                    state.body(current, token);
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
    public static void ForEach<T>(T[] values,
        CancellationToken cancellationToken,
        Action<T, CancellationToken> body)
    {
        InvokeCore(values.Length,
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
    public static void ForEach<T>(IReadonlyMemoryList<T> values,
        CancellationToken cancellationToken,
        Action<T, CancellationToken> body)
    {
        InvokeCore(values.Count,
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
    public static void ForEach<T>(Memory<T> values,
        CancellationToken cancellationToken,
        Action<T, CancellationToken> body)
    {
        InvokeCore(values.Length,
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
    public static void ForEach<T>(ReadOnlyMemory<T> values,
        CancellationToken cancellationToken,
        Action<T, CancellationToken> body)
    {
        InvokeCore(values.Length,
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
    public static void ForEach<T>(IReadOnlyList<T> values,
        CancellationToken cancellationToken,
        Action<T, CancellationToken> body)
    {
        InvokeCore(values.Count,
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
    public static void For(int fromInclusive,
        int toExclusive,
        CancellationToken cancellationToken,
        Action<int, CancellationToken> body)
    {
        InvokeCore(Math.Max(0, toExclusive - fromInclusive),
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
    public static void Invoke(CancellationToken cancellationToken,
        params Action<CancellationToken>[] actions)
    {
        InvokeCore(actions.Length,
            actions,
            [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
            static (index, state, token) =>
                state[index](token),
            cancellationToken);
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static async ValueTask InvokeAsyncCore<TState>(int count,
        TState state,
        Func<int, TState, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        if (count <= 0 || cancellationToken.IsCancellationRequested) return;

        if (IsWorker.Value)
        {
            for (var index = 0; index < count; index++)
            {
                await body(index, state, cancellationToken);
            }

            return;
        }

        Work work = new(cancellationToken,
            count,
            true,
            (index, token) => body(index, state, token),
            null);

        var workerCount = Math.Min(count, Tasks.Length);

        work.ActiveWorkers = workerCount;
        for (var i = 0; i < workerCount; i++)
            WorkChannel.Writer.TryWrite(work);

        await work.TaskCompletionSource.Task;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static void InvokeCore<TState>(int count,
        TState state,
        Action<int, TState, CancellationToken> body,
        CancellationToken cancellationToken)
    {
        if (count <= 0 || cancellationToken.IsCancellationRequested) return;

        if (IsWorker.Value)
        {
            for (var index = 0; index < count; index++)
                body(index, state, cancellationToken);
            return;
        }

        Work work = new(cancellationToken,
            count,
            false,
            null,
            (index, token) => body(index, state, token));

        var workerCount = Math.Min(count, Tasks.Length);
        work.ActiveWorkers = workerCount;
        for (var i = 0; i < workerCount; i++)
            WorkChannel.Writer.TryWrite(work);

        work.TaskCompletionSource.Task.GetAwaiter().GetResult();
    }
}