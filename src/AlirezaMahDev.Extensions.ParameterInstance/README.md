# AlirezaMahDev.Extensions.ParameterInstance

## Project Description

This project provides a high-performance, thread-safe implementation of parameter-based instance factories. It enables
efficient creation, caching, and management of object instances based on parameter values, significantly improving
performance in scenarios where object creation is expensive or where instance reuse is desired.

## Key Features

- **Thread-Safe Caching**: Implements a concurrent dictionary for safe multi-threaded access
- **Lazy Initialization**: Creates instances only when first requested
- **Resource Management**: Automatic cleanup of disposable resources
- **Dependency Injection**: Seamless integration with .NET's DI container
- **Flexible Configuration**: Customizable caching and instance creation behaviors
- **High Performance**: Optimized for low-latency and high-throughput scenarios

## Core Components

### ParameterInstanceFactory<TInstance, TParameter>

The main factory class that implements `IParameterInstanceFactory<TInstance, TParameter>`:

- **Thread-Safe Operations**: Uses `ConcurrentDictionary` for thread-safe instance caching
- **Lazy Initialization**: Implements lazy instantiation with `Lazy<T>`
- **Resource Management**: Automatically disposes `IDisposable` and `IAsyncDisposable` instances
- **Dependency Injection**: Uses `ActivatorUtilities` for service resolution
- **Instance Management**: Methods to remove or clear cached instances

### ParameterInstanceFactoryExtensions

Extension methods that simplify working with the factory in a DI container:

- `AddParameterInstanceFactory()`: Registers the factory with the service collection
- `GetOrAddParameterInstance()`: Helper method to get or create instances
- `TryRemoveParameterInstance()`: Helper method to remove instances from cache

## Usage Example

### Registration

```csharp
// In Startup.cs or Program.cs
services.AddParameterInstanceFactory<UserContext, UserSession>(
    options =>
    {
        // Configure factory options
        options.CacheSize = 1000;
        options.EnableAutoCleanup = true;
    },
    (provider, context) => 
    {
        // Create and configure the instance
        var logger = provider.GetRequiredService<ILogger<UserSession>>();
        return new UserSession(context.UserId, context.TenantId, logger);
    });
```

### Usage in Application

```csharp
public class UserRequestProcessor
{
    private readonly IParameterInstanceFactory<UserSession, UserContext> _sessionFactory;

    public UserRequestProcessor(
        IParameterInstanceFactory<UserSession, UserContext> sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task ProcessRequest(UserContext context)
    {
        // Get or create a session for the user context
        // This will create the session only if it doesn't exist in the cache
        var session = _sessionFactory.GetOrCreate(context);
        
        try
        {
            // Use the session...
            await session.ProcessRequestAsync();
        }
        finally
        {
            // Optionally remove the session if it's no longer needed
            _sessionFactory.TryRemove(context);
        }
    }
}
```

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.ParameterInstance.Abstractions**: Defines the core interfaces
- **AlirezaMahDev.Extensions**: Provides base builder patterns

### Why These Dependencies?

- **ParameterInstance.Abstractions**: Provides the contract interfaces that this project implements
- **Extensions**: Supplies the builder infrastructure for DI integration

## Performance Considerations

- **Memory Usage**: The factory maintains a cache of instances, so be mindful of memory consumption
- **Thread Safety**: All operations are thread-safe, but consider the thread safety of the instances being created
- **Lifetime Management**: Instances are kept in memory until explicitly removed or the factory is disposed

## Best Practices

1. **Use for Expensive Objects**: Best suited for objects that are expensive to create
2. **Monitor Cache Size**: Keep an eye on the cache size to prevent memory leaks
3. **Clean Up**: Explicitly remove instances when they're no longer needed
4. **Thread Safety**: Ensure that the instances you create are thread-safe if they'll be accessed from multiple threads
5. **Disposable Instances**: Always implement `IDisposable` or `IAsyncDisposable` for resources that need cleanup

## Implementation Details

The factory uses a combination of `ConcurrentDictionary` and `Lazy<T>` to ensure thread-safe lazy initialization. When
an instance is requested:

1. The factory checks if an instance exists for the given parameters
2. If not, it creates a new `Lazy<T>` instance that will create the actual object when needed
3. The `Lazy<T>` ensures that the object is created only once, even if multiple threads request it simultaneously
4. The instance is stored in the cache for future use
5. When the factory is disposed, it disposes of all disposable instances in the cache