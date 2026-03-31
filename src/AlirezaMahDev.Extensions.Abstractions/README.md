# AlirezaMahDev.Extensions.Abstractions

## Project Description

This project serves as the foundation for the AlirezaMahDev.Extensions ecosystem, providing essential abstractions and interfaces that enable consistent and type-safe configuration patterns across all extension libraries. It defines the core contracts for implementing the builder pattern with dependency injection and options configuration in .NET applications.

## Dependencies

### Core Dependencies

- **Microsoft.Extensions.Hosting.Abstractions**: Provides the fundamental interfaces for .NET's hosting infrastructure.
- **JetBrains.Annotations**: Enhances code quality with static analysis attributes and code annotations.

### Utility Dependencies

- **CommunityToolkit.Common**: Offers common utility methods and extension methods.
- **CommunityToolkit.Diagnostics**: Provides guard clauses and argument validation utilities.
- **CommunityToolkit.HighPerformance**: Delivers high-performance programming utilities and optimizations.

### Resilience Dependencies

- **Polly**: A .NET resilience and transient-fault-handling library.
- **Polly.Extensions.Http**: Extends Polly with HTTP-specific resilience policies and utilities.

---

## Core Interfaces

### IOptionsBase

The `IOptionsBase` interface is the foundation for all configuration options in the ecosystem:

```csharp
public interface IOptionsBase
{
    static abstract string Key { get; }
}
```

**Key Features:**

- Enforces a consistent way to define configuration keys
- Supports hierarchical configuration through key composition
- Enables type-safe configuration binding

### IBuilderBase

The `IBuilderBase` interface hierarchy provides a fluent API for configuring services and options:

```csharp
public interface IBuilderBase
{
    IServiceCollection Services { get; }
}

public interface IBuilderBase<TOptions> : IBuilderBase
    where TOptions : class, IOptionsBase
{
    OptionsBuilder<TOptions> OptionsBuilder { get; }
    
    OptionsBuilder<TSubOptions> AddSubOptions<TSubOptions>()
        where TSubOptions : class, IOptionsBase;
}
```

### IScopedRefReadOnlyEquatable<T>

Interface for scoped ref equality comparison:

```csharp
public interface IScopedRefReadOnlyEquatable<T> : IEquatable<T>
    where T : allows ref struct
{
    bool Equals(scoped ref readonly T other);
}
```

### IScopedRefReadOnlyComparable<T>

Interface for scoped ref comparison:

```csharp
public interface IScopedRefReadOnlyComparable<T> : IComparable<T>
    where T : allows ref struct
{
    int CompareTo(scoped ref readonly T other);
}
```

### IScopedRefReadOnlyComparer<T>

Interface for scoped ref comparison with IComparer:

```csharp
public interface IScopedRefReadOnlyComparer<T> : IComparer<T>
    where T : allows ref struct
{
    int Compare(scoped ref readonly T x, scoped ref readonly T y);
}
```

### IScopedInEqualityOperators<TSelf, TOther, TResult>

Interface for scoped in equality operators:

```csharp
public interface IScopedInEqualityOperators<TSelf, TOther, TResult> : IEqualityOperators<TSelf, TOther, TResult>
{
    public static abstract TResult operator ==(scoped in TSelf? left, scoped in TOther? right);
    public static abstract TResult operator !=(scoped in TSelf? left, scoped in TOther? right);
}
```

---

## Data Structures (Structs)

### BinarySearchRange

```csharp
public readonly record struct BinarySearchRange(int Start, int End);
```

Represents a range for binary search results.

**Properties:**

- `Start`: Start index of the range
- `End`: End index of the range

**Usage:**

```csharp
var range = new BinarySearchRange(5, 10);
if (range.TryGetRange(out var result))
{
    // Use result (Range type)
}
```

### ComparisonWrap<TComparisonTarget, T>

```csharp
public struct ComparisonWrap<TComparisonTarget, T>(TComparisonTarget unWrap)
    where T : allows ref struct
```

A wrapper for comparison collections.

**Properties:**

- `UnWrap`: The wrapped comparison collection

### ComparisonCollection<T>

```csharp
public readonly record struct ComparisonCollection<T>(IEnumerable<ScopedRefReadOnlyComparison<T>> Enumerable)
    : IComparisonCollection<T>
```

A collection of comparison operations.

### ComparisonCollectionChain<T>

```csharp
public readonly struct ComparisonCollectionChain<T>(
    IEnumerable<ScopedRefReadOnlyComparison<T>> enumerable,
    ScopedRefReadOnlyComparison<T> comparison,
    ScopedRefReadOnlyComparison<T> currentComparison,
    IComparisonChain<T>? previousComparisonChain)
    : IComparisonCollection<T>, IComparisonChain<T>
```

