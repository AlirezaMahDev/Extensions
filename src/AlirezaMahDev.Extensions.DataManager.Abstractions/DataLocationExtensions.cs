using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationExtensions
{
    extension<T>(T locationBase)
        where T : IDataLocationBase<T>
    {
        public Memory<byte> GetMemory(IDataAccess access) =>
            access.ReadMemory(locationBase.Offset);
    }

    extension(DataLocation location)
    {
        public DataLocation<TValue> As<TValue>()
            where TValue : unmanaged, IDataValue<TValue> =>
            location.Offset.Length >= TValue.ValueSize ? new(location.Offset) : throw new InvalidCastException();
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public ref TValue GetRefValue(IDataAccess access) =>
            ref MemoryMarshal.AsRef<TValue>(location.GetMemory(access).Span);
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>, IDataValueDefault<TValue>
    {
        public static DataLocation<TValue> Create(IDataAccess access)
        {
            return DataLocation<TValue>.Create(access, TValue.Default);
        }
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public bool IsDefault => location.Offset.IsDefault;

        public DataLocation<TValue> WhenDefault(Func<DataLocation<TValue>> func) =>
            location.IsDefault ? func() : location;

        public async ValueTask<DataLocation<TValue>> WhenDefaultAsync(
            Func<CancellationToken, ValueTask<DataLocation<TValue>>> func,
            CancellationToken cancellationToken = default) =>
            location.IsDefault ? await func(cancellationToken) : location;

        public TResult? WhenNotDefault<TResult>(Func<DataLocation<TValue>, TResult> func) =>
            location.IsDefault ? func(location) : default;

        public async ValueTask<TResult?> WhenNotDefaultAsync<TResult>(
            Func<DataLocation<TValue>, CancellationToken, ValueTask<TResult?>> func,
            CancellationToken cancellationToken = default) =>
            location.IsDefault ? await func(location, cancellationToken) : default;


        public DataLocation<TValue>? NullWhenDefault() =>
            location.IsDefault ? null : location;
    }
}