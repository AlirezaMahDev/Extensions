using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace AlirezaMahDev.Extensions.File.Json.Abstractions;

public interface IJsonAccess<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>
    where TEntity : class
{
    JsonSerializerOptions JsonSerializerOptions { get; }

    TEntity GetEntity();
    ValueTask<TEntity> GetEntityAsync(CancellationToken cancellationToken = default);

    void Save();
    ValueTask SaveAsync(CancellationToken cancellationToken = default);
}