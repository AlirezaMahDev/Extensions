# AlirezaMahDev.Extensions.File.Data

## Project Description

This project provides a high-performance, file-based data storage solution for .NET applications. It offers a low-level
API for managing structured binary data with efficient read/write operations, built on top of the file access
abstractions provided by `AlirezaMahDev.Extensions.File`.

## Key Features

- **Binary Data Management**: Efficient storage and retrieval of structured binary data
- **Thread-Safe Operations**: Built-in support for concurrent access
- **Memory-Mapped Files**: Utilizes memory mapping for high-performance I/O
- **Type-Safe Access**: Safe access to data through generic methods
- **Dependency Injection**: Seamless integration with .NET's DI container
- **Automatic Resource Management**: Proper disposal of file handles and resources

## Core Components

### DataService

Main service for accessing data storage:

- `Default`: Gets the default data access instance
- `Access(string name)`: Gets or creates a named data access instance

### DataAccess

Core class for data operations:

- `Block`: Provides access to the underlying data blocks
- `FileAccess`: Underlying file access instance
- `Root`: Root data location
- `GenerateId(int length)`: Generates unique identifiers for data storage

### DataBlock

Manages blocks of data in the file:

- `Read<T>(long offset)`: Reads a value of type T from the specified offset
- `Write<T>(long offset, T value)`: Writes a value of type T to the specified offset
- `Allocate(int size)`: Allocates a new block of the specified size

### DataLocation

Represents a location in the data file:

- `Read<T>()`: Reads a value from this location
- `Write<T>(T value)`: Writes a value to this location
- `Next`: Gets or sets the next location in the chain

## Usage Example

### Registration

```csharp
// In Program.cs or Startup.cs
services.AddFileData();

// Or with custom configuration
services.AddFileData(builder =>
{
    builder.Configure<FileOptions>(options =>
    {
        options.Path = "./App_Data";
    });
});
```

### Basic Usage

```csharp
public class MyService
{
    private readonly IDataService _dataService;

    public MyService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public void StoreData(string name, byte[] data)
    {
        var dataAccess = _dataService.Access("mydata");
        
        // Allocate space for the data
        var location = dataAccess.Block.Allocate(data.Length);
        
        // Write the data
        dataAccess.Block.WriteBytes(location, data);
        
        // Store the location for later retrieval
        // (e.g., in a dictionary or database)
        StoreLocation(name, location);
    }

    public byte[] RetrieveData(string name)
    {
        var location = GetLocation(name);
        if (location == 0) return null;
        
        var dataAccess = _dataService.Access("mydata");
        return dataAccess.Block.ReadBytes(location);
    }
}
```

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.File.Data.Abstractions**: Defines the core interfaces
- **AlirezaMahDev.Extensions.File**: Provides file access functionality
- **AlirezaMahDev.Extensions**: Core extension infrastructure

## Performance Considerations

- **Memory Mapping**: Uses memory-mapped files for efficient I/O
- **Unsafe Code**: Utilizes unsafe code for direct memory access when needed
- **Thread Safety**: Implements proper synchronization for concurrent access
- **Buffer Management**: Efficient buffer management for read/write operations

## Error Handling

The implementation handles various error scenarios:

- File access errors
- Invalid data formats
- Resource disposal issues
- Concurrency conflicts

## Best Practices

1. **Always dispose**: Use `using` statements or `IAsyncDisposable` for proper cleanup
2. **Handle exceptions**: Implement proper error handling for file operations
3. **Consider concurrency**: Be aware of potential concurrency issues in multi-threaded scenarios
4. **Monitor file size**: Be mindful of disk space when storing large amounts of data
5. **Use appropriate data types**: Choose the most efficient data types for your use case

## Configuration

### FileOptions

- `Path`: The base directory for storing data files (default: `./.data`)

### Example Configuration

```json
{
  "File": {
    "Path": "./App_Data/Storage"
  }
}
```

## Advanced Usage

### Working with Custom Types

```csharp
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MyData
{
    public int Id;
    public long Timestamp;
    public double Value;
}

// Writing data
var data = new MyData { Id = 1, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Value = 42.0 };
var location = dataAccess.Block.Allocate(Unsafe.SizeOf<MyData>());
dataAccess.Block.Write(location, data);

// Reading data
var readData = dataAccess.Block.Read<MyData>(location);
```

### Implementing Custom Data Structures

You can build higher-level data structures on top of the low-level block storage:

```csharp
public class CustomDataStructure
{
    private readonly IDataAccess _dataAccess;
    private long _rootLocation;

    public CustomDataStructure(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
        _rootLocation = _dataAccess.Block.Allocate(/* size of root structure */);
    }

    // Implement your data structure operations here
}
```