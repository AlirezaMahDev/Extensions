# AlirezaMahDev.Extensions.DataManager.Abstractions

## Project Description

This project provides a set of abstractions for efficient data management in .NET applications. It offers a low-level, high-performance data access layer with support for memory-mapped files, data location tracking, and efficient data storage and retrieval. The library is designed to be extensible and can be used as a foundation for building custom data management solutions.

## Key Features

- **Data Location Management**: Track and manage data locations with precise control over memory and storage.
- **Type-Safe Data Access**: Generic interfaces for type-safe data operations with support for custom value types.
- **Memory Management**: Efficient memory allocation and deallocation with support for large data sets.
- **Asynchronous Operations**: Full support for asynchronous data access patterns.
- **Thread Safety**: Built-in support for concurrent access with locking mechanisms.
- **Extensible Design**: Interfaces and base classes that can be extended for custom storage backends.

## Core Interfaces

### IDataAccess

The main interface for data access operations:
- `ReadMemory`/`ReadMemoryAsync`: Read data from a specific offset
- `WriteMemory`/`WriteMemoryAsync`: Write data to a specific offset
- `AllocateOffset`: Allocate space for new data
- `GetRoot`/`GetTrash`: Access root and trash locations
- `Lock`/`UnLock`: Thread synchronization

### IDataLocation

Represents a location in the data store:
- `Offset`: The position in the data store
- `Length`: The size of the data
- `Access`: Reference to the underlying data access
- `Memory`: Direct memory access to the data

### Data Structures

- **DataPath**: Represents a path in the data store
- **DataTrash**: Manages deleted data locations for reuse
- **String Types**: Optimized fixed-size string types (String16, String32, String64, String128, String256)
- **Collections**: Interfaces for data collections and dictionaries

## Usage Example

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

## Design Principles

1. **Performance**: Optimized for high-throughput, low-latency data access
2. **Type Safety**: Strongly-typed interfaces prevent common errors
3. **Extensibility**: Designed to be extended for various storage backends
4. **Memory Efficiency**: Minimizes allocations and copies
5. **Thread Safety**: Built-in support for concurrent access

## Dependencies

- **.NET 10.0**: Base framework requirements
- **System.Memory**: For efficient memory operations

## Architecture

The project follows a layered architecture:
- **Abstraction Layer**: Defines core interfaces and contracts
- **Data Types**: Implements value types and data structures
- **Extensions**: Provides utility methods and extensions

## Implementation Notes

- The library uses `unmanaged` constraints for value types to ensure memory safety
- All string types are fixed-size for predictable memory layout
- The data location system enables efficient data tracking and management
- The trash collection system helps with memory reuse and fragmentation prevention