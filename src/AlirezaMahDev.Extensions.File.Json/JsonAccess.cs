using System.Text.Json;

using AlirezaMahDev.Extensions.File.Json.Abstractions;
using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File.Json;

internal class JsonAccess<TEntity>(
    IFileAccess fileAccess)
    : IJsonAccess<TEntity>, IAsyncDisposable, IDisposable
    where TEntity : class
{
    private TEntity? _entity;
    private bool _dispose;

    public JsonSerializerOptions JsonSerializerOptions { get; } = new(JsonSerializerDefaults.General);

    public TEntity GetEntity()
    {
        if (_entity is not null)
        {
            return _entity;
        }

        _entity = fileAccess.Access(stream =>
            stream.Length != 0 &&
            JsonSerializer.Deserialize<TEntity>(stream, JsonSerializerOptions) is { } entity
                ? entity
                : Activator.CreateInstance<TEntity>());

        return _entity;
    }

    public async ValueTask<TEntity> GetEntityAsync(CancellationToken cancellationToken = default)
    {
        if (_entity is not null)
        {
            return _entity;
        }

        _entity = await fileAccess.AccessAsync(async (stream, token) =>
                stream.Length != 0 &&
                await JsonSerializer.DeserializeAsync<TEntity>(stream, JsonSerializerOptions, token) is { } entity
                    ? entity
                    : Activator.CreateInstance<TEntity>(),
            cancellationToken);

        return _entity;
    }

    public void Save()
    {
        if (_entity is null)
        {
            return;
        }

        fileAccess.Replace(stream =>
            JsonSerializer.Serialize(stream, _entity, JsonSerializerOptions));
    }

    public async ValueTask SaveAsync(CancellationToken cancellationToken = default)
    {
        if (_entity is null)
        {
            return;
        }

        await fileAccess.ReplaceAsync(async (stream, token) =>
                await JsonSerializer.SerializeAsync(stream, _entity, JsonSerializerOptions, token),
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_dispose)
        {
            return;
        }

        if (_entity is not null)
        {
            await SaveAsync();
        }

        GC.SuppressFinalize(this);
        _dispose = true;
    }

    private void ReleaseUnmanagedResources()
    {
        _entity = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            Save();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        ReleaseUnmanagedResources();
        await SaveAsync();
    }

    ~JsonAccess()
    {
        Dispose(false);
    }
}