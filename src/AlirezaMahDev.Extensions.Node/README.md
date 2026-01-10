# AlirezaMahDev.Extensions.Node

## Project Description

Integration layer for running Node.js-backed tasks from .NET. Provides services and builders to configure a lightweight
node worker that can execute TypeScript/JavaScript tasks and exchange data with .NET services.

## Dependencies

### Project Dependencies

- **AlirezaMahDev.Extensions**: Uses the core builder infrastructure and DI patterns.

### Why this dependency?

- Relies on the shared builder pattern and options pipeline to register node-related services and configuration
  consistently with the rest of the ecosystem.

## Key Components

- **INodeService / NodeService**: High-level API to queue and execute node tasks and retrieve results.
- **NodeWorker**: Background worker that manages node process lifecycle and task execution.
- **NodeBuilder / NodeOptions**: Fluent configuration surface for service registration and runtime options (paths,
  scripts, etc.).
- **TypeScript assets**: `index.ts` and accompanying configuration provide the default runtime bridge for task
  execution.

This project enables hybrid solutions where performance-critical or ecosystem-specific logic can be delegated to Node.js
while remaining fully integrated into a .NET application.
