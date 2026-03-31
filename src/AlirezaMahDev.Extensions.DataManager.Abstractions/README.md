# AlirezaMahDev.Extensions.DataManager.Abstractions

## Project Description

This project provides a set of abstractions for efficient data management in .NET applications. It offers a low-level, high-performance data access layer with support for memory-mapped files, data location tracking, and efficient data storage and retrieval. The library is designed to be extensible and can be used as a foundation for building custom data management solutions.

---

## Core Interfaces

### IDataAccess

The main interface for data access operations:

- `ReadMemory`/`ReadMemoryAsync`: Read data from a specific offset
- `WriteMemory`/`WriteMemoryAsync`: Write data to a specific offset
- `AllocateOffset`: Allocate space for new data
- `GetRoot`/`GetTrash`: Access root and trash locations
- `Lock`/`UnLock`: Thread synchronization
- `FileId`: The file identifier

### IDataLocation

Represents a location in the data store:

- `Offset`: The position in the data store
- `Length`: The size of the data
- `Access`: Reference to the underlying data access
- `Memory`: Direct memory access to the data
- `RefValue`: Direct reference to the value

### IDataCollection

Represents a collection of data items.

### IDataDictionary

Represents a dictionary-like data structure.

---

## Data Structures (Structs)

### DataOffset

```csharp
public readonly struct DataOffset(int fileId, int partIndex, int offset, int length)
    : IScopedRefReadOnlyEquatable<DataOffset>, IScopedInEqualityOperators<DataOffset, DataOffset, bool>
```

Represents a location in the data store with file, part, offset, and length information.

**Properties:**

- `FileId`: File identifier
- `PartIndex`: Part index within the file
- `Offset`: Position in the data store
- `Length`: Size of the data

**Methods:**

- `IsNull`: Check if offset is null/empty
- `IsNotNull`: Check if offset is valid

**Usage:**

```csharp
var offset = new DataOffset(fileId: 1, partIndex: 0, offset: 1024, length: 64);
var isValid = offset.IsNotNull;
```

### DataLocation<TValue>

```csharp
public readonly struct DataLocation<TValue>(DataOffset offset, IDataMapFilePartOwner owner)
    : IScopedRefReadOnlyEquatable<DataLocation<TValue>>
```

Generic wrapper for data location with type safety.

**Properties:**

- `Offset`: The data offset
- `Owner`: The owner/manager
- `RefValue`: Direct reference to the value (get/set)
- `RefReadOnlyValue`: Read-only reference

**Static Methods:**

- `Create(IDataAccess access, TValue value)`: Create a new data location
- `Read(IDataAccess access, DataOffset offset)`: Read from existing offset

**Usage:**

```csharp
// Create new location
var location = DataLocation<MyValueType>.Create(dataAccess, default);
location.RefValue = new MyValueType { /* ... */ };

// Read existing location
var readLocation = DataLocation<MyValueType>.Read(dataAccess, location.Offset);
var value = readLocation.RefValue;
```

### DataLocationWrap<TValue, TWrap>

```csharp
public readonly struct DataLocationWrap<TValue, TWrap>(IDataAccess access, DataLocation<TValue> location, TWrap wrap)
    where TValue : unmanaged, IDataValue<TValue>
```

A wrapper combining location with additional wrap data.

**Properties:**

- `Access`: Data access instance
- `Location`: The data location
- `Wrap`: The wrap data

### String Types

Fixed-size string types for efficient memory storage:

#### String16, String32, String64, String128, String256

```csharp
public readonly struct String16 : IScopedRefReadOnlyEquatable<String16>, IString<String16>
public readonly struct String32 : IScopedRefReadOnlyEquatable<String32>, IString<String32>
public readonly struct String64 : IScopedRefReadOnlyEquatable<String64>, IString<String64>
public readonly struct String128 : IScopedRefReadOnlyEquatable<String128>, IString<String128>
public readonly struct String256 : IScopedRefReadOnlyEquatable<String256>, IString<String256>
```

**Properties:**

- `Length`: Current length of the string
- `Span`: Span to the string data

**Methods:**

- `Clear()`: Clear the string
- `Write(ReadOnlySpan<char> value)`: Write a new value
- `TryWrite(...)`: Try to write with overflow check
- `AsSpan()`: Get as ReadOnlySpan
- `ToString()`: Convert to string

**Usage:**

```csharp
var str = new String64();
str.Write("Hello");
var length = str.Length;
var span = str.Span;
```

### DataPath

Represents a path in the data store.

### DataTrash

Manages deleted data locations for reuse.

---

## Extension Methods

The project uses **C# 12 Extension Methods** with the `extension` keyword.

### DataLocationExtensions

```csharp
extension<TValue>(DataLocation<TValue> location) where TValue : unmanaged, IDataValue<TValue>
```

**Methods:**

- `Write(TValue value)`: Write value to location
- `Read()`: Read value from location
- `AsReadOnly()`: Get as read-only

**Usage:**

```csharp
var location = DataLocation<MyValueType>.Create(access, default);
location.Write(myValue);
var value = location.Read();
```

### DataLocationStorageExtensions

```csharp
extension<TValue>(in DataLocation<TValue> location) where TValue : unmanaged, IDataValue<TValue>
```

**Methods:**

- `Storage`: Get storage info
- `IsStored`: Check if data is stored

**Usage:**

```csharp
var isStored = location.IsStored;
var storage = location.Storage;
```

### DataCollectionWrapExtensions

```csharp
extension<TValue>(DataCollectionWrap<TValue> collection)
```

**Methods:**

- `FirstOrDefault(...)`: Find first item
- `Any(...)`: Check if collection has items
- `Count`: Get item count

**Usage:**

```csharp
var firstItem = collection.FirstOrDefault(x => x.Id == targetId);
var hasItems = collection.Any();
```

### DataLockWrapExtensions

```csharp
extension<TValue>(DataLocation<TValue> location)
```

**Methods:**

- `ReadLock(...)`: Read with lock
- `WriteLock(...)`: Write with lock

**Usage:**

```csharp
using var locked = location.ReadLock();
var value = locked.RefReadOnlyValue;
```

### DataDictionaryWrapExtensions

```csharp
extension<TKey, TValue>(DataDictionaryWrap<TKey, TValue> dictionary)
```

**Methods:**

- `TryGet(...)`: Try to get value by key
- `TryAdd(...)`: Try to add key-value pair
- `Remove(...)`: Remove entry

**Usage:**

```csharp
if (dictionary.TryGet(key, out var value))
{
    // Use value
}
```

### DataCollectionItemWrapExtensions

```csharp
extension<TValue>(DataCollectionItemWrap<TValue> item)
```

**Properties:**

- `Value`: The item value
- `Next`: Next item in collection

### DataEmptyWrapExtensions

```csharp
extension(DataEmptyWrap empty)
```

**Properties:**

- `IsEmpty`: Check if wrap is empty
- `Value`: Get empty value

### DataLocationDataTrashExtensions

```csharp
extension<TValue>(DataLocation<TValue> location)
```

**Methods:**

- `MoveToTrash()`: Move to trash for reuse

### DataLocationWrapExtensions

```csharp
extension<TValue>(DataLocation<TValue> location)
```

**Methods:**

- `AsWrap(...)`: Convert to wrapped version

---

## Usage Examples

### Basic Data Access

```csharp
// Create a data access instance
IDataAccess dataAccess = /* implementation */;

// Allocate and write data
var location = DataLocation<MyValueType>.Create(dataAccess, default);
location.RefValue = new MyValueType { /* ... */ };

// Read data
var readLocation = DataLocation<MyValueType>.Read(dataAccess, location.Offset);
var value = readLocation.RefValue;
```

### Using String Types

```csharp
var name = new String64();
name.Write("Product Name");
Console.WriteLine(name.ToString());
```

### Thread-Safe Operations

```csharp
// Use read lock for thread-safe read
using var readLock = location.ReadLock();
var value = readLock.RefReadOnlyValue;

// Use write lock for thread-safe write
using var writeLock = location.WriteLock();
writeLock.RefValue = newValue;
```

### Collection Operations

```csharp
var item = collection.FirstOrDefault(x => x.Id == targetId);
if (item.HasValue)
{
    var value = item.Value.RefValue;
}
```

---

## Design Principles

1. **Performance**: Optimized for high-throughput, low-latency data access
2. **Type Safety**: Strongly-typed interfaces prevent common errors
3. **Extensibility**: Designed to be extended for various storage backends
4. **Memory Efficiency**: Minimizes allocations and copies
5. **Thread Safety**: Built-in support for concurrent access
6. **Value Types**: Uses `unmanaged` constraints for maximum performance

---

## Dependencies

- **.NET 10.0**: Base framework requirements
- **System.Memory**: For efficient memory operations

---

## Architecture

The project follows a layered architecture:

- **Abstraction Layer**: Defines core interfaces and contracts
- **Data Types**: Implements value types and data structures
- **Extensions**: Provides utility methods and extensions

---

## Implementation Notes

- The library uses `unmanaged` constraints for value types to ensure memory safety
- All string types are fixed-size for predictable memory layout
- The data location system enables efficient data tracking and management
- The trash collection system helps with memory reuse and fragmentation prevention
- All operations support both synchronous and asynchronous patterns
