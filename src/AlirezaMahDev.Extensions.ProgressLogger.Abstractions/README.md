# AlirezaMahDev.Extensions.ProgressLogger.Abstractions

## Project Description

Contracts for structured progress reporting in .NET applications. Defines interfaces and options that standardize how long-running operations report progress, enabling multiple implementations and sinks.

## Dependencies

### Project Dependencies
- None within the repository beyond .NET base libraries for contracts.

## Key Components

- **Progress Logger Contracts**: Interfaces for starting, advancing, and completing progress scopes.
- **Scopes & Context**: Abstractions to group related progress messages and carry metadata.
- **Options Contracts**: Configure categories, throttling, and sink selection via the shared options model in implementations.

These abstractions allow applications and libraries to depend on a stable API while choosing a concrete logging strategy at composition time.
