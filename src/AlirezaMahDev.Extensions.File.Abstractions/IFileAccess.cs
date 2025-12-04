namespace AlirezaMahDev.Extensions.File.Abstractions;

public interface IFileAccess
{
    IServiceProvider Provider { get; }

    string Path { get; }
    FileStream FileStream { get; }

    void Access(Action<Stream> action);
    TResult Access<TResult>(Func<Stream, TResult> func);

    ValueTask AccessAsync(Func<Stream, CancellationToken, ValueTask> func,
        CancellationToken cancellationToken = default);

    ValueTask<TResult> AccessAsync<TResult>(Func<Stream, CancellationToken, ValueTask<TResult>> func,
        CancellationToken cancellationToken = default);

    void Replace(Action<Stream> action);

    ValueTask ReplaceAsync(Func<Stream, CancellationToken, ValueTask> func,
        CancellationToken cancellationToken = default);

    void Change(Action<Stream, Stream> action);

    ValueTask ChangeAsync(Func<Stream, Stream, CancellationToken, ValueTask> func,
        CancellationToken cancellationToken = default);
}