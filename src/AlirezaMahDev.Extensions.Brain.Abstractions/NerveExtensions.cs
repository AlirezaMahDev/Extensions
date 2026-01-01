using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public Neuron<TData, TLink>? Find(ReadOnlyMemoryValue<TData> data)
        {
            if (nerve.Cache.NeuronSection.TryGet(in data.Value, out var neuronOffset))
                return new(neuronOffset.Value);

            var connection = nerve.Neuron.Wrap(nerve)
                .GetUnloadedConnections()
                .FirstOrDefault(x => x.Wrap(nerve).Neuron.Wrap(nerve).RefData.Equals(data.Value))
                .NullWhenDefault();
            if (!connection.HasValue)
                return null;

            return new(nerve.Cache.NeuronSection.Set(in data.Value, connection.Value.Wrap(nerve).RefValue.Neuron));
        }

        public Neuron<TData, TLink> FindOrAdd(ReadOnlyMemoryValue<TData> data)
        {
            return nerve.Neuron.Wrap(nerve)
                .Lock(neuronDataLocation =>
                {
                    if (nerve.Find(data) is { } neuron)
                        return neuron;

                    var neuronValue = nerve.Access
                        .Create(NeuronValue<TData>.Default with { Data = data.Value });

                    var neuronDataLocationWrap = neuronDataLocation.Wrap(nerve.Access);
                    var connectionValue = nerve.Access.Create(ConnectionValue<TLink>.Default with
                    {
                        Neuron = neuronValue.Offset,
                        Next = neuronDataLocationWrap.RefValue.Connection
                    });

                    neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                    return new(nerve.Cache.NeuronSection.Set(in data.Value, neuronValue.Offset));
                });
        }

        public async ValueTask<Neuron<TData, TLink>> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
            CancellationToken cancellationToken = default)
        {
            return await nerve.Neuron.Wrap(nerve)
                .LockAsync(neuronDataLocation =>
                    {
                        if (nerve.Find(data) is { } neuron)
                            return neuron;

                        var neuronValue =
                            nerve.Access.Create(NeuronValue<TData>.Default with { Data = data.Value });

                        var neuronDataLocationWrap = neuronDataLocation.Wrap(nerve.Access);
                        var connectionValue = nerve.Access
                            .Create(ConnectionValue<TLink>.Default with
                                {
                                    Neuron = neuronValue.Offset,
                                    Next = neuronDataLocationWrap.RefValue.Connection
                                });

                        neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                        return new(nerve.Cache.NeuronSection.Set(in data.Value, neuronValue.Offset));
                    },
                    cancellationToken);
        }
    }
}