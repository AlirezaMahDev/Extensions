using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.File.Abstractions;

public class FileOptions : IOptionsBase
{
    public static string Key { get; } = "File";
    public string Path { get; set; } =
        System.IO.Path.Combine(Environment.CurrentDirectory, ".data");
}