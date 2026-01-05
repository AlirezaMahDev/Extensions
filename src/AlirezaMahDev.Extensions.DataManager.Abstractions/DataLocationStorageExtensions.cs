namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationStorageExtensions
{
    extension<TValue>(IDataStorage<TValue> value)
        where TValue : unmanaged, IDataStorage<TValue>
    {
        public IDataStorage<TValue> Storage() => null!;
    }

    extension<TValue>(DataWrap<TValue, IDataStorage<TValue>> wrap)
        where TValue : unmanaged, IDataStorage<TValue>
    {
        public DataLocation? TryGetData()
        {
            return wrap.Location.GetRefValue(wrap.Access).Data.IsNull
                ? null
                : new(wrap.Location.GetRefValue(wrap.Access).Data);
        }

        public DataLocation<TDataValue>? TryGetData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return wrap.Location.GetRefValue(wrap.Access).Data.IsNull
                ? null
                : TDataValue.ValueSize == wrap.Location.GetRefValue(wrap.Access).Data.Length
                    ? new(wrap.Location
                        .GetRefValue(wrap.Access)
                        .Data)
                    : throw new InvalidDataException($"{typeof(TDataValue).Name} size must be {
                        wrap.Location.GetRefValue(wrap.Access).Data.Length}");
        }


        public DataLocation CreateNewData(int length)
        {
            var dataLocation = wrap.TryGetData();
            if (dataLocation.HasValue)
                wrap.Access.GetTrash().Add(wrap.Access, dataLocation.Value);
            var newDataLocation = wrap.Access.Create(length);
            wrap.Location.GetRefValue(wrap.Access).Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = wrap.TryGetData();
            if (dataLocation.HasValue)
                wrap.Access.GetTrash().Add(wrap.Access, dataLocation.Value);
            var newDataLocation = wrap.Access.Create<TDataValue>();
            wrap.Location.GetRefValue(wrap.Access).Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = wrap.TryGetData();
            if (dataLocation.HasValue)
                wrap.Access.GetTrash().Add(wrap.Access, dataLocation.Value);
            var newDataLocation = wrap.Access.Create(@default);
            wrap.Location.GetRefValue(wrap.Access).Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataLocation GetOrCreateData(int length)
        {
            var dataLocation = wrap.TryGetData();
            return dataLocation.HasValue
                ? dataLocation.Value.Offset.Length == length
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Offset.Length} but you need {length}")
                : wrap.CreateNewData(length);
        }

        public DataLocation<TDataValue> GetOrCreateData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = wrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue
                ? dataLocation.Value.Offset.Length == TDataValue.ValueSize
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Offset.Length} but you need {TDataValue.ValueSize}")
                : wrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = wrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue
                ? dataLocation.Value.Offset.Length == TDataValue.ValueSize
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Offset.Length} but you need {TDataValue.ValueSize}")
                : wrap.CreateNewData(@default);
        }

        public DataLocation GetOrCreateNewData(int length)
        {
            var dataLocation = wrap.TryGetData();
            return dataLocation.HasValue && dataLocation.Value.Offset.Length == length
                ? dataLocation.Value
                : wrap.CreateNewData(length);
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = wrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue && dataLocation.Value.Offset.Length == TDataValue.ValueSize
                ? dataLocation.Value
                : wrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = wrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue && dataLocation.Value.Offset.Length == TDataValue.ValueSize
                ? dataLocation.Value
                : wrap.CreateNewData(@default);
        }
    }
}