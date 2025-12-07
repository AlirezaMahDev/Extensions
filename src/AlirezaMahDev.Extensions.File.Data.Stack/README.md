# AlirezaMahDev.Extensions.File.Data.Stack

## Project Description

Concrete, file-backed stack data structures built on top of the File.Data layer. Provides persistent LIFO containers with thread-safe operations and efficient binary layout suitable for large datasets.

## Dependencies

### Project Dependencies
- **AlirezaMahDev.Extensions.File.Data**: Low-level binary data storage and allocation APIs.
- **AlirezaMahDev.Extensions.File.Data.Stack.Abstractions**: Contracts implemented by this project.

### Why these dependencies?
- **File.Data** supplies the block, location, and allocation primitives used to persist stack nodes.
- **Stack.Abstractions** defines the stack contracts so implementations remain interchangeable and testable.

## Key Components

- **Stack Access/Factory**: Creates and opens named stacks backed by a shared data file.
- **Stack Node Layouts**: Compact binary representation of nodes with next pointers and payload.
- **Thread-Safe Operations**: Push/Pop/Peek coordinated via File.Data synchronization primitives.

This library enables durable stack semantics with predictable performance characteristics over file storage.
