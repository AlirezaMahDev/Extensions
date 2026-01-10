# AlirezaMahDev.Extensions.ParameterInstance.Abstractions

## Project Description

This project provides a set of abstractions for implementing parameter-based instance factories in .NET applications. It
defines interfaces for creating, caching, and managing object instances based on parameter values, enabling efficient
resource utilization and instance reuse patterns.

## Key Features

- **Parameter-based instance creation**: Create and manage instances based on parameter values
- **Thread-safe instance caching**: Built-in support for thread-safe instance caching
- **Flexible lifecycle management**: Control how instances are created, cached, and disposed
- **Type-safe generic interfaces**: Strongly-typed APIs for better developer experience
- **Dependency injection ready**: Designed to work seamlessly with .NET's DI container

## Core Components

### IParameterInstanceFactory<TParameter>

The base interface for parameter-based instance factories:

- `Options`: Gets the configuration options for the factory
- `TryRemove(TParameter)`: Attempts to remove an instance from the cache

### IParameterInstanceFactory<TInstance, TParameter>

Extends the base interface with instance management capabilities:

- `GetOrCreate(TParameter)`: Gets an existing instance or creates a new one if it doesn't exist
- Implements `IEnumerable<TInstance>` for enumerating all created instances

### ParameterInstanceFactoryOptions

Configuration options for parameter instance factories:

- Cache management settings
- Instance lifetime policies
- Factory behavior customization

## Usage Example

```csharp
// Define your parameter type
public record UserContext(string UserId, string TenantId);

// Register the factory in DI
services.AddParameterInstanceFactory<UserContext, UserSession>(
    options =>
    {
        // Configure factory options
        options.CacheSize = 1000;
    },
    (provider, context) => new UserSession(context.UserId, context.TenantId));

// Resolve and use the factory
public class UserService
{
    private readonly IParameterInstanceFactory<UserSession, UserContext> _sessionFactory;

    public UserService(IParameterInstanceFactory<UserSession, UserContext> sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task ProcessUserRequest(UserContext context)
    {
        // Get or create a session for the user context
        var session = _sessionFactory.GetOrCreate(context);
        
        // Use the session...
    }
}
```

## Dependencies

- **.NET 10.0**: Base framework requirements

## Design Principles

1. **Type Safety**: Strongly-typed interfaces prevent common errors
2. **Extensibility**: Designed to be extended for various caching and lifecycle scenarios
3. **Performance**: Efficient instance management with minimal overhead
4. **Thread Safety**: Built-in support for concurrent access
5. **Dependency Injection**: First-class support for .NET's DI container

## When to Use

This library is particularly useful when you need to:

- Cache expensive-to-create objects based on parameters
- Manage the lifecycle of parameter-dependent instances
- Implement the flyweight pattern
- Create thread-safe instance caches
- Implement multi-tenant services where each tenant gets its own instance

## Implementation Notes

- The factory pattern implemented here is particularly useful for scenarios where object creation is expensive
- The cache is automatically managed by the factory based on the provided options
- Implementations should ensure thread safety when creating and accessing instances