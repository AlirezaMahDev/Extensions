namespace AlirezaMahDev.Extensions.File.Abstractions;

public interface IFileService
{
    IFileAccess Access(string name);
}