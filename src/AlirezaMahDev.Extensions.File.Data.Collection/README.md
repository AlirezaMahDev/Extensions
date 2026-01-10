# AlirezaMahDev.Extensions.File.Data.Collection

## Project Description

This project provides a high-performance, file-based collection implementation for .NET applications. It offers a
persistent, thread-safe collection that can store and retrieve objects with minimal overhead, making it ideal for
scenarios requiring fast access to large datasets with persistence.

## Key Features

- **Thread-Safe Operations**: Built on `ConcurrentDictionary` for safe concurrent access
- **Lazy Loading**: Objects are loaded on-demand for optimal performance
- **Persistence**: Data is automatically persisted to disk
- **Type Safety**: Strongly-typed API for working with collections and objects
- **Memory Efficiency**: Optimized for minimal memory usage with large datasets
- **High Performance**: Designed for low-latency operations

## Core Components

### CollectionAccess

The main entry point for working with collections, providing methods to:

- Create and manage collections
- Access collection properties
- Enumerate through collection items
- Perform batch operations

### CollectionObjects

Manages a collection of objects with features like:

- Thread-safe access to collection items
- Lazy loading of objects
- Efficient memory management
- Index-based access

### CollectionObject

Represents an individual item in the collection with:

- Property storage and retrieval
- Type-safe value access
- Change tracking
- Serialization support

### CollectionProperties

Manages metadata and properties of collections:

- Custom property definitions
- Type information
- Validation rules
- Indexing options

## Usage Example

```csharp
// Create a new collection access
var access = await CollectionAccessFactory.CreateAsync("MyCollection");

// Get or create a collection
var collection = await access.GetOrCreateCollectionAsync("MyItems");

// Add items to the collection
for (int i = 0; i < 1000; i++)
{
    var item = collection.CreateObject();
    item["Name"] = $"Item {i}";
    item["Value"] = i * 10;
    await item.SaveAsync();
}

// Query items
var items = await collection.WhereAsync(x => (int)x["Value"] > 500);
foreach (var item in items)
{
    Console.WriteLine($"{item["Name"]}: {item["Value"]}");
}
```

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.File.Data**: Core file data access functionality
- **AlirezaMahDev.Extensions.File.Data.Collection.Abstractions**: Defines the core interfaces and contracts

### Why These Dependencies?

- **File.Data**: Provides the underlying file-based storage infrastructure
- **Collection.Abstractions**: Defines the contracts that this project implements

## Performance Considerations

- **Memory Usage**: Uses lazy loading to minimize memory footprint
- **Concurrency**: Optimized for high-concurrency scenarios
- **Persistence**: Implements efficient write-behind caching for better performance
- **Indexing**: Supports custom indexes for faster lookups

## Best Practices

1. **Batch Operations**: Use batch operations for bulk inserts or updates
2. **Dispose Properly**: Always dispose of collection and access objects when done
3. **Use Async**: Prefer async methods for I/O-bound operations
4. **Monitor Memory**: Keep an eye on memory usage with large collections
5. **Indexing**: Define appropriate indexes for frequently queried properties

## Implementation Details

The implementation uses a combination of memory-mapped files and in-memory caching to provide fast access to collection
data. Collections are stored in a structured format that allows for efficient querying and updates.

### Data Structure

```
Collection/
  ├── [CollectionName]/
  │   ├── _meta.json         # Collection metadata and schema
  │   ├── objects/           # Individual object data
  │   │   ├── 0000001.bin
  │   │   ├── 0000002.bin
  │   │   └── ...
  │   └── indexes/           # Index data
  │       ├── [PropertyName].idx
  │       └── ...
  └── ...
```

## Error Handling

The library throws appropriate exceptions for error conditions:

- `CollectionNotFoundException`: When a requested collection doesn't exist
- `ObjectNotFoundException`: When an object is not found
- `InvalidOperationException`: For invalid operations
- `IOException`: For file system related errors

## Thread Safety

All public members of `CollectionAccess` and related classes are thread-safe and may be used concurrently from multiple
threads.

## Contributing

Contributions are welcome! Please ensure that any changes maintain backward compatibility and include appropriate unit
tests.