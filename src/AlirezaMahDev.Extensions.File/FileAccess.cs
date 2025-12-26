using AlirezaMahDev.Extensions.File.Abstractions;

using Microsoft.Extensions.Options;

using FileOptions = AlirezaMahDev.Extensions.File.Abstractions.FileOptions;

namespace AlirezaMahDev.Extensions.File;

internal class FileAccess(
    string name,
    IOptionsMonitor<FileOptions> options,
    IServiceProvider provider)
    : IFileAccess, IAsyncDisposable, IDisposable
{
    private bool _disposed;
    private const string Format = "fb";
    private const string NewFormat = $"new.{Format}";
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private FileStream? _fileStream;

    public IServiceProvider Provider { get; } = provider;
    public string Path { get; } = System.IO.Path.Combine(options.CurrentValue.Path, $"{name}.{Format}");

    public FileStream FileStream =>
        _fileStream ??= new(Path, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, FileShare.None);

    private void CloseFileStream()
    {
        FileStream.Flush();
        FileStream.Dispose();
        _fileStream = null;
    }

    private async ValueTask CloseFileStreamAsync(CancellationToken cancellationToken = default)
    {
        await FileStream.FlushAsync(cancellationToken);
        await FileStream.DisposeAsync();
        _fileStream = null;
    }


    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        DisposeCore();
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    public void Access(Action<Stream> action)
    {
        _semaphoreSlim.Wait();
        FileStream.Seek(0, SeekOrigin.Begin);
        action(FileStream);
        FileStream.Flush();
        _semaphoreSlim.Release();
    }

    public TResult Access<TResult>(Func<Stream, TResult> func)
    {
        _semaphoreSlim.Wait();
        FileStream.Seek(0, SeekOrigin.Begin);
        var result = func(FileStream);
        FileStream.Flush();
        _semaphoreSlim.Release();
        return result;
    }

    public async ValueTask AccessAsync(Func<Stream, CancellationToken, ValueTask> func,
        CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        FileStream.Seek(0, SeekOrigin.Begin);
        await func(FileStream, cancellationToken);
        await FileStream.FlushAsync(cancellationToken);
        _semaphoreSlim.Release();
    }

    public async ValueTask<TResult> AccessAsync<TResult>(Func<Stream, CancellationToken, ValueTask<TResult>> func,
        CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        FileStream.Seek(0, SeekOrigin.Begin);
        var result = await func(FileStream, cancellationToken);
        await FileStream.FlushAsync(cancellationToken);
        _semaphoreSlim.Release();
        return result;
    }

    public void Replace(Action<Stream> action)
    {
        _semaphoreSlim.Wait();
        var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path)!, $"{name}.{NewFormat}");
        using (FileStream newFileStream = new(newPath,
                   FileMode.Create,
                   System.IO.FileAccess.ReadWrite,
                   FileShare.None))
        {
            action(newFileStream);
        }

        CloseFileStream();
        System.IO.File.Move(newPath, Path, true);
        _semaphoreSlim.Release();
    }

    public async ValueTask ReplaceAsync(Func<Stream, CancellationToken, ValueTask> func,
        CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path)!, $"{name}.{NewFormat}");
        await using (FileStream newFileStream = new(newPath,
                         FileMode.Create,
                         System.IO.FileAccess.ReadWrite,
                         FileShare.None))
        {
            await func(newFileStream, cancellationToken);
        }

        await CloseFileStreamAsync(cancellationToken);
        System.IO.File.Move(newPath, Path, true);
        _semaphoreSlim.Release();
    }


    public void Change(Action<Stream, Stream> action)
    {
        _semaphoreSlim.Wait();
        var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path)!, $"{name}.{NewFormat}");
        using (FileStream newFileStream = new(newPath,
                   FileMode.Create,
                   System.IO.FileAccess.ReadWrite,
                   FileShare.None))
        {
            FileStream.Seek(0, SeekOrigin.Begin);
            action(FileStream, newFileStream);
        }

        CloseFileStream();
        System.IO.File.Move(newPath, Path, true);
        _semaphoreSlim.Release();
    }

    public async ValueTask ChangeAsync(Func<Stream, Stream, CancellationToken, ValueTask> func,
        CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        var newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path)!, $"{name}.{NewFormat}");
        await using (FileStream newFileStream = new(newPath,
                         FileMode.Create,
                         System.IO.FileAccess.ReadWrite,
                         FileShare.None))
        {
            FileStream.Seek(0, SeekOrigin.Begin);
            await func(FileStream, newFileStream, cancellationToken);
        }

        await CloseFileStreamAsync(cancellationToken);
        System.IO.File.Move(newPath, Path, true);
        _semaphoreSlim.Release();
    }

    protected virtual void DisposeCore()
    {
        _semaphoreSlim.Dispose();
        FileStream.Dispose();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        _semaphoreSlim.Dispose();
        await FileStream.DisposeAsync();
    }

    private bool Equals(FileAccess? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Path == other.Path;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((FileAccess)obj);
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }

    public static bool operator ==(FileAccess? left, FileAccess? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FileAccess? left, FileAccess? right)
    {
        return !Equals(left, right);
    }
}