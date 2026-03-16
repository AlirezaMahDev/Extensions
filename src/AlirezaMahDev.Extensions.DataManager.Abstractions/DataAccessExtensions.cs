namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataAccessExtensions
{
    extension(IDataAccess access)
    {
        public DataLocation Create(int length)
        {
            return DataLocation.Create(access, length);
        }

        public DataLocation<TDataValue> Create<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            return DataLocation<TDataValue>.Create(access);
        }

        public DataLocation<TDataValue> Create<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return DataLocation<TDataValue>.Create(access, @default);
        }
    }
}