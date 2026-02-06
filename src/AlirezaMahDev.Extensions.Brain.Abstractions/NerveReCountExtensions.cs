using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.Progress.Abstractions;

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
            using (var wraps = nerve.NeuronWrap.GetConnectionsWrapRaw().ToMemoryList())
            {
                if (wraps.Count == 0)
                    return;
                Parallel.For(0, wraps.Count, (index) =>
                   {
                       wraps[index].Lock(location =>
                           location.RefValue.NextCount = wraps.Count - index - 1);
                       progressLogger.IncrementCount();
                   });
                Interlocked.Add(ref nerve.Counter.RefValue.NeuronCount, wraps.Count);
            }

            nerve.Counter.RefValue.ConnectionCount = 0;
            var connectionWrap = nerve.ConnectionWrap;
            INerve<TData, TLink>.ReCountCore(progressLogger, connectionWrap);

        }

        public static void ReCountCore(IProgressLogger progressLogger, CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            using (var wraps = connectionWrap.GetConnectionsWrapRaw().ToMemoryList())
            {
                if (wraps.Count == 0)
                    return;
                Parallel.For(0, wraps.Count, (index) =>
                   {
                       wraps[index].Lock(location =>
                           location.RefValue.NextCount = wraps.Count - index - 1);
                       progressLogger.IncrementCount();
                   });
                Interlocked.Add(ref connectionWrap.Nerve.Counter.RefValue.ConnectionCount, wraps.Count);
            }

            Parallel.ForEach(connectionWrap.GetConnectionsWrapRaw(), (item) =>
                    INerve<TData, TLink>.ReCountCore(progressLogger, item));
        }

        public async Task ReCountAsync(IProgressLogger progressLogger, CancellationToken cancellationToken = default)
        {
            nerve.Counter.RefValue.NeuronCount = 0;
            using (var wraps = nerve.NeuronWrap.GetConnectionsWrapRaw().ToMemoryList())
            {
                if (wraps.Count == 0)
                    return;
                await Parallel.ForAsync(0, wraps.Count,
                    cancellationToken,
                    async (index, token) =>
                   {
                       await wraps[index].LockAsync(location =>
                           location.RefValue.NextCount = wraps.Count - index - 1, token);
                       progressLogger.IncrementCount();
                   });
                Interlocked.Add(ref nerve.Counter.RefValue.NeuronCount, wraps.Count);
            }

            nerve.Counter.RefValue.ConnectionCount = 0;
            var connectionWrap = nerve.ConnectionWrap;
            await INerve<TData, TLink>.ReCountAsyncCore(progressLogger, connectionWrap, cancellationToken);
        }

        private static async Task ReCountAsyncCore(IProgressLogger progressLogger, CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap, CancellationToken cancellationToken)
        {
            using (var wraps = connectionWrap.GetConnectionsWrapRaw().ToMemoryList())
            {
                if (wraps.Count == 0)
                    return;
                await Parallel.ForAsync(0, wraps.Count, cancellationToken,
                    async (index, token) =>
                    {
                        await wraps[index].LockAsync(location =>
                            location.RefValue.NextCount = wraps.Count - index - 1, token);
                        progressLogger.IncrementCount();
                    });
                Interlocked.Add(ref connectionWrap.Nerve.Counter.RefValue.ConnectionCount, wraps.Count);
            }

            await Parallel.ForEachAsync(connectionWrap.GetConnectionsWrapRaw(), cancellationToken,
                async (item, token) =>
                    await INerve<TData, TLink>.ReCountAsyncCore(progressLogger, item, token));
        }
    }
}
