#pragma warning disable CA1068

using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class SmartParallel
{
    private readonly struct Work(
        CancellationToken cancellationToken,
        MemoryValue<int> index,
        int length,
        bool async,
        Func<int, CancellationToken, ValueTask>? bodyAsync,
        Action<int, CancellationToken>? bodySync)
    {
        public readonly CancellationToken CancellationToken = cancellationToken;
        public readonly MemoryValue<int> Index = index;
        public readonly int Length = length;
        public readonly bool Async = async;
        public readonly Func<int, CancellationToken, ValueTask>? BodyAsync = bodyAsync;
        public readonly Action<int, CancellationToken>? BodySync = bodySync;
        public TaskCompletionSource TaskCompletionSource { get; } = new();
    }

    private static readonly AsyncLocal<bool> IsWorker = new();
    private static readonly Channel<Work> Channel = System.Threading.Channels.Channel.CreateUnbounded<Work>();
    private static readonly Task[] Tasks = [.. Enumerable.Repeat(Task.Run(Worker), Environment.ProcessorCount)];

    private static async Task Worker()
    {
        IsWorker.Value = true;
        await foreach (var work in Channel.Reader.ReadAllAsync())
        {
            if (work.Async)
            {
                while (!work.CancellationToken.IsCancellationRequested)
                {
                    var index = Interlocked.Increment(ref work.Index.Value) - 1;
                    if (index >= work.Length)
                    {
                        work.TaskCompletionSource.TrySetResult();
                        break;
                    }

                    try
                    {
                        await work.BodyAsync!(index, work.CancellationToken);
                    }
                    catch (Exception e)
                    {
                        work.TaskCompletionSource.TrySetException(e);
                    }
                }

                if (work.CancellationToken.IsCancellationRequested)
                {
                    work.TaskCompletionSource.TrySetCanceled(work.CancellationToken);
                }
            }
            else
            {
                while (!work.CancellationToken.IsCancellationRequested)
                {
                    var index = Interlocked.Increment(ref work.Index.Value) - 1;
                    if (index >= work.Length)
                    {
                        work.TaskCompletionSource.TrySetResult();
                        break;
                    }

                    try
                    {
                        work.BodySync!(index, work.CancellationToken);
                    }
                    catch (Exception e)
                    {
                        work.TaskCompletionSource.TrySetException(e);
                    }
                }

                if (work.CancellationToken.IsCancellationRequested)
                {
                    work.TaskCompletionSource.TrySetCanceled(work.CancellationToken);
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
        if (IsWorker.Value)
        {
            for (var index = 0; index < count; index++)
            {
                await body(index, state, cancellationToken);
            }

            return;
        }

        Work work = new(
            cancellationToken,
            0,
            count,
            true,
            (index, token) => body(index, state, token),
            null
        );
        for (var i = 0; i < Tasks.Length; i++)
        {
            if (work.TaskCompletionSource.Task.IsCompleted)
            {
                break;
            }

            await Channel.Writer.WriteAsync(work, cancellationToken);
        }

        await work.TaskCompletionSource.Task;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static void InvokeCore<TState>(int count,
        TState state,
        Action<int, TState, CancellationToken> body,
        CancellationToken cancellationToken)
    {
        if (IsWorker.Value)
        {
            for (var index = 0; index < count; index++)
            {
                body(index, state, cancellationToken);
            }

            return;
        }

        Work work = new(
            cancellationToken,
            0,
            count,
            false,
            null,
            (index, token) => body(index, state, token)
        );
        for (var i = 0; i < Tasks.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (work.TaskCompletionSource.Task.IsCompleted)
            {
                break;
            }

            while (!Channel.Writer.TryWrite(work))
            {
            }
        }

        work.TaskCompletionSource.Task.GetAwaiter().GetResult();
    }
}