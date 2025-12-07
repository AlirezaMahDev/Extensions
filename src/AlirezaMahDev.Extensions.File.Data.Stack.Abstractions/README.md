# AlirezaMahDev.Extensions.File.Data.Stack.Abstractions

## Project Description

Abstraction layer for persistent, file-backed stack data structures. Defines contracts for stack operations and storage semantics over the File.Data primitives so implementations can be substituted without changing consumers.

## Dependencies

### Project Dependencies
- **AlirezaMahDev.Extensions.File.Data.Abstractions**: Base contracts for binary file data access used by stack abstractions.

### Why this dependency?
- Stack abstractions are expressed in terms of data locations/blocks and synchronization primitives provided by File.Data.Abstractions.

## Key Components

- **Stack contracts**: Interfaces for Push, Pop, Peek, Count, and enumeration.
- **Node representation**: Contract for linking nodes via offsets to enable O(1) push/pop.
- **Factory/Access contracts**: Interfaces to open/create named stacks in a given data context.

These abstractions allow multiple concrete implementations to exist (different layouts or policies) while keeping the consumer API stable.
