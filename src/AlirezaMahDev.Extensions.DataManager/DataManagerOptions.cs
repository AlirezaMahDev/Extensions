namespace AlirezaMahDev.Extensions.DataManager;

public class DataManagerOptions
{
    public string DirectoryPath { get; set; } = Path.Combine(Environment.CurrentDirectory, ".data");
    public string DefaultName { get; set; } = "data";
}