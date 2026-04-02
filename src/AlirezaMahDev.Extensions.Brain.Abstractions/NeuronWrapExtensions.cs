namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronWrapExtensions
{
    extension<TData, TLink>(in CellWrap<NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Optional<CellWrap<NeuronValue<TData>, TData, TLink>> NextWrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Location.ReadLock((scoped ref readonly x, scoped ref readonly nerve) =>
                x.Next.Offset.IsNull
                    ? Optional<CellWrap<NeuronValue<TData>, TData, TLink>>.Null
                    : x.Next.NewWrap(nerve),
                    wrap.Nerve);
        }

        public DataOffset? NextUnloadItem
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => wrap.Nerve.NextUnloadItem.GetOrNull(in wrap.Location.Offset);
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            set
            {
                var dataOffset = value ?? DataOffset.Null;
                wrap.Nerve.NextUnloadItem.Set(in wrap.Location.Offset, in dataOffset);
            }
        }
    }

    extension<TData, TLink>(CellWrap<NeuronValue<TData>, TData, TLink> wrap)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<NeuronValue<TData>, TData, TLink>> GetNeuronsWrapRaw()
        {
            return wrap.NextWrap is { HasValue: true } childWrap
                ? CellWrap<NeuronValue<TData>, TData, TLink>.GetNeuronsWrapCore(childWrap.Value)
                : [];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static IEnumerable<CellWrap<NeuronValue<TData>, TData, TLink>> GetNeuronsWrapCore(
            CellWrap<NeuronValue<TData>, TData, TLink> nextWrap)
        {
            Optional<CellWrap<NeuronValue<TData>, TData, TLink>> current = nextWrap;
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value.NextWrap;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<CellWrap<NeuronValue<TData>, TData, TLink>> GetUnloadedNeuronsWrap()
        {
            var unloaded = wrap.NextUnloadItem;
            if (unloaded?.IsNull == true)
            {
                yield break;
            }

            var lastNeuronWrap = unloaded.HasValue
                ? new Neuron(unloaded.Value).NewWrap(wrap.Nerve) : wrap.NextWrap;
            while (lastNeuronWrap.HasValue)
            {
                var neuronWrap = lastNeuronWrap.Value;
                wrap.Nerve.TrySetNeuronCache(in neuronWrap);
                wrap.NextUnloadItem = neuronWrap.Location
                    .ReadLock((scoped ref readonly x) => x.Next.Offset);
                lastNeuronWrap = neuronWrap.NextWrap;
                yield return neuronWrap;
            }
        }
    }
}