namespace AlirezaMahDev.Extensions.File.Abstractions;

public class FileOptions
{
    public string Path { get; set; } =
        System.IO.Path.Combine(Environment.CurrentDirectory, ".data");
}