A chain of comparison operations for multi-level sorting.

### Disposable

```csharp
public readonly struct Disposable(Action action) : IDisposable
```

A simple disposable that executes an action on disposal.

**Usage:**

```csharp
using var disposable = new Disposable(() => Console.WriteLine("Disposed!"));
// Do work...
// "Disposed!" is printed when exiting the using block
```

### DisposableValue<TValue, TDisposable>

```csharp
public ref struct DisposableValue<TValue, TDisposable>(TValue value, TDisposable disposable)
    where TDisposable : IDisposable
```

A disposable wrapper around a value.

**Properties:**

- `Value`: The wrapped value

### Optional<TValue>

```csharp
public struct Optional<TValue>
    where TValue : struct, IEquatable<TValue>
```

A value type that may or may not have a value.

**Properties:**

- `HasValue`: Indicates whether the optional has a value
- `Value`: The value (getter only available when HasValue is true)

**Static Properties:**

- `Null`: Returns a null optional

**Static Methods:**

- `From(TValue value)`: Creates an optional from a value

**Usage:**

```csharp
var optional = Optional<int>.From(42);
if (optional.HasValue)
{
    var value = optional.Value; // 42
}

var nullOptional = Optional<int>.Null;
// or
var implicitOptional = (Optional<int>)42;
```

### RefOptional<TValue>

```csharp
public readonly ref struct RefOptional<TValue>
    where TValue : struct, IScopedRefReadOnlyEquatable<TValue>
```

A ref-based optional for value types.

**Properties:**

- `HasValue`: Indicates whether the optional has a value
- `Value`: Reference to the value

### RefReadOnlyOptional<TValue>

```csharp
public readonly ref struct RefReadOnlyOptional<TValue>
    where TValue : struct, IScopedRefReadOnlyEquatable<TValue>
```

A readonly ref-based optional.

### MemoryValue<T>

```csharp
public readonly struct MemoryValue<T>
    where T : struct
```

A wrapper for a single element memory.

**Properties:**

- `HasValue`: Indicates whether the memory has a value
- `Value`: Reference to the value

**Implicit Conversions:**

- From `T` to `MemoryValue<T>`
- From `MemoryValue<T>` to `Memory<T>`
- From `MemoryValue<T>` to `ReadOnlyMemoryValue<T>`
- From `MemoryValue<T>` to `SpanValue<T>`
- From `MemoryValue<T>` to `ReadOnlySpanValue<T>`

### ReadOnlyMemoryValue<T>

```csharp
public readonly struct ReadOnlyMemoryValue<T>
    where T : struct
```

A readonly wrapper for a single element memory.

### SpanValue<T>

```csharp
public readonly ref struct SpanValue<T>
    where T : struct
```

A wrapper for a single element span.

### ReadOnlySpanValue<T>

```csharp
public readonly ref struct ReadOnlySpanValue<T>
    where T : struct
```

A readonly wrapper for a single element span.

### MemoryList<T>

```csharp
public sealed class MemoryList<T> : IMemoryList<T>
```

A high-performance list backed by `Memory<T>`.

**Properties:**

- `Count`: Number of elements
- `Memory`: The memory buffer
- `IsReadOnly`: Always false

**Indexers:**

- `ref T this[int index]`: Get/set by reference

**Methods:**

- `Add(T item)`: Add an item
- `Clear()`: Clear all items
- `Remove(T item)`: Remove an item
- `RemoveAt(int index)`: Remove at index
- `Insert(int index, T item)`: Insert at index
- `Clone()`: Create a shallow clone

**Usage:**

```csharp
var list = new MemoryList<int>(capacity: 100);
list.Add(1);
list.Add(2);
var first = list[0]; // by ref
var count = list.Count;
```

### ScoreSortItem<T>

```csharp
public record struct ScoreSortItem<T>(T Value) : IScoreSortItem
    where T : notnull
```

An item wrapper for score-based sorting.

### ScopedComparisonChain<T>

```csharp
public readonly record struct ScopedComparisonChain<T>(
    ScopedRefReadOnlyComparison<T> Comparison,
    ScopedRefReadOnlyComparison<T> CurrentComparison,
    IComparisonChain<T>? PreviousComparisonChain)
    : IComparisonChain<T>, IScopedRefReadOnlyComparer<T>
```

A chain of comparison operations.

---

## Delegate Types

### ScopedRefReadOnlyComparison<T>

