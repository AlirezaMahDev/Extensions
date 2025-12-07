# AlirezaMahDev.Extensions.Brain.Abstractions

## Project Description

This project defines the core abstractions and interfaces for the Brain extension, providing a contract-based approach to neural network operations. It serves as the foundation for the Brain implementation, enabling loose coupling and testability.

## Dependencies

### Project Dependencies
- **AlirezaMahDev.Extensions.DataManager.Abstractions**: Provides data management abstractions for neural network persistence

## Core Interfaces

### INerve<TData>

The central interface representing a neural network, providing methods for training and inference:
- `Learn`: Trains the network with provided data
- `Think`: Performs inference on input data
- `Sleep`: Optimizes the network state
- `Name`: Identifies the nerve instance
- `Root`: Provides access to the root connection

### INeuron<TData>

Represents a single neuron in the network:
- `Offset`: Memory offset of the neuron
- `RefValue`: Direct reference to the neuron's value
- `RefData`: Direct reference to the neuron's data
- `Connection`: Gets or creates connections to other neurons

### IConnection<TData>

Manages connections between neurons:
- `Location`: Data location of the connection
- `Offset`: Memory offset of the connection
- `Neuron`: Connected neuron
- `Previous`/`Next`: Navigation through connection chains

### IBrainService

Service interface for managing neural network instances:
- `GetOrAdd<TData>`: Retrieves or creates a named neural network

## Data Structures

### NeuronValue<TData>

Structure representing a neuron's state:
- `Data`: The actual data stored in the neuron
- `Score`: Activation score
- `Weight`: Connection weight
- `Connection`: Reference to connected neurons

### ConnectionValue

Structure representing a connection between neurons:
- `Next`: Reference to the next neuron in the connection chain
- `Weight`: Strength of the connection

## Design Principles

1. **Type Safety**: Strongly-typed generic interfaces
2. **Separation of Concerns**: Clear distinction between different neural network components
3. **Memory Efficiency**: Uses value types and references where appropriate
4. **Extensibility**: Designed to support various neural network architectures

## Usage Example

```csharp
// Get or create a neural network
var nerve = brainService.GetOrAdd<float[]>(networkName);

// Train the network
var trainingData = GetTrainingData();
nerve.Learn(trainingData);

// Perform inference
var input = new float[] { /* input data */ };
var result = nerve.Think(input);
```

## Architecture

The project follows a clean architecture with:
- **Abstractions Layer**: Defines the contracts for neural network operations
- **Data Structures**: Provides the core data types used by the implementation
- **Service Contracts**: Defines the service interfaces for neural network management

This design allows for different implementations while maintaining a consistent API for consumers.