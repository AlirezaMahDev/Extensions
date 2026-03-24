using System.Text.Json;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class JsonElementExtensions
{
    extension(JsonElement.ObjectEnumerator jsonElements)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public JsonProperty? FirstOrNull(Func<JsonProperty, bool> predicate)
        {
            using var enumerator = jsonElements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    return enumerator.Current;
                }
            }

            return null;
        }
    }

    extension(JsonElement.ArrayEnumerator jsonElements)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public JsonElement? FirstOrNull(Func<JsonElement, bool> predicate)
        {
            using var enumerator = jsonElements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    return enumerator.Current;
                }
            }

            return null;
        }
    }
}