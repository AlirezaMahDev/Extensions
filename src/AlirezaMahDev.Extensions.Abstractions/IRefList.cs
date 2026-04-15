using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefList<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefList<TSelf, T>, allows ref struct
{
    int Add(in T value);
    int Add(params ReadOnlySpan<T> values);
    bool Insert(int index, in T value);
    bool Insert(int index, params ReadOnlySpan<T> values);
    bool Remove(int index, [NotNullWhen(true)] out T? result);
    bool Remove(int index, Span<T> result);
    void Clean();  
    
}