# AlirezaMahDev.Extensions.File.Data.Abstractions

## Project Description

Contract layer for file-backed binary data access. It defines the primitives (services, value types, and contracts)
needed to build high-performance, persistent data structures over files while keeping implementations decoupled.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.File.Abstractions**: Provides core file access contracts used for low-level I/O.

### Why these dependencies?

- **File.Abstractions** is the underlying file contract that data abstractions build upon to map binary data to
  persistent storage in a consistent way.

## Key Components

- **Data access contracts**: Interfaces to read, write, allocate, and navigate data in files.
- **Locations and paths**: Abstractions to represent offsets/lengths and hierarchical paths inside data files.
- **Typed value helpers**: Facilities to read/write unmanaged value types efficiently.
- **Extensibility points**: Interfaces intended to be implemented by concrete storage providers (e.g., collections,
  stacks, tables).

These abstractions allow other projects (e.g., File.Data, Collection, Stack, Table) to implement concrete, testable, and
interchangeable storage layers.
