using System.Text.Json;

namespace Parto.Extensions.Node;

public static class NodeDefaults
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new(JsonSerializerDefaults.Web);
}