```csharp
public delegate int ScopedRefReadOnlyComparison<T>(scoped ref readonly T x, scoped ref readonly T y)
    where T : allows ref struct;
```

A comparison delegate for scoped ref readonly parameters.

### ScopedRefReadOnlyFunc<T, TResult>

```csharp
public delegate TResult ScopedRefReadOnlyFunc<T, out TResult>(scoped ref readonly T arg)
    where TResult : allows ref struct
    where T : allows ref struct;
```

A function delegate for scoped ref readonly parameters.

### RefFunc<T, TResult>

```csharp
public delegate TResult RefFunc<T, out TResult>(ref T arg)
    where TResult : allows ref struct
    where T : allows ref struct;
```

A function delegate for ref parameters.

### RefReadOnlyFunc<T, TResult>

```csharp
public delegate TResult RefReadOnlyFunc<T, out TResult>(ref readonly T arg)
    where TResult : allows ref struct
    where T : allows ref struct;
```

### RefReadOnlyFuncOut<T, TResult>

```csharp
public delegate void RefReadOnlyFuncOut<T, TResult>(ref readonly T arg, out TResult result)
    where TResult : allows ref struct
    where T : allows ref struct;
```

### ComparisonBuilder<TComparisonTarget, T>

```csharp
public delegate ComparisonWrap<TComparisonTarget, T> ComparisonBuilder<TComparisonTarget, T>(
    ComparisonWrap<TComparisonTarget, T> comparisonChain)
    where T : allows ref struct;
```

---

## Extension Methods

This project uses **C# 12 Extension Methods** with the `extension` keyword.

### BinarySearchRangeExtensions

```csharp
extension(ref BinarySearchRange binarySearchRange)
```

**Methods:**

- `TryGetRange(out Range range)`: Try to convert to Range

**Usage:**

```csharp
var range = new BinarySearchRange(5, 10);
if (range.TryGetRange(out var result))
{
    // Use result (Range)
}
```

### BinarySearchSpanExtensions

```csharp
extension<T>(ReadOnlySpan<T> readOnlySpan)
```

**Methods:**

- `BinarySearchLowerBound<TComparable>(scoped ref readonly TComparable value)`: Find lower bound
- `BinarySearchUpperBound<TComparable>(scoped ref readonly TComparable value)`: Find upper bound
- `LowerBound<TComparable>(scoped ref readonly TComparable value)`: Binary search lower bound
- `UpperBound<TComparable>(scoped ref readonly TComparable value)`: Binary search upper bound
- `BinarySearchRange<TComparable>(scoped ref readonly TComparable value)`: Get range of equal elements
- `TryBinarySearchRangeSlice<TComparable>(...)`: Try to get slice of equal elements
- `BinarySearchRangeSlice<TComparable>(scoped ref readonly TComparable value)`: Get slice (throws if not found)

### ComparerExtensions

```csharp
extension<T>(Comparer<T>)
```

**Methods:**

- `NullUp(T? x, T? y)`: Compare nulls as greater than non-nulls
- `NullDown(T? x, T? y)`: Compare nulls as less than non-nulls

### ComparerHelperExtensions

```csharp
extension<T>(ComparerHelper<T>) where T : allows ref struct
```

**Methods:**

- `NullUp(scoped ref readonly T? x, scoped ref readonly T? y)`: Compare nulls up
- `NullDown(scoped ref readonly T? x, scoped ref readonly T? y)`: Compare nulls down

### ComparisonCollectionExtensions

```csharp
extension<T>(ComparisonCollection<T>)
extension<TComparisonCollection, T>(ComparisonWrap<TComparisonCollection, T>)
    where TComparisonCollection : struct, IComparisonCollection<T>
    where T : allows ref struct
```

**Methods:**

- `Wrap()`: Wrap the collection
- `With(ScopedRefReadOnlyComparison<T>)`: Add a comparison
- `WithDescending(ScopedRefReadOnlyComparison<T>)`: Add descending comparison
- `WithBy<TKey>(Func<T, TKey>)`: Order by a key

### ComparisonCollectionChainExtensions

```csharp
extension<T>(ComparisonCollectionChain<T>)
extension<T>(ComparisonCollectionChain<T> unwrap)
```

**Methods:**

- `OrderBy(ScopedRefReadOnlyComparison<T>)`: Create ordering chain
- `OrderByDescending(ScopedRefReadOnlyComparison<T>)`: Create descending ordering
- `Wrap()`: Wrap the chain

### ScopedComparisonChainExtensions

```csharp
extension<T>(ScopedComparisonChain<T>) where T : allows ref struct
extension<T>(ScopedComparisonChain<T> unwrap) where T : allows ref struct
extension<TComparisonChain, T>(ComparisonWrap<TComparisonChain, T>)
    where TComparisonChain : struct, IComparisonChain<T>
    where T : allows ref struct
```

**Methods:**

- `ChainOrder(ScopedRefReadOnlyComparison<T>)`: Add to comparison chain
- `ChainOrderDescending(ScopedRefReadOnlyComparison<T>)`: Add descending
- `ChainOrderBy<TKey>(Func<T, TKey>)`: Order by key
- `ChainOrderByDescending<TKey>(Func<T, TKey>)`: Order by key descending
- `GetComparisonChains()`: Get all chains
- `Merge()`: Merge chains

### MemoryExtensions

```csharp
extension<T>(Memory<T> memory)
```

**Methods:**

- `FirstOrDefault()`: Get first element or default
- `Skip(int length)`: Skip elements
- `Take(int length)`: Take elements
- `TakeWhile(RefFunc<T, bool>)`: Take while predicate is true
- `Where(RefFunc<T, bool>)`: Filter elements

### MemoryListExtensions

```csharp
extension<T>(IEnumerable<T> values)
extension<T>(IReadOnlyCollection<T> values)
extension<T>(ICollection<T> values)
extension<T>(ReadOnlySpan<T> values)
extension<T>(Span<T> values)
extension<T>(ReadOnlyMemory<T> values)
extension<T>(Memory<T> values)
```

**Methods:**

- `ToMemoryList()`: Convert to MemoryList<T>

### MemoryValueExtensions

```csharp
extension<T>(T t) where T : struct
extension<T>(Memory<T> memory) where T : struct
extension<T>(MemoryValue<T> memoryValue) where T : struct
```

**Methods:**

- `AsMemoryValue()`: Convert to MemoryValue<T>
- `ElementAt(int index)`: Get element at index
- `AsReadOnlyMemoryValue()`: Convert to ReadOnlyMemoryValue<T>
- `AsSpanValue()`: Convert to SpanValue<T>
- `AsReadOnlySpanValue()`: Convert to ReadOnlySpanValue<T>

### ReadOnlyMemoryValueExtensions

```csharp
extension<T>(T t) where T : struct
extension<T>(ReadOnlyMemory<T> readOnlyMemory) where T : struct
extension<T>(ReadOnlyMemoryValue<T> readOnlyMemoryValue) where T : struct
```

### ReadOnlySpanValueExtensions

```csharp
extension<T>(ref T t) where T : struct
```

### SpanExtensions

```csharp
extension<T>(Span<T> span)
```

**Methods:**

- `Select<TOut>(RefFunc<T, TOut>)`: Select elements
- `First()`: Get first element (throws if empty)
- `First(RefFunc<T, bool>)`: Get first matching element
- `FirstOrDefault()`: Get first element or null ref
- `FirstOrDefault(RefFunc<T, bool>)`: Get first matching or null ref

### SpanValueExtensions

```csharp
extension<T>(ref T t) where T : struct
extension<T>(SpanValue<T> spanValue) where T : struct
```

### SpanSortExtensions

```csharp
extension<T>(Span<T> span)
```

**Methods:**

- `Sort<TBridge>(ScopedRefReadOnlyFunc<T, TBridge>, ScopedRefReadOnlyComparison<TBridge>)`: Sort with bridge
- `Sort<TBridge, TComparer>(ScopedRefReadOnlyFunc<T, TBridge>, TComparer)`: Sort with comparer bridge

### NearBinarySearchSpanExtensions

```csharp
extension<T>(Memory<T> memory)
```

**Methods:**

- `Near(T value, ScopedComparisonChain<T>, int depth)`: Find near matches
- `Near<TBridge>(ref TBridge value, ScopedRefReadOnlyFunc<T, TBridge>, ScopedComparisonChain<TBridge>, int depth)`: Find near with bridge

### JsonElementExtensions

```csharp
extension(JsonElement.ObjectEnumerator)
extension(JsonElement.ArrayEnumerator)
```

**Methods:**

- `FirstOrNull(Func<JsonProperty, bool>)`: Find first or null
- `FirstOrNull(Func<JsonElement, bool>)`: Find first or null

### LinqExtensions

```csharp
extension<T>(IAsyncEnumerable<T> asyncEnumerable)
```

**Methods:**

- `AsAsyncEnumerable()`: Convert to async enumerable
- `Test()`: Test method

### ProcessExtensions

```csharp
extension(Process)
```

**Methods:**

- `InvokeAsync(string command)`: Invoke command asynchronously

### ThreadPoolExtensions

```csharp
extension(Task)
extension<T>(Task<T>)
extension(ValueTask)
extension<T>(ValueTask<T>)
extension(ConfiguredTaskAwaitable)
extension<T>(ConfiguredTaskAwaitable<T>)
extension(ConfiguredValueTaskAwaitable)
extension<T>(ConfiguredValueTaskAwaitable<T>)
```

**Methods:**

- `AsTaskRun()`: Ensure execution on thread pool

### XxHash3Extensions

```csharp
extension(XxHash3)
```

**Methods:**

- `Combine<T1>(in T1 value1)`: Combine hash of 1 value
- `Combine<T1, T2>(in T1 value1, in T2 value2)`: Combine hash of 2 values
- `Combine<T1, T2, T3>(...)`: Combine hash of 3 values
- `Combine<T1, T2, T3, T4>(...)`: Combine hash of 4 values
- `Combine<T1, T2, T3, T4, T5>(...)`: Combine hash of 5 values
- `CombineSpan<T>(ReadOnlySpan<T> values)`: Combine hash of span
- `Builder()`: Create XxHash3Builder

**Usage:**

```csharp
var hash = XxHash3.Combine(1, 2, 3);
var hashFromSpan = XxHash3.CombineSpan(values);
```

### SmartParallel

A static class for parallel execution:

**Methods:**

- `ForEachAsync<T>(IAsyncEnumerable<T>, CancellationToken, Func<T, CancellationToken, ValueTask>)`: Async foreach
- `ForEachAsync<T>(IEnumerable<T>, CancellationToken, Func<T, CancellationToken, ValueTask>)`: Async foreach
- `ForEach<T>(IEnumerable<T>, CancellationToken, Action<T, CancellationToken>)`: Sync foreach
- `ForEachAsync<T>(T[], CancellationToken, Func<T, CancellationToken, ValueTask>)`: Array async foreach
- `ForEach<T>(T[], CancellationToken, Action<T, CancellationToken>)`: Array sync foreach
- `ForEachAsync<T>(Memory<T>, ...)`: Memory async foreach
- `ForEach<T>(Memory<T>, ...)`: Memory sync foreach
- `ForAsync(int from, int to, CancellationToken, Func<int, CancellationToken, ValueTask>)`: Async for loop
- `For(int from, int to, CancellationToken, Action<int, CancellationToken>)`: Sync for loop

**Usage:**

```csharp
await SmartParallel.ForEachAsync(items, cancellationToken, async (item, token) =>
{
    await ProcessAsync(item, token);
});

SmartParallel.For(0, 100, CancellationToken.None, (index, token) =>
{
    Process(index);
});
```

### ScoreSortExtensions

```csharp
extension<T>(Memory<T> memory) where T : notnull
extension<T>(Span<T> input) where T : IScoreSortItem
```

**Methods:**

- `AsScoreSort()`: Convert to ScoreSortComparer
- `TakeBestScoreSort(int depth, ComparisonBuilder<...>)`: Get best sorted items
- `ScoreSort(ComparisonBuilder<...>)`: Sort by score

---

## Design Principles

1. **Separation of Concerns**: Clear distinction between abstraction and implementation
2. **Type Safety**: Compile-time checking of configuration options
3. **Extensibility**: Designed to be extended for various configuration scenarios
4. **Consistency**: Uniform patterns across all extension libraries
5. **Dependency Injection First**: Built around .NET's built-in dependency injection
6. **Performance**: Optimized for high-performance scenarios using ref structs and spans

---

## Usage Example

```csharp
// Define your options class
public class MyOptions : IOptionsBase
{
    public static string Key => "MySection";
    public string Setting1 { get; set; }
    public int Setting2 { get; set; }
}

// Define your builder interface
public interface IMyBuilder : IBuilderBase<MyOptions>
{
    // Additional builder methods can be added here
}

// Using Optional
var result = Optional<string>.From("value");
if (result.HasValue)
{
    Console.WriteLine(result.Value);
}

// Using MemoryList
var list = new MemoryList<int>();
list.Add(1);
list.Add(2);

// Using XxHash3 for hashing
var hash = XxHash3.Combine(1, 2, 3);

// Using SmartParallel
await SmartParallel.ForEachAsync(items, token, async (item, t) => await ProcessAsync(item, t));
```

---

## Architecture

The project follows these architectural principles:

- **Minimal Dependencies**: Only essential dependencies are included
- **Interface Segregation**: Small, focused interfaces for each responsibility
- **Extension Friendly**: Designed to be extended by other projects in the ecosystem
- **Testable**: Interfaces make it easy to create test doubles for unit testing
- **High Performance**: Uses ref structs, spans, and memory for maximum performance
