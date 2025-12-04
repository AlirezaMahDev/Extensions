using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataLocationExtensions
{
    extension(IDataLocation location)
    {
        public DataLocation<TValue> As<TValue>()
            where TValue : unmanaged
        {
            return new(location);
        }

        public DataLocationObject<TEntity> AsObject<TEntity>()
            where TEntity : class
        {
            return new(location);
        }

        public DataLocation<TValue> GetOrAdd<TValue>(String64 key)
            where TValue : unmanaged
        {
            return location.GetOrAdd(key, Unsafe.SizeOf<TValue>()).As<TValue>();
        }

        public IDataLocation GetOrAdd(String64 key)
        {
            return location.GetOrAdd(key, 0);
        }

        public IDataLocation ForceAdd(String64 key)
        {
            return location.ForceAdd(key, 0);
        }

        public DataBlockMemory? LastData()
        {
            return location.FirstOrDefault()?.Data.AsMemory();
        }

        public DataBlockMemory NewData(int length)
        {
            return location.GetOrAdd(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), length)
                .Data.AsMemory();
        }
    }
}