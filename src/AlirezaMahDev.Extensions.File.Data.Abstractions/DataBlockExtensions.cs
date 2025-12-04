using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataBlockExtensions
{
    extension(IDataBlock dataBlock)
    {
        public IDataBlockAccessor Read(long id, int length)
        {
            return dataBlock.Read(new(id, length));
        }

        public DataBlockReader AsReader(long id)
        {
            return new(dataBlock, id);
        }

        public DataBlockMemoryObject ReadObject(long id, Type type)
        {
            return dataBlock.Read(id, Marshal.SizeOf(type)).AsMemory().AsObject(type);
        }

        public DataBlockMemoryValue<T> ReadValue<T>(long id)
            where T : unmanaged
        {
            return dataBlock.Read(id, Unsafe.SizeOf<T>()).AsMemory().AsValue<T>();
        }

        public DataBlockMemoryRefValue<T> ReadRefValue<T>(long id)
            where T : unmanaged
        {
            return dataBlock.Read(id, Unsafe.SizeOf<T>()).AsMemory().AsRefValue<T>();
        }
    }
}