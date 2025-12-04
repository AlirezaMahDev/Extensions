namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class StringExtensions
{
    public static Type ToType(this String256 string256)
    {
        return Type.GetType(string256)!;
    }

    public static String256 ToString256(this Type type)
    {
        return type.FullName!;
    }
}