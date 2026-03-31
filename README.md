# AlirezaMahDev.Extensions

A collection of extension libraries for .NET projects designed to provide reusable components and utilities to accelerate development.

## Project Overview

This monorepo contains multiple extension libraries organized by functionality:

### 1. Core

| Project | Description |
|---------|-------------|
| [`AlirezaMahDev.Extensions`](src/AlirezaMahDev.Extensions/README.md) | Builder pattern implementation for .NET configuration |
| [`AlirezaMahDev.Extensions.Abstractions`](src/AlirezaMahDev.Extensions.Abstractions/README.md) | Core abstractions and interfaces |

### 2. Brain (Neural Network)

| Project | Description |
|---------|-------------|
| [`AlirezaMahDev.Extensions.Brain`](src/AlirezaMahDev.Extensions.Brain/README.md) | Neural network implementation with GPU acceleration |
| [`AlirezaMahDev.Extensions.Brain.Abstractions`](src/AlirezaMahDev.Extensions.Brain.Abstractions/README.md) | Neural network abstractions |

### 3. DataManager

| Project | Description |
|---------|-------------|
| [`AlirezaMahDev.Extensions.DataManager`](src/AlirezaMahDev.Extensions.DataManager/README.md) | File-based data management |
| [`AlirezaMahDev.Extensions.DataManager.Abstractions`](src/AlirezaMahDev.Extensions.DataManager.Abstractions/README.md) | Data management abstractions |

### 4. File Operations

| Project | Description |
|---------|-------------|
| `AlirezaMahDev.Extensions.File` | File operation utilities |
| `AlirezaMahDev.Extensions.File.Abstractions` | File operation abstractions |
| `AlirezaMahDev.Extensions.File.Data` | Data file components |

### 5. Other Projects

| Project | Description |
|---------|-------------|
| `AlirezaMahDev.Extensions.Node` | Node-based structures |
| `AlirezaMahDev.Extensions.ParameterInstance` | Parameter instance management |
| `AlirezaMahDev.Extensions.PersianTimestamp` | Persian date/time utilities |
| `AlirezaMahDev.Extensions.Progress` | Progress reporting |

---

## Data Structures (Struct) and Extension Methods

This project uses **C# 12 Extension Methods** which provides cleaner syntax compared to traditional approaches.

### C# 12 Extension Methods Syntax

```csharp
// New C# 12 syntax
public static class MyExtensions
{
    extension<T>(T value) where T : MyConstraint
    {
        public void MyMethod() { /* ... */ }
    }
}

// Usage
var result = value.MyMethod();
```

### Practical Examples

#### Example 1: Using NeuronExtensions

```csharp
// Create a neuron
var neuron = new Neuron(offset);

// Create a wrap for the neuron
var wrap = neuron.NewWrap<MyData>(nerve);
```

#### Example 2: Using ConnectionExtensions

```csharp
// Create a connection
var connection = new Connection(offset);

// Create a wrap for the connection
var connectionWrap = connection.NewWrap<MyData>(nerve);
```

#### Example 3: Using NerveExtensions

```csharp
// Find or add a neuron
var neuron = nerve.FindOrAddNeuron(ref data);

// Or using cache
var cacheKey = nerve.CreateNeuronCacheKey(in data);
var result = nerve.FindNeuronCore(ref cacheKey, in data);
```

#### Example 4: Using NeuronWrapExtensions

```csharp
// Get connection wrap
var connectionWrap = wrap.ConnectionWrap;

// Get all connections
var connections = wrap.GetConnectionsWrap();
```

---

## Installation

### Via NuGet

```bash
# Core
dotnet add package AlirezaMahDev.Extensions
dotnet add package AlirezaMahDev.Extensions.Abstractions

# Brain
dotnet add package AlirezaMahDev.Extensions.Brain
dotnet add package AlirezaMahDev.Extensions.Brain.Abstractions

# DataManager
dotnet add package AlirezaMahDev.Extensions.DataManager
dotnet add package AlirezaMahDev.Extensions.DataManager.Abstractions
```

### Configuration in Program.cs

```csharp
// Add services
services.AddBrain(builder =>
{
    builder.AddNerve<MyDataType>("nerve1");
});

services.AddDataManager(options =>
{
    options.DirectoryPath = "Data";
});
```

---

## Key Dependencies

- **.NET 10.0** (or higher)
- **Microsoft.Extensions.Hosting**: Hosting infrastructure
- **ILGPU**: GPU acceleration for computations
- **Polly**: Resilience and fault handling
- **CommunityToolkit.HighPerformance**: Performance optimization

---

## Architecture

Projects follow a layered architecture:

```
Abstractions (Interfaces/Contracts)
    ↓
Implementation (Concrete Classes)
    ↓
Extensions (Utility Methods)
```

### Design Principles

1. **Separation of Concerns**: Clear distinction between abstraction and implementation
2. **Type Safety**: Compile-time checking
3. **Extensibility**: Designed for various scenarios
4. **Performance**: Optimized for high performance
5. **Thread Safety**: Support for concurrent operations
