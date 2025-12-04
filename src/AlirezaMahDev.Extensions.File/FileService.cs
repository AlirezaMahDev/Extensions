using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File;

internal class FileService(FileAccessFactory fileAccessFactory) : IFileService
{
    public IFileAccess Access(string name)
    {
        return fileAccessFactory.GetOrCreate(name);
    }
}