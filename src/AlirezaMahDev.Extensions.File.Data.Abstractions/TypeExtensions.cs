using System.Reflection;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class TypeExtensions
{
    extension(Type type)
    {
        public bool IsUnmanaged
        {
            get
            {
                if (type.IsPrimitive || type.IsPointer || type.IsEnum)
                {
                    return true;
                }

                if (type.IsValueType)
                {
                    return type
                        .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .All(fieldInfo => fieldInfo.FieldType.IsUnmanaged);
                }

                return false;
            }
        }
    }
}