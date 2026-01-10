# AlirezaMahDev.Extensions

## Project Description

This project serves as the core implementation of the builder pattern infrastructure for .NET applications. It provides
a set of base classes that enable a fluent, type-safe configuration pattern for .NET's dependency injection system, with
built-in support for options pattern and hierarchical configuration.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.Abstractions**: Defines the core interfaces and contracts for the builder pattern
  implementation.

### NuGet Dependencies

- **Microsoft.Extensions.Hosting**: Provides the hosting infrastructure for .NET applications.
- **Microsoft.Extensions.Http**: Implements HTTP client factory and related services.
- **Microsoft.Extensions.Http.Polly**: Integrates Polly for HTTP resilience patterns.
- **Microsoft.Extensions.Options.ConfigurationExtensions**: Enables configuration binding for options pattern.
- **Microsoft.Extensions.Caching.Memory**: Provides in-memory caching capabilities.
- **Microsoft.Extensions.ObjectPool**: Offers object pooling for performance optimization.
- **Polly.Extensions**: Extends the Polly resilience framework with additional functionality.
- **Polly.RateLimiting**: Adds rate limiting capabilities to the resilience pipeline.

## Key Components

### BuilderBase Class Hierarchy

1. **BuilderBase**
    - Base class providing access to the service collection
    - Serves as the foundation for all builder implementations

2. **BuilderBase<TOptions>**
    - Extends BuilderBase with options configuration support
    - Automatically handles configuration binding using Microsoft.Extensions.Options
    - Supports hierarchical configuration through configuration keys
    - Provides methods for adding and configuring sub-options

3. **BuilderBase<TOptions, TParent, TParentOptions>**
    - Enables building hierarchical configuration structures
    - Maintains parent-child relationships between builders
    - Automatically manages configuration key hierarchies

### Core Features

- **Fluent API**: Enables method chaining for clean, readable configuration code
- **Type Safety**: Compile-time checking of configuration options
- **Dependency Injection**: Seamless integration with .NET's built-in DI container
- **Configuration Binding**: Automatic binding of configuration to strongly-typed options
- **Hierarchical Configuration**: Support for nested configuration structures
- **Options Validation**: Built-in support for options validation
- **Service Registration**: Simplified service registration with common patterns

## Usage Example

```csharp
// Example of creating a hierarchical configuration
services.AddMyService()
    .Configure(options => 
    {
        options.BaseUrl = "https://api.example.com";
    })
    .AddFeatureA()
        .Configure(options => 
        {
            options.FeatureAOption = "Value";
        })
    .AddFeatureB()
        .Configure(options =>
        {
            options.FeatureBOption = 42;
        });
```

## Architecture

The project follows a clean architecture approach where:

- Core abstractions are defined in the Abstractions project
- This project provides concrete implementations of those abstractions
- Dependencies are minimal and focused on the core functionality
- Extension methods are provided for common scenarios to improve developer experience