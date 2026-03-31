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

    extension<TValue>(in DataLocationWrap<TValue, IDataStorage<TValue>> wrap)
        where TValue : unmanaged, IDataStorage<TValue>
    {
        public bool GetData(out DataOffset result)
        {
            using var @lock = wrap.Location.ReadLock();
            ref readonly var refVal = ref @lock.RefReadOnlyValue;
            if (refVal.Data.IsNull)
            {
                result = default;
                return false;
            }

            result = refVal.Data;
            return true;
        }

        public bool GetData<TDataValue>(out DataLocation<TDataValue> result)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            using var @lock = wrap.Location.ReadLock();
            ref readonly var refVal = ref @lock.RefReadOnlyValue;

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

            DataLocation<TDataValue>.Read(wrap.Access, refVal.Data, out result);
            return true;
        }


        public DataOffset CreateNewData(int length)
        {
            if (wrap.GetData(out var dataLocation))
            {
                wrap.Access.Trash.Add(wrap.Access, in dataLocation);
            }

            var result = wrap.Access.AllocateOffset(length);
            using var writeLock = wrap.Location.WriteLock();
            writeLock.RefValue.Data = result;
            return result;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            if (wrap.GetData(out var dataLocation))
            {
                wrap.Access.Trash.Add(wrap.Access, in dataLocation);
            }

            wrap.Access.Create(out DataLocation<TDataValue> newDataLocation);
            using var @lock = wrap.Location.WriteLock();
            @lock.RefValue.Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            if (wrap.GetData(out var dataLocation))
            {
                wrap.Access.Trash.Add(wrap.Access, in dataLocation);
            }

            wrap.Access.Create(@default, out var newDataLocation);
            using var @lock = wrap.Location.WriteLock();
            @lock.RefValue.Data = newDataLocation.Offset;
            return newDataLocation;
        }

        public DataOffset GetOrCreateData(int length)
        {
            return wrap.GetData(out var dataLocation)
                ? dataLocation.Length == length
                    ? dataLocation
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Length} but you need {length}")
                : wrap.CreateNewData(length);
        }

        public DataLocation<TDataValue> GetOrCreateData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            return wrap.GetData(out DataLocation<TDataValue> dataLocation)
                ? dataLocation.Offset.Length == TDataValue.ValueSize
                    ? dataLocation
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Offset.Length} but you need {TDataValue.ValueSize}")
                : wrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return wrap.GetData(out DataLocation<TDataValue> dataLocation)
                ? dataLocation.Offset.Length == TDataValue.ValueSize
                    ? dataLocation
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Offset.Length} but you need {TDataValue.ValueSize}")
                : wrap.CreateNewData(@default);
        }

        public DataOffset GetOrCreateNewData(int length)
        {
            return wrap.GetData(out var dataLocation) && dataLocation.Length == length
                ? dataLocation
                : wrap.CreateNewData(length);
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            return wrap.GetData(out DataLocation<TDataValue> dataLocation) &&
                   dataLocation.Offset.Length == TDataValue.ValueSize
                ? dataLocation
                : wrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return wrap.GetData(out DataLocation<TDataValue> dataLocation) &&
                   dataLocation.Offset.Length == TDataValue.ValueSize
                ? dataLocation
                : wrap.CreateNewData(@default);
        }
    }
}