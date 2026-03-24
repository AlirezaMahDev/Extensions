namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationStorageExtensions
{
    extension<TValue>(IDataStorage<TValue> value)
        where TValue : unmanaged, IDataStorage<TValue>
    {
        public IDataStorage<TValue> Storage()
        {
            return null!;
        }
    }

    extension<TValue>(DataWrap<TValue, IDataStorage<TValue>> wrap)
        where TValue : unmanaged, IDataStorage<TValue>
    {
        public bool GetData(out DataLocation result)
        {
            ref readonly var refVal = ref wrap.Location.GetRefValue(wrap.Access);
            if (refVal.Data.IsNull)
            {
                result = default;
                return false;
            }

            result = new(refVal.Data);
            return true;
        }

        public bool GetData<TDataValue>(out DataLocation<TDataValue> result)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            ref readonly var refVal = ref wrap.Location.GetRefValue(wrap.Access);

            if (refVal.Data.IsNull)
            {
                result = default;
                return false;
            }

            if (TDataValue.ValueSize != refVal.Data.Length)
            {
                ThrowHelper.ThrowInvalidDataException(
                    $"{typeof(TDataValue).Name} size must be {refVal.Data.Length}");
            }

            result = new(refVal.Data);
            return true;
        }


        public DataLocation CreateNewData(int length)
        {
            if (wrap.GetData(out var dataLocation))
            {
                wrap.Access.GetTrash().Add(wrap.Access, in dataLocation);
            }

            wrap.Access.Create(length, out var newDataLocation);
            wrap.Location.GetRefValue(wrap.Access).Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            if (wrap.GetData(out var dataLocation))
            {
                wrap.Access.GetTrash().Add(wrap.Access, in dataLocation);
            }

            wrap.Access.Create<TDataValue>(out var newDataLocation);
            wrap.Location.GetRefValue(wrap.Access).Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            if (wrap.GetData(out var dataLocation))
            {
                wrap.Access.GetTrash().Add(wrap.Access, in dataLocation);
            }

            wrap.Access.Create(@default, out var newDataLocation);
            wrap.Location.GetRefValue(wrap.Access).Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataLocation GetOrCreateData(int length)
        {
            return wrap.GetData(out var dataLocation)
                ? dataLocation.Offset.Length == length
                    ? dataLocation
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Offset.Length} but you need {length}")
                : wrap.CreateNewData(length);
        }

        public DataLocation<TDataValue> GetOrCreateData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            return wrap.GetData<TValue, TDataValue>(out var dataLocation)
                ? dataLocation.Offset.Length == TDataValue.ValueSize
                    ? dataLocation
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Offset.Length} but you need {TDataValue.ValueSize}")
                : wrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return wrap.GetData<TValue, TDataValue>(out var dataLocation)
                ? dataLocation.Offset.Length == TDataValue.ValueSize
                    ? dataLocation
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Offset.Length} but you need {TDataValue.ValueSize}")
                : wrap.CreateNewData(@default);
        }

        public DataLocation GetOrCreateNewData(int length)
        {
            return wrap.GetData(out var dataLocation) && dataLocation.Offset.Length == length
                ? dataLocation
                : wrap.CreateNewData(length);
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            return wrap.GetData<TValue, TDataValue>(out var dataLocation) &&
                   dataLocation.Offset.Length == TDataValue.ValueSize
                ? dataLocation
                : wrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return wrap.GetData<TValue, TDataValue>(out var dataLocation) &&
                   dataLocation.Offset.Length == TDataValue.ValueSize
                ? dataLocation
                : wrap.CreateNewData(@default);
        }
    }
}