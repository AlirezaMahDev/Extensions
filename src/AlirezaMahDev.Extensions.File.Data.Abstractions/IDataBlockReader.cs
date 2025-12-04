namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataBlockReader<out TReader>
    where TReader : IDataBlockReader<TReader>
{
    TReader Read(int length, out DataBlockMemory result);
}