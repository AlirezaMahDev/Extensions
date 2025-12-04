using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataBlockReaderExtensions
{
    extension<TReader>(TReader reader)
        where TReader : IDataBlockReader<TReader>
    {
        public TReader ReadValue<TValue>(out DataBlockMemoryValue<TValue> blockMemoryValue)
            where TValue : unmanaged
        {
            reader = reader.Read(Unsafe.SizeOf<TValue>(), out var block);
            blockMemoryValue = block.AsValue<TValue>();
            return reader;
        }

        public TReader ReadRefValue<TValue>(out DataBlockMemoryRefValue<TValue> blockRefValue)
            where TValue : unmanaged
        {
            reader = reader.Read(Unsafe.SizeOf<TValue>(), out var block);
            blockRefValue = block.AsRefValue<TValue>();
            return reader;
        }

        public TReader ReadObject(Type type,
            out DataBlockMemoryObject blockMemoryObject)
        {
            reader = reader.Read(Marshal.SizeOf(type), out var block);
            blockMemoryObject = block.AsObject(type);
            return reader;
        }
    }
}