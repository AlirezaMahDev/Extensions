namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataMapFilePartCache
{
    bool Accessed { get; }
    ulong AccessCount { get; }
    ulong LastHash { get; }
    ulong CurrentHash { get; }
    bool HasChange { get; }

    void AccessRefByte(int offset, ScopedRefAction<byte> action);
    TResult AccessRefByte<TResult>(int offset, ScopedRefFunc<byte, TResult> func);
    void AccessSpan(int offset, ScopedRefReadOnlyAction<Span<byte>> action);
    TResult AccessSpan<TResult>(int offset, ScopedRefReadOnlyFunc<Span<byte>, TResult> func);
    void AccessRefReadOnlyByte(int offset, ScopedRefReadOnlyAction<byte> action);
    TResult AccessRefReadOnlyByte<TResult>(int offset, ScopedRefReadOnlyFunc<byte, TResult> func);
    void AccessReadOnlySpan(int offset, ScopedRefReadOnlyAction<ReadOnlySpan<byte>> action);
    TResult AccessReadOnlySpan<TResult>(int offset, ScopedRefReadOnlyFunc<ReadOnlySpan<byte>, TResult> func);

    ref byte EnterAccessRefByte(int offset);
    Span<byte> EnterAccessSpan(int offset);
    ref readonly byte EnterAccessRefReadOnlyByte(int offset);
    ReadOnlySpan<byte> EnterAccessReadOnlySpan(int offset);
    void ExitAccess();
}