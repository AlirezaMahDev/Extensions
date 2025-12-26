namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class StringExtensions
{
    extension(String256 string256)
    {
        public Type ToType()
        {
            return Type.GetType(string256)!;
        }
    }

    extension(Type type)
    {
        public String256 ToString256()
        {
            return type.FullName!;
        }
    }
}