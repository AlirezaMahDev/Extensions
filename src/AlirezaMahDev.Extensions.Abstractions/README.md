# AlirezaMahDev.Extensions.Abstractions

## Project Description

This project serves as the foundation for the AlirezaMahDev.Extensions ecosystem, providing essential abstractions and interfaces that enable consistent and type-safe configuration patterns across all extension libraries. It defines the core contracts for implementing the builder pattern with dependency injection and options configuration in .NET applications.

## Dependencies

### Core Dependencies
- **Microsoft.Extensions.Hosting.Abstractions**: Provides the fundamental interfaces for .NET's hosting infrastructure.
- **JetBrains.Annotations**: Enhances code quality with static analysis attributes and code annotations.

### Utility Dependencies
- **CommunityToolkit.Common**: Offers common utility methods and extension methods.
- **CommunityToolkit.Diagnostics**: Provides guard clauses and argument validation utilities.
- **CommunityToolkit.HighPerformance**: Delivers high-performance programming utilities and optimizations.

### Resilience Dependencies
- **Polly**: A .NET resilience and transient-fault-handling library.
- **Polly.Extensions.Http**: Extends Polly with HTTP-specific resilience policies and utilities.

## Key Components

### IOptionsBase Interface

The `IOptionsBase` interface is the foundation for all configuration options in the ecosystem:

```csharp
public interface IOptionsBase
{
    static abstract string Key { get; }
}
```

**Key Features:**
- Enforces a consistent way to define configuration keys
- Supports hierarchical configuration through key composition
- Enables type-safe configuration binding

### IBuilderBase Interface

The `IBuilderBase` interface hierarchy provides a fluent API for configuring services and options:

```csharp
public interface IBuilderBase
{
    IServiceCollection Services { get; }
}

public interface IBuilderBase<TOptions> : IBuilderBase
    where TOptions : class, IOptionsBase
{
    OptionsBuilder<TOptions> OptionsBuilder { get; }
    
    OptionsBuilder<TSubOptions> AddSubOptions<TSubOptions>()
        where TSubOptions : class, IOptionsBase;
}
```

**Key Features:**
- **Service Collection Access**: Direct access to the underlying `IServiceCollection`
- **Options Configuration**: Type-safe configuration of options with support for validation
- **Hierarchical Options**: Built-in support for nested configuration sections
- **Fluent API**: Method chaining for clean and readable configuration code

## Design Principles

1. **Separation of Concerns**: Clear distinction between abstraction and implementation
2. **Type Safety**: Compile-time checking of configuration options
3. **Extensibility**: Designed to be extended for various configuration scenarios
4. **Consistency**: Uniform patterns across all extension libraries
5. **Dependency Injection First**: Built around .NET's built-in dependency injection

## Usage Example

```csharp
// Define your options class
public class MyOptions : IOptionsBase
{
    public static string Key => "MySection";
    public string Setting1 { get; set; }
    public int Setting2 { get; set; }
}

// Define your builder interface
public interface IMyBuilder : IBuilderBase<MyOptions>
{
    // Additional builder methods can be added here
}
```

## Architecture

The project follows these architectural principles:
- **Minimal Dependencies**: Only essential dependencies are included
- **Interface Segregation**: Small, focused interfaces for each responsibility
- **Extension Friendly**: Designed to be extended by other projects in the ecosystem
- **Testable**: Interfaces make it easy to create test doubles for unit testing