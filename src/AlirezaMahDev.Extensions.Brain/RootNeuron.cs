using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class RootNeuron<TData, TLink>(Neuron<TData, TLink> neuron)
    : Neuron<TData, TLink>(new(neuron._nerve, neuron.Location)), IRootNeuron<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public INeuron<TData, TLink>? Find(ReadOnlyMemoryValue<TData> data)
    {
        if (_nerve.Cache.TryGet<TData, INeuron<TData, TLink>>(in data.Value, out var neuron))
            return neuron;

        var connection = this.FirstOrDefault(x => x.GetNeuron().RefData.Equals(data.Value));
        if (connection is null)
            return null;

        return _nerve.Cache.Set(in data.Value, connection.GetNeuron());
    }

    public async ValueTask<INeuron<TData, TLink>?> FindAsync(ReadOnlyMemoryValue<TData> data,
        CancellationToken cancellationToken = default)
    {
        if (_nerve.Cache.TryGet<TData, INeuron<TData, TLink>>(in data.Value, out var neuron))
            return neuron;

        var connection = await this.FirstOrDefaultAsync(async (x, token) =>
                (await x.GetNeuronAsync(token)).RefData.Equals(data.Value),
            cancellationToken);
        if (connection is null)
            return null;

        return _nerve.Cache.Set(data.Value, await connection.GetNeuronAsync(cancellationToken));
    }

    public INeuron<TData, TLink> FindOrAdd(ReadOnlyMemoryValue<TData> data)
    {
        return Lock(neuronDataLocation =>
        {
            if (Find(data) is { } neuron)
                return neuron;

            var neuronValue = _nerve.Location.Access.Create(NeuronValue<TData>.Default with { Data = data.Value });
            neuron = _nerve.NeuronFactory.GetOrCreate(neuronValue.Offset);

            var connectionValue = _nerve.Location.Access
                .Create(ConnectionValue<TLink>.Default with
                {
                    Neuron = neuron.Offset,
                    Next = neuronDataLocation.RefValue.Connection
                });

            neuronDataLocation.RefValue.Connection = connectionValue.Offset;

            return _nerve.Cache.Set(in data.Value, neuron);
        });
    }

    public async ValueTask<INeuron<TData, TLink>> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
        CancellationToken cancellationToken = default)
    {
        return await LockAsync(async (neuronDataLocation, token) =>
            {
                if (await FindAsync(data, token) is { } neuron)
                    return neuron;

                var neuronValue =
                    await _nerve.Location.Access.CreateAsync(NeuronValue<TData>.Default with { Data = data.Value },
                        token);
                neuron = await _nerve.NeuronFactory.GetOrCreateAsync(neuronValue.Offset, token);

                var connectionValue = await _nerve.Location.Access
                    .CreateAsync(ConnectionValue<TLink>.Default with
                        {
                            Neuron = neuron.Offset,
                            Next = neuronDataLocation.RefValue.Connection
                        },
                        token);

                neuronDataLocation.RefValue.Connection = connectionValue.Offset;

                return _nerve.Cache.Set(in data.Value, neuron);
            },
            cancellationToken);
    }
}