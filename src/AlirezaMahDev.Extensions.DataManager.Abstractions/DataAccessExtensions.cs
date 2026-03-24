namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataAccessExtensions
{
    extension(IDataAccess access)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Create(int length, out DataLocation result)
        {
            DataLocation.Create(access, length, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Create<TDataValue>(out DataLocation<TDataValue> result)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            DataLocation<TDataValue>.Create(access, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Create<TDataValue>(TDataValue @default, out DataLocation<TDataValue> result)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            DataLocation<TDataValue>.Create(access, @default, out result);
        }
    }
}