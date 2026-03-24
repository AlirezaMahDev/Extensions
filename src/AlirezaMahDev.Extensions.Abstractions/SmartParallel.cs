#pragma warning disable CA1068

using System.Threading.Channels;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class SmartParallel
{
    private static readonly SemaphoreSlim Semaphore = new(Environment.ProcessorCount, Environment.ProcessorCount);
    private static readonly AsyncLocal<MemoryValue<int>> AsyncLocalReservedCount = new();
    private readonly struct Work(CancellationToken cancellationToken, MemoryValue<int> index, int length, Func<int, CancellationToken, ValueTask> body)
    {
        public readonly CancellationToken CancellationToken = cancellationToken;
        public readonly MemoryValue<int> Index = index;
        public readonly int Length = length;
        public readonly Func<int, CancellationToken, ValueTask> Body = body;
    }


    private static readonly Channel<Work> Channel = System.Threading.Channels.Channel.CreateUnbounded<Work>();
    private static readonly Task[] Tasks = [.. Enumerable.Repeat(Task.Run(async () =>
    {
        await foreach (var work in Channel.Reader.ReadAllAsync())
        {
            while (!work.CancellationToken.IsCancellationRequested)
            {
                var index = Interlocked.Increment(ref work.Index.Value) - 1;
                if (index >= work.Length)
                {
                    break;
                }

                await work.Body(index, work.CancellationToken);
            }
        }
    }), Environment.ProcessorCount)];

    private static ref int CurrentCountReserved
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (!AsyncLocalReservedCount.Value.HasValue)
            {
                AsyncLocalReservedCount.Value = new(0);
            }

            return ref AsyncLocalReservedCount.Value.Value;
        }
    }

    private static bool CurrentHasReserve
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var value = AsyncLocalReservedCount.Value;
            return value.HasValue && value.Value > 0;
        }
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
                    try
                    {
                        state.semaphoreSlim.Wait(token);
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
    public static void InvokeAsync(CancellationToken cancellationToken,
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
    public static ValueTask InvokeAsyncCore<TState>(int count,
        TState state,
        Func<int, TState, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        return cancellationToken.IsCancellationRequested || count == 0
            ? ValueTask.CompletedTask
            : CurrentHasReserve
                ? InvokeAsyncCoreNeedReserve(count, state, body, cancellationToken)
                : InvokeAsyncCoreHasReserve(count, state, body, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static void InvokeCore<TState>(int count,
        TState state,
        Action<int, TState, CancellationToken> body,
        CancellationToken cancellationToken)
    {
        if (Volatile.Read(ref CurrentCountReserved) == 0)
        {
            if (!cancellationToken.IsCancellationRequested && count != 0)
            {
                InvokeCoreNeedReserve(count, state, body, cancellationToken);
            }
        }
        else
        {
            if (!cancellationToken.IsCancellationRequested && count != 0)
            {
                InvokeCoreHasReserve(count, state, body, cancellationToken);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static async ValueTask InvokeAsyncCoreNeedReserve<TState>(int count,
        TState state,
        Func<int, TState, CancellationToken, ValueTask> body,
        CancellationToken cancellationToken)
    {
        await Semaphore.WaitAsync(cancellationToken);
        Interlocked.Increment(ref CurrentCountReserved);
        try
        {
            await InvokeAsyncCoreHasReserve(count, state, body, cancellationToken);
        }
        finally
        {
            Interlocked.Decrement(ref CurrentCountReserved);
            Semaphore.Release();
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void InvokeCoreNeedReserve<TState>(int count,
        TState state,
        Action<int, TState, CancellationToken> body,
        CancellationToken cancellationToken)
    {
        Semaphore.Wait(cancellationToken);
        Interlocked.Increment(ref CurrentCountReserved);
        try
        {
            InvokeCoreHasReserve(count, state, body, cancellationToken);
        }
        finally
        {
            Interlocked.Decrement(ref CurrentCountReserved);
            Semaphore.Release();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static async ValueTask InvokeAsyncCoreHasReserve<TState>(int count,
    TState state,
    Func<int, TState, CancellationToken, ValueTask> body,
    CancellationToken cancellationToken)
    {
        var reserveCount = 1;
        var index = 0;

        MemoryList<Task>? tasks = null;
        CancellationTokenSource? cancellationTokenSource = null;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var maxWorkerCountNeed = count - Volatile.Read(ref index) - Volatile.Read(ref reserveCount);
                if (maxWorkerCountNeed > 0)
                {
                    for (var i = 0; i < maxWorkerCountNeed; i++)
                    {
                        if (!Semaphore.Wait(0, CancellationToken.None))
                        {
                            break;
                        }

                        cancellationTokenSource ??= CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        tasks ??= [with(Semaphore.CurrentCount + 1)];

                        Interlocked.Increment(ref reserveCount);
                        tasks.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        var funcIndex = Interlocked.Increment(ref index) - 1;
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

                var funcIndex = Interlocked.Increment(ref index) - 1;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void InvokeCoreHasReserve<TState>(int count,
    TState state,
    Action<int, TState, CancellationToken> body,
    CancellationToken cancellationToken)
    {
        var reserveCount = 1;
        var index = 0;

        MemoryList<Task>? tasks = null;
        CancellationTokenSource? cancellationTokenSource = null;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var maxWorkerCountNeed = count - Volatile.Read(ref index) - Volatile.Read(ref reserveCount);
                if (maxWorkerCountNeed > 0)
                {
                    for (var i = 0; i < maxWorkerCountNeed; i++)
                    {
                        if (!Semaphore.Wait(0, CancellationToken.None))
                        {
                            break;
                        }

                        cancellationTokenSource ??= CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        tasks ??= [];

                        Interlocked.Increment(ref reserveCount);
                        tasks.Add(Task.Run(() =>
                            {
                                try
                                {
                                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                                    {
                                        var funcIndex = Interlocked.Increment(ref index) - 1;
                                        if (funcIndex >= count)
                                        {
                                            break;
                                        }

                                        body(funcIndex, state, cancellationTokenSource.Token);
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

                var funcIndex = Interlocked.Increment(ref index) - 1;
                if (funcIndex >= count)
                {
                    break;
                }

                body(funcIndex, state, cancellationToken);
            }

            if (tasks is not null && cancellationTokenSource is not null)
            {
                Task.WhenAll(tasks.Memory.Span).GetAwaiter().GetResult();
            }
        }
        catch
        {
            if (tasks is not null && cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                Task.WhenAll(tasks.Memory.Span).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing).GetAwaiter().GetResult();
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