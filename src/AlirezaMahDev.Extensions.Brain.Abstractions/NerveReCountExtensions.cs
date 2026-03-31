namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveReCountExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public void ReCount(IProgressLogger progressLogger)
        {
            nerve.Counter.UnsafeRefValue.NeuronCount = 0;
            using (var wraps =
                   nerve.RootNeuronWrap.GetConnectionsWrapRaw().ToMemoryList())
            {
                if (wraps.Count == 0)
                {
                    return;
                }

                SmartParallel.For(0,
                    wraps.Count,
                    CancellationToken.None,
                    (index, cancellationToken) =>
                    {
                        wraps[index].Location.WriteLock((scoped ref value) =>
                            value.NextCount = wraps.Count - index - 1, cancellationToken);
                        progressLogger.IncrementCount();
                    });
                Interlocked.Add(ref nerve.Counter.UnsafeRefValue.NeuronCount, wraps.Count);
            }

            nerve.Counter.UnsafeRefValue.ConnectionCount = 0;
            var connectionWrap = nerve.RootConnectionWrap;
            INerve<TData, TLink>.ReCountCore(progressLogger, connectionWrap);
        }

        public static void ReCountCore(IProgressLogger progressLogger,
            CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            using (var wraps =
                   connectionWrap.GetConnectionsWrapRaw().ToMemoryList())
            {
                if (wraps.Count == 0)
                {
                    return;
                }

                SmartParallel.For(0,
                    wraps.Count,
                    CancellationToken.None,
                    (index, cancellationToken) =>
                    {
                        wraps[index].Location.WriteLock((scoped ref value) =>
                            value.NextCount = wraps.Count - index - 1, cancellationToken);
                        progressLogger.IncrementCount();
                    });
                Interlocked.Add(ref connectionWrap.Nerve.Counter.UnsafeRefValue.ConnectionCount, wraps.Count);
            }

            SmartParallel.ForEach(connectionWrap.GetConnectionsWrapRaw(),
                CancellationToken.None,
                (item, _) =>
                    INerve<TData, TLink>.ReCountCore(progressLogger, item));
        }
    }
}