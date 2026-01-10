using AlirezaMahDev.Extensions.File.Abstractions;

namespace AlirezaMahDev.Extensions.File;

public static class FileAccessExtensions
{
    extension(IFileAccess fileAccess)
    {
        public void MemoryAccess(Action<Stream> action)
        {
            fileAccess.Access(stream =>
            {
                using MemoryStream memoryStream = new();
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                action(memoryStream);
            });
        }

        public TResult MemoryAccess<TResult>(Func<Stream, TResult> func)
        {
            return fileAccess.Access(stream =>
            {
                using MemoryStream memoryStream = new();
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return func(memoryStream);
            });
        }

        public async ValueTask MemoryAccessAsync(Func<Stream, CancellationToken, ValueTask> func,
            CancellationToken cancellationToken = default)
        {
            await fileAccess.AccessAsync(async (stream, token) =>
                {
                    await using MemoryStream memoryStream = new();
                    await stream.CopyToAsync(memoryStream, token);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await func(memoryStream, token);
                },
                cancellationToken);
        }

        public async ValueTask<TResult> MemoryAccessAsync<TResult>(
            Func<Stream, CancellationToken, ValueTask<TResult>> func,
            CancellationToken cancellationToken = default)
        {
            return await fileAccess.AccessAsync(async (stream, token) =>
                {
                    await using MemoryStream memoryStream = new();
                    await stream.CopyToAsync(memoryStream, token);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return await func(memoryStream, token);
                },
                cancellationToken);
        }

        public void MemoryReplace(Action<Stream> action)
        {
            fileAccess.Replace(stream =>
            {
                using MemoryStream memoryStream = new();
                action(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(stream);
            });
        }

        public async ValueTask MemoryReplaceAsync(Func<Stream, CancellationToken, ValueTask> func,
            CancellationToken cancellationToken = default)
        {
            await fileAccess.ReplaceAsync(async (stream, token) =>
                {
                    await using MemoryStream memoryStream = new();
                    await func(memoryStream, token);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(stream, token);
                },
                cancellationToken);
        }

        public void MemoryChange(Action<Stream, Stream> action)
        {
            fileAccess.Change((fromStream, toStream) =>
            {
                using MemoryStream memoryStream = new();
                fromStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                action(memoryStream, toStream);
            });
        }

        public async ValueTask MemoryChangeAsync(Func<Stream, Stream, CancellationToken, ValueTask> func,
            CancellationToken cancellationToken = default)
        {
            await fileAccess.ChangeAsync(async (fromStream, toStream, token) =>
                {
                    await using MemoryStream memoryStream = new();
                    await fromStream.CopyToAsync(memoryStream, token);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await func(memoryStream, toStream, token);
                },
                cancellationToken);
        }
    }
}