# AlirezaMahDev.Extensions.DataManager

## Project Description

This project provides a high-performance, file-based data management system for .NET applications. It implements the abstractions defined in `AlirezaMahDev.Extensions.DataManager.Abstractions` to offer a concrete, efficient data storage solution. The library is designed for scenarios requiring fast, reliable, and concurrent access to structured data with minimal overhead.

## Key Features

- **File-based Storage**: Efficient storage of structured data in files
- **Concurrent Access**: Thread-safe operations for multi-threaded scenarios
- **Memory Management**: Smart caching and memory optimization
- **Temporary Data Support**: Specialized handling for temporary data
- **Resource Management**: Proper disposal of resources with `IDisposable`
- **Asynchronous Operations**: Full async support for all I/O operations

## Core Components

### IDataManager

The main interface for managing data access:
- `Open`: Opens or creates a data store with the specified key
- `Close`: Closes and cleans up resources for a specific data store
- `OpenTemp`: Creates a temporary data store for short-lived data

### DataAccess

Implements `IDataAccess` to provide:
- Memory-mapped file operations
- Thread-safe concurrent access
- Efficient data caching
- Resource management

### DataManager

The main implementation of `IDataManager` that:
- Manages multiple data stores
- Handles resource cleanup
- Implements the builder pattern for configuration

## Usage Example

```csharp
// Configure services
services.AddDataManager(options =>
{
    options.DirectoryPath = "Data";
    options.FileFormat = "{0}.data";
});

// Resolve and use
using var scope = serviceProvider.CreateScope();
var dataManager = scope.ServiceProvider.GetRequiredService<IDataManager>();

// Open a data store
using var dataAccess = dataManager.Open("mydata");

// Get or create root location
var root = dataAccess.GetRoot();

// Work with data...
```

## Configuration

The `DataManager` can be configured using `DataManagerOptions`:

```csharp
services.AddDataManager(options =>
{
    options.DirectoryPath = "App_Data";  // Base directory for data files
    options.FileFormat = "{0}.dat";      // File naming pattern
});
```

## Dependencies

### Project Dependencies
- **AlirezaMahDev.Extensions.DataManager.Abstractions**: Core interfaces
- **AlirezaMahDev.Extensions**: Extension infrastructure

### NuGet Dependencies
- **System.IO.Hashing**: For checksum and hash operations

## Performance Considerations

- **Caching**: Implements in-memory caching of frequently accessed data
- **Lazy Loading**: Resources are loaded on demand
- **Concurrent Access**: Uses thread-safe collections for concurrent operations
- **Efficient I/O**: Utilizes `RandomAccess` for high-performance file operations

## Error Handling

The library throws appropriate exceptions for error conditions:
- `ArgumentNullException` for null arguments
- `ObjectDisposedException` for operations on disposed objects
- `IOException` for I/O related errors

## Thread Safety

All public members of `DataManager` and `DataAccess` are thread-safe and may be used concurrently from multiple threads.

## Resource Management

All types that implement `IDisposable` should be properly disposed to release system resources. The recommended pattern is to use the `using` statement:

```csharp
using var dataAccess = dataManager.Open("mydata");
// Use dataAccess here
```

## Implementation Notes

- Uses memory-mapped files for efficient data access
- Implements a simple but effective caching mechanism
- Designed for both small and large data sets
- Supports both synchronous and asynchronous operations