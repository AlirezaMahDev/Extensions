namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataBlockMemoryExtensions
{
    extension(IDataBlockAccessor dataBlockAccessor)
    {
        public DataBlockMemory AsMemory()
        {
            return new(dataBlockAccessor);
        }
    }

    extension(DataBlockMemory block)
    {
        public DataBlockMemoryReader AsReader()
        {
            return new(block);
        }

        public DataBlockMemoryObject AsObject(Type type)
        {
            return new(block, type);
        }

        public DataBlockMemoryValue<T> AsValue<T>()
            where T : unmanaged
        {
            return new(block);
        }

        public DataBlockMemoryRefValue<T> AsRefValue<T>()
            where T : unmanaged
        {
            return new(block);
        }
    }
}