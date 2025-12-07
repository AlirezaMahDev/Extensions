# AlirezaMahDev.Extensions.File.Abstractions

## Project Description

This project provides a set of abstractions for file-based operations in the AlirezaMahDev.Extensions ecosystem. It defines interfaces for file access, service management, and configuration, enabling consistent and type-safe file handling across applications.

## Key Features

- **Unified File Access**: Standardized interface for file operations
- **Thread-Safe Operations**: Built-in support for concurrent access
- **Flexible Configuration**: Extensible options for file handling
- **Asynchronous Support**: Full async/await pattern support
- **Stream-Based Operations**: Efficient handling of file streams
- **Dependency Injection**: Seamless integration with .NET's DI container

## Core Components

### IFileService

Main service interface for accessing files:
- `Access(string name)`: Gets a file accessor for the specified file

### IFileAccess

Provides thread-safe access to file operations:
- `Access()`: Performs read operations on the file
- `Replace()`: Replaces the entire file content
- `Change()`: Modifies file content with a transformation
- Async variants of all methods available

### IFileBuilder

Fluent interface for configuring file services:
- Extends `IBuilderBase<FileOptions>`
- Used with dependency injection to configure file handling

### FileOptions

Configuration options for file handling:
- `Path`: Base directory for file operations (defaults to `./.data`)

## Usage Example

```csharp
// Register file services
services.AddFile(builder =>
{
    builder.OptionsBuilder.Configure(options =>
    {
        options.Path = "./App_Data";
    });
});

// Resolve and use
var fileService = serviceProvider.GetRequiredService<IFileService>();

// Access a file
var fileAccess = fileService.Access("data.json");

// Read from file
var content = fileAccess.Access(stream => 
{
    using var reader = new StreamReader(stream);
    return reader.ReadToEnd();
});

// Write to file
fileAccess.Replace(stream => 
{
    using var writer = new StreamWriter(stream);
    writer.Write("New content");
});
```

## Dependencies

### Project Dependencies
- **AlirezaMahDev.Extensions.Abstractions**: Core extension infrastructure

- **AlirezaMahDev.Extensions.Abstractions**: Uses the base builder and options interfaces to maintain consistency with the broader framework

**Why this dependency?** The file abstractions follow the same builder and configuration patterns established by the core abstractions, ensuring a unified API experience.

## Key Components

### IFileService

Defines the interface for managing file access. Provides methods to obtain file access objects for specific files.

### IFileAccess

Represents access to a specific file, providing methods to read, write, and manipulate file content.

### IFileBuilder

Defines the builder pattern for configuring file services within the dependency injection container.

### FileOptions

Configuration options for file services, implementing `IOptionsBase` to integrate with the configuration system.

These abstractions enable testable, mockable, and consistent file operations throughout the application.