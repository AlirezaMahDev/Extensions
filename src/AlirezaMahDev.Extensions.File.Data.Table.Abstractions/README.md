# AlirezaMahDev.Extensions.File.Data.Table.Abstractions

## Project Description

Contracts for tabular, file-backed storage. Defines table, row, column, and indexing abstractions that map structured data onto the File.Data primitives while keeping implementations swappable.

## Dependencies

### Project Dependencies
- **AlirezaMahDev.Extensions.File.Data.Abstractions**: Base binary storage contracts.
- **AlirezaMahDev.Extensions.File.Data.Collection.Abstractions**: Collection metadata and object-grouping contracts.
- **AlirezaMahDev.Extensions.File.Data.Stack.Abstractions**: Stack contracts for append/log-style scenarios.

### Why these dependencies?
- Tables build on data blocks/locations (File.Data), use collection metadata (Collection) and may employ stack-like structures for journaling or append operations (Stack).

## Key Components

- **ITable/ITableFactory contracts**: Open/create tables and manage schema.
- **IRow/IColumn abstractions**: Represent structured records and fields.
- **Index contracts**: Optional index interfaces for fast lookups.

These abstractions standardize table semantics so multiple concrete storage engines can be used interchangeably.
