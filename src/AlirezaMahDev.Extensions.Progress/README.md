# AlirezaMahDev.Extensions.ProgressLogger

## Project Description

Lightweight progress logging utilities for .NET applications, built on the shared builder infrastructure. Provides
simple, structured progress reporting APIs suitable for long-running operations and background tasks.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions**: Uses the common builder and options patterns for DI registration and configuration.

### Why this dependency?

- Ensures consistent configuration and service registration across the ecosystem, enabling easy composition with other
  libraries.

## Key Components

- **Progress Logger Services**: Report start/advance/complete events with contextual data.
- **Scopes and Categories**: Group related progress messages and attach metadata.
- **Builder/Options**: Configure defaults (e.g., categories, throttling, sinks).

This project aims to provide a consistent pattern for reporting progress that integrates naturally with the rest of the
ecosystem.
