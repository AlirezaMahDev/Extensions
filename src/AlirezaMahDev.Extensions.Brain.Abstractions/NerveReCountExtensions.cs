namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveReCountExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public void ReCount(IProgressLogger progressLogger)
        {
            nerve.Counter.RefValue.NeuronCount = 0;
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
                    (index, _) =>
                    {
                        wraps[index]
                            .Lock((ref readonly location) =>
                                location.RefValue.NextCount = wraps.Count - index - 1);
                        progressLogger.IncrementCount();
                    });
                Interlocked.Add(ref nerve.Counter.RefValue.NeuronCount, wraps.Count);
            }

            nerve.Counter.RefValue.ConnectionCount = 0;
            var connectionWrap = nerve.RootConnectionWrap;
            INerve<TData, TLink>.ReCountCore(progressLogger, connectionWrap);
        }

        public static void ReCountCore(IProgressLogger progressLogger,
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
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
                    (index, _) =>
                    {
                        wraps[index]
                            .Lock((ref readonly location) =>
                                location.RefValue.NextCount = wraps.Count - index - 1);
                        progressLogger.IncrementCount();
                    });
                Interlocked.Add(ref connectionWrap.Nerve.Counter.RefValue.ConnectionCount, wraps.Count);
            }

            SmartParallel.ForEach(connectionWrap.GetConnectionsWrapRaw(),
                CancellationToken.None,
                (item, _) =>
                    INerve<TData, TLink>.ReCountCore(progressLogger, item));
        }
    }
}