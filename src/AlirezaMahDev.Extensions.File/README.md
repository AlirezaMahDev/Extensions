# AlirezaMahDev.Extensions.File

## Project Description

This project provides a robust implementation of file access operations for the AlirezaMahDev.Extensions ecosystem. It
implements the interfaces defined in `AlirezaMahDev.Extensions.File.Abstractions`, offering thread-safe,
high-performance file handling with support for dependency injection and configuration.

## Key Features

- **Thread-Safe File Operations**: Implements thread-safe file access with proper locking mechanisms
- **Dependency Injection**: Seamless integration with .NET's DI container
- **Configuration Support**: Flexible configuration through `FileOptions`
- **Resource Management**: Proper disposal of file handles and resources
- **Asynchronous Support**: Full async/await pattern implementation
- **File Locking**: Ensures data integrity with proper file locking

## Core Components

### FileService

Implements `IFileService` to provide file access instances:

- `Access(string name)`: Gets or creates a file accessor for the specified file
- Thread-safe and singleton-scoped by default

### FileAccess

Implements `IFileAccess` to provide thread-safe file operations:

- `Access()`: Performs read operations on the file
- `Replace()`: Replaces the entire file content atomically
- `Change()`: Modifies file content with a transformation
- Implements `IDisposable` and `IAsyncDisposable` for proper resource cleanup

### FileBuilder

Implements `IFileBuilder` for configuring file services:

- Configures the file storage directory
- Registers required services with the DI container
- Ensures the storage directory exists

## Usage Example

### Registration

```csharp
// In Startup.cs or Program.cs
services.AddFile(builder =>
{
    builder.OptionsBuilder.Configure(options =>
    {
        options.Path = "./App_Data/Files";
    });
});
```

### Usage in Application

```csharp
public class MyService
{
    private readonly IFileService _fileService;

    public MyService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<string> ReadFileContentAsync(string fileName)
    {
        var fileAccess = _fileService.Access(fileName);
        return await fileAccess.AccessAsync(async (stream, ct) =>
        {
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync(ct);
        });
    }

    public async Task WriteFileContentAsync(string fileName, string content)
    {
        var fileAccess = _fileService.Access(fileName);
        await fileAccess.ReplaceAsync(async (stream, ct) =>
        {
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(content);
        });
    }
}
```

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.File.Abstractions**: Defines the core interfaces
- **AlirezaMahDev.Extensions.ParameterInstance**: For parameter instance management
- **AlirezaMahDev.Extensions**: Core extension infrastructure

## Performance Considerations

- **File Locking**: Uses `FileShare.None` to prevent concurrent access issues
- **Resource Management**: Implements `IDisposable` and `IAsyncDisposable` for proper cleanup
- **Thread Safety**: Uses `SemaphoreSlim` for thread synchronization
- **Caching**: Maintains file stream instances for performance

## Error Handling

The implementation handles various file system errors and provides appropriate exceptions:

- `IOException`: For file access issues
- `ObjectDisposedException`: For operations on disposed objects
- `UnauthorizedAccessException`: For permission issues

## Configuration Options

### FileOptions

- `Path`: The base directory for storing files (default: `./.data`)

Example configuration:

```json
{
  "File": {
    "Path": "./App_Data/Files"
  }
}
```

## Best Practices

1. **Always dispose**: Use `using` statements or `await using` with `IFileAccess`
2. **Handle exceptions**: Wrap file operations in try-catch blocks
3. **Use async/await**: Prefer asynchronous methods for better scalability
4. **Configure paths**: Set appropriate paths for your environment
5. **Consider file locking**: Be aware of file locking behavior in concurrent scenarios

- **AlirezaMahDev.Extensions.File.Abstractions**: Implements the file abstractions defined in this project
- **AlirezaMahDev.Extensions.ParameterInstance**: Uses parameter-based instance factories for managing file access
  instances
- **AlirezaMahDev.Extensions**: Inherits base builder implementations for consistent configuration patterns

**Why these dependencies?**

- **File.Abstractions**: Provides the contracts that this project implements
- **ParameterInstance**: Enables efficient caching and reuse of file access objects based on file paths
- **Extensions**: Provides the builder pattern infrastructure for service registration

## Key Components

### FileService

The main service implementation that manages file access objects. It uses a factory pattern to create and cache file
access instances.

### FileAccess

Concrete implementation of `IFileAccess`, providing actual file I/O operations including:

- Reading and writing file content
- File stream management
- File metadata operations

### FileBuilder

Builder implementation for configuring file services in the dependency injection container, following the standard
builder pattern.

### FileAccessFactory

Factory for creating and managing `FileAccess` instances with caching support to avoid redundant file operations.

### Extension Methods

- **FilebaseExtensions**: Extension methods for file-related operations
- **FileAccessExtensions**: Extension methods for `IFileAccess` objects to simplify common file operations