# AlirezaMahDev.Extensions.File.Json

## Project Description

JSON-centric utilities over the File and Extensions infrastructure. Provides services to persist and retrieve JSON
documents with thread-safe file access and a fluent configuration model.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions.File.Json.Abstractions**: Contracts implemented by this project.
- **AlirezaMahDev.Extensions.File**: Thread-safe file access used for JSON persistence.
- **AlirezaMahDev.Extensions**: Builder and options infrastructure.

### Why these dependencies?

- Uses File to ensure safe concurrent I/O, and the shared builder pipeline for consistent configuration across the
  ecosystem. Abstractions keep the consumer API stable.

## Key Components

- **JsonService**: High-level API for loading/saving JSON documents.
- **JsonAccess**: Encapsulates read/replace/change operations for JSON files via the underlying file access.
- **JsonBuilder/Options**: Fluent configuration for storage paths and conventions.

This module simplifies durable JSON storage scenarios while aligning with the ecosystem’s DI and configuration patterns.
