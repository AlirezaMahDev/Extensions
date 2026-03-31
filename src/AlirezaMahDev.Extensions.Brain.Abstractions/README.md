# AlirezaMahDev.Extensions.Brain.Abstractions

## Project Description

This project defines the core abstractions and interfaces for the Brain extension, providing a contract-based approach to neural network operations. It serves as the foundation for the Brain implementation, enabling loose coupling and testability.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.DataManager.Abstractions**: Provides data management abstractions for neural network persistence

---

## Core Interfaces

### INerve<TData, TLink>

The central interface representing a neural network, providing methods for training and inference:

- `Learn`: Trains the network with provided data
- `Think`: Performs inference on input data
- `Sleep`: Optimizes the network state
- `Name`: Identifies the nerve instance
- `Root`: Provides access to the root connection
- `Access`: Provides access to data storage

### INeuron

Represents a single neuron in the network:

- `Offset`: Memory offset of the neuron
- `RefValue`: Direct reference to the neuron's value
- `RefData`: Direct reference to the neuron's data
- `Connection`: Gets or creates connections to other neurons

### IConnection<TLink>

Manages connections between neurons:

- `Location`: Data location of the connection
- `Offset`: Memory offset of the connection
- `Neuron`: Connected neuron
- `Previous`/`Next`: Navigation through connection chains
- `Link`: Link data

### IBrainService

Service interface for managing neural network instances:

- `GetOrAdd<TData>`: Retrieves or creates a named neural network

---

## Data Structures (Structs)

### Neuron

```csharp
public readonly struct Neuron(DataOffset offset)
    : IScopedRefReadOnlyEquatable<Neuron>, IScopedInEqualityOperators<Neuron, Neuron, bool>
```

Represents a neuron in the neural network using a memory offset.

**Properties:**

- `Offset`: Memory offset in the data store

**Usage:**

```csharp
var neuron = new Neuron(offset);
var offsetValue = neuron.Offset;
```

### Connection

```csharp
public readonly struct Connection(DataOffset offset)
    : IScopedRefReadOnlyEquatable<Connection>, IScopedInEqualityOperators<Connection, Connection, bool>
```

Represents a connection between neurons.

**Properties:**

- `Offset`: Memory offset in the data store

**Usage:**

```csharp
var connection = new Connection(offset);
var connectionOffset = connection.Offset;
```

### Cache (or CacheValue)

```csharp
public readonly struct Cache(DataOffset offset)
    : IScopedRefReadOnlyEquatable<Cache>, IScopedInEqualityOperators<Cache, Cache, bool>
```

Represents a cached item in the neural network.

**Properties:**

- `Offset`: Memory offset in the cache store

### NerveCacheKey

```csharp
public readonly struct NerveCacheKey(ReadOnlySpan<byte> bytes)
```

A key structure used for caching neurons in memory.

**Usage:**

```csharp
var cacheKey = nerve.CreateNeuronCacheKey(in data);
```

### CellWrap<TValue, TData, TLink>

```csharp
public readonly struct CellWrap<TValue, TData, TLink>(INerve<TData, TLink> nerve, DataLocation<TValue> location)
    : IScopedRefReadOnlyEquatable<CellWrap<TValue, TData, TLink>>
```

A wrapper providing access to cell data with nerve context.

**Properties:**

- `Nerve`: The associated nerve
- `Location`: The data location

**Usage:**

```csharp
var wrap = new CellWrap<NeuronValue<TData>, TData, TLink>(nerve, location);
var nerveRef = wrap.Nerve;
var neuronValue = wrap.Location.RefValue;
```

### CellEnumerable<T>

```csharp
public readonly struct CellEnumerable<T>(int count, IEnumerable<T> enumerable) : IEnumerable<T>, ICollection<T>
```

An enumerable collection of cells.

---

## Extension Methods

The project uses **C# 12 Extension Methods** with the `extension` keyword. Here are the available extensions:

### NeuronExtensions

```csharp
extension<TData, TLink>(in Neuron neuron)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `NewWrap(INerve<TData, TLink> nerve)`: Creates a new CellWrap for the neuron
- `NewWrap<TValue>(ref CellWrap<TValue, TData, TLink> wrap)`: Creates a wrap with custom value type

**Usage:**

```csharp
var neuron = new Neuron(offset);
var wrap = neuron.NewWrap<MyData>(nerve);
```

### ConnectionExtensions

```csharp
extension<TData, TLink>(in Connection connection)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `NewWrap(INerve<TData, TLink> nerve)`: Creates a new CellWrap for the connection
- `NewWrap<TValue>(ref CellWrap<TValue, TData, TLink> wrap)`: Creates a wrap with custom value type

**Usage:**

```csharp
var connection = new Connection(offset);
var connectionWrap = connection.NewWrap<MyLinkData>(nerve);
```

### NerveExtensions

```csharp
extension<TData, TLink>(INerve<TData, TLink> nerve)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `FindNeuronCore(ref readonly NerveCacheKey cacheKey, ref readonly TData data)`: Find neuron using cache key
- `FindOrAddNeuron(ref readonly TData data)`: Find or create a neuron
- `AddNeuronCore(...)`: Internal method for adding neurons

**Usage:**

```csharp
var neuron = nerve.FindOrAddNeuron(ref data);
var cacheKey = nerve.CreateNeuronCacheKey(in data);
var result = nerve.FindNeuronCore(ref cacheKey, in data);
```

### NeuronWrapExtensions

```csharp
extension<TData, TLink>(in CellWrap<NeuronValue<TData>, TData, TLink> wrap)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Properties:**

- `ConnectionWrap`: Get the connection wrap for this neuron

**Methods:**

- `GetConnectionsWrap()`: Get all connections
- `FindCore(...)`: Find connections by cache key
- `FindOrAddConnectionCore(...)`: Find or create connections

**Usage:**

```csharp
var connectionWrap = wrap.ConnectionWrap;
var connections = wrap.GetConnectionsWrap();
```

### ConnectionWrapExtensions

```csharp
extension<TData, TLink>(in CellWrap<ConnectionValue<TLink>, TData, TLink> wrap)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Properties:**

- `NeuronWrap`: Get the associated neuron wrap

**Methods:**

- `GetNextWrap()`: Get next connection in chain
- `GetPreviousWrap()`: Get previous connection in chain

### ConnectionWrapMemoryExtensions

```csharp
extension<TData, TLink>(in CellWrap<ConnectionValue<TLink>, TData, TLink> wrap)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `FirstOrDefault(...)`: Find first matching connection
- `Any(...)`: Check if any connections match

### NerveCacheExtensions

```csharp
extension<TData, TLink>(INerve<TData, TLink> nerve)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `TryGetNeuronCacheCore(...)`: Try to get cached neuron
- `TrySetNeuronCacheCore(...)`: Set neuron cache
- `TryGetNeuronConnectionCacheCore(...)`: Get cached connection
- `CreateNeuronCacheKey(...)`: Create cache key

### NerveSleepExtensions

```csharp
extension<TData, TLink>(INerve<TData, TLink> nerve)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `Sleep()`: Optimize network state (clear caches, consolidate)
- `SleepCore(...)`: Core sleep operation

### NerveThinkExtensions

```csharp
extension<TData, TLink>(INerve<TData, TLink> nerve)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `Think(in TData data)`: Perform inference
- `ThinkCore(...)`: Core think operation

### NerveLearnExtensions

```csharp
extension<TData, TLink>(INerve<TData, TLink> nerve)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
```

**Methods:**

- `Learn(in TData data)`: Learn from data
- `LearnCore(...)`: Core learn operation

---

## Design Principles

1. **Type Safety**: Strongly-typed generic interfaces
2. **Separation of Concerns**: Clear distinction between different neural network components
3. **Memory Efficiency**: Uses value types and references where appropriate
4. **Extensibility**: Designed to support various neural network architectures
5. **Performance**: Optimized for high-throughput operations

---

## Usage Examples

### Basic Usage

```csharp
// Get or create a neural network
var nerve = brainService.GetOrAdd<float[]>(networkName);

// Train the network
var trainingData = GetTrainingData();
nerve.Learn(trainingData);

// Perform inference
var input = new float[] { /* input data */ };
var result = nerve.Think(input);

// Optimize network
nerve.Sleep();
```

### Using Neuron Extensions

```csharp
// Find or create a neuron with specific data
var neuron = nerve.FindOrAddNeuron(ref myData);

// Create a wrapper for the neuron
var neuronWrap = neuron.NewWrap<MyDataType>(nerve);

// Access the neuron's value
var neuronValue = neuronWrap.Location.RefValue;

// Get connections
var connections = neuronWrap.GetConnectionsWrap();
```

### Using Cache

```csharp
// Create cache key from data
var cacheKey = nerve.CreateNeuronCacheKey(in data);

// Try to find in cache
if (nerve.TryGetNeuronCacheCore(in cacheKey, out var offset))
{
    var cachedNeuron = new Neuron(offset);
    // Use cached neuron
}
```

---

## Architecture

The project follows a clean architecture with:

- **Abstractions Layer**: Defines the contracts for neural network operations
- **Data Structures**: Provides the core value types used by the implementation
- **Service Contracts**: Defines the service interfaces for neural network management
- **Extensions**: Provides utility extension methods for easy usage

This design allows for different implementations while maintaining a consistent API for consumers.
