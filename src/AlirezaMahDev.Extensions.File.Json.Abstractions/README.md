# AlirezaMahDev.Extensions.File.Json.Abstractions

## Project Description

Contracts for JSON file persistence over the ecosystem’s file infrastructure. Defines interfaces for reading, writing, and transforming JSON documents while keeping concrete storage and serialization details pluggable.

## Dependencies

### Project Dependencies
- **AlirezaMahDev.Extensions.File.Abstractions**: Underlying file access contracts used to implement JSON persistence safely.

### Why this dependency?
- JSON abstractions rely on standardized file access semantics (locking, async I/O, streams) provided by File.Abstractions.

## Key Components

- **JSON access contracts**: Interfaces to load, save, and modify JSON documents atomically.
- **Service/Factory contracts**: Open or create JSON-backed resources by logical name.
- **Options contracts**: Configure storage paths and naming conventions via the shared options model.

These contracts allow multiple JSON implementations to coexist while presenting a stable API to consumers.
