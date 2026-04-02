namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataOffsetExtensions
{
    extension(in DataOffset offset)
    {
        public DataLocation<TValue> AsLocation<TValue>(IDataAccess access)
            where TValue : unmanaged, IDataValue<TValue>
        {
            DataLocation<TValue>.Read(access, offset, out var location);
            return location;
        }

        public Optional<DataLocation<TValue>> AsOptionalLocation<TValue>(IDataAccess access)
            where TValue : unmanaged, IDataValue<TValue>
        {
            if (offset.IsNull)
                return Optional<DataLocation<TValue>>.Null;
            DataLocation<TValue>.Read(access, offset, out var location);
            return location;
        }
    }
}