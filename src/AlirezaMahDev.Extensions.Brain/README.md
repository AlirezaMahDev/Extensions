d# AlirezaMahDev.Extensions.Brain

## Project Description

This project implements a neural network infrastructure with a focus on high-performance computation, leveraging ILGPU
for GPU acceleration. It provides a flexible and extensible framework for building and training neural networks with
support for custom data types and network architectures.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.Brain.Abstractions**: Core interfaces and contracts
- **AlirezaMahDev.Extensions.DataManager**: Data management and persistence
- **AlirezaMahDev.Extensions.ParameterInstance**: Parameter handling and configuration
- **AlirezaMahDev.Extensions**: Core extension infrastructure

### NuGet Dependencies

- **ILGPU (v1.5.3)**: High-performance GPU computing
- **ILGPU.Algorithms (v1.5.3)**: GPU-accelerated algorithms

## Key Components

### Nerve

The `Nerve<TData>` class is the central component that manages the neural network. It provides:

- Neural network training and inference capabilities
- Thread-safe operations for concurrent access
- Integration with data management for persistence
- GPU acceleration through ILGPU

### Neuron

The `Neuron<TData>` class represents individual neurons in the network, featuring:

- Support for generic data types (unmanaged)
- Thread-safe caching mechanism
- Connection management to other neurons
- Efficient memory management

### Connection

Connection handling between neurons, including:

- Weighted connections
- Bi-directional relationships
- Connection persistence

### Factory Classes

- **NeuronFactory**: Creates and manages neuron instances
- **ConnectionFactory**: Handles creation and management of connections
- **BrainBuilder**: Fluent API for configuring the brain extension

## Usage Example

```csharp
// Configure services
services.AddBrain(builder =>
{
    // Configure brain components
    builder.AddNerve<MyDataType>("nerve1");
    // Additional configuration...
});

// Resolve and use
var nerve = serviceProvider.GetRequiredService<INerve<MyDataType>>();
nerve.Learn(trainingData);
var result = nerve.Predict(inputData);
```

## Performance Considerations

- Utilizes GPU acceleration through ILGPU for computationally intensive operations
- Implements efficient memory management for large neural networks
- Thread-safe design for concurrent access
- Optimized data structures for fast neural network operations

## Architecture

The project follows a modular architecture with clear separation of concerns:

- **Data Layer**: Handles persistence and retrieval of neural network data
- **Core Logic**: Implements neural network operations and algorithms
- **Infrastructure**: Provides GPU acceleration and performance optimizations
- **API Layer**: Exposes a clean, type-safe interface for consumers

## Configuration

Configuration can be done through the `BrainBuilder` class, which provides a fluent API for setting up the neural
network with various options and parameters.