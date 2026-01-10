# AlirezaMahDev.Extensions.File.Data.Table

## Project Description

Persistent, file-backed tabular storage built over the File.Data, Collection, and Stack layers. Provides row/column
style data modeling with indexes and efficient binary layout for scalable reads and writes.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.File.Data**: Core binary storage and allocation.
- **AlirezaMahDev.Extensions.File.Data.Collection**: Collection primitives for object grouping and metadata.
- **AlirezaMahDev.Extensions.File.Data.Stack**: Stack primitives for append-like and log-style operations.
- **AlirezaMahDev.Extensions.File.Data.Table.Abstractions**: Contracts implemented by this project.

### Why these dependencies?

- Tables are composed from lower-level blocks (File.Data), object collections (Collection), and stack-like structures
  for append/undo scenarios (Stack). Abstractions ensure a stable API across implementations.

## Key Components

- **Table Access/Factory**: Creates/opens tables and manages schema metadata.
- **Row/Column Layouts**: Compact binary representations optimized for random access and scanning.
- **Indexing**: Optional index structures for key lookups.
- **Concurrency**: Thread-safe operations built on File.Data synchronization primitives.

This module offers a foundation for structured datasets where durability and predictable performance are required.
