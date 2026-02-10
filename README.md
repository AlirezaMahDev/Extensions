# AlirezaMahDev.Extensions

This repository contains a collection of extension libraries for .NET projects. The goal is to provide reusable components and utilities to accelerate development.

## Overview

The extensions are organized into different projects, each targeting a specific functionality. This repository serves as a monorepo to manage and develop these extensions in a coordinated way.

## Available Extensions

Here is a list of the available extension libraries in this repository:

*   **AlirezaMahDev.Extensions.Abstractions**: Provides common abstractions and interfaces used by other extensions.
*   **AlirezaMahDev.Extensions.Brain**: Contains components related to neural networks or other AI-related logic.
    *   **AlirezaMahDev.Extensions.Brain.Abstractions**: Abstractions for the Brain extension.
*   **AlirezaMahDev.Extensions.DataManager**: A tool for managing data.
    *   **AlirezaMahDev.Extensions.DataManager.Abstractions**: Abstractions for the DataManager.
    *   **AlirezaMahDev.Extensions.DataManager.Stack**: Stack-based data management.
        *   **AlirezaMahDev.Extensions.DataManager.Stack.Abstractions**: Abstractions for stack-based data management.
*   **AlirezaMahDev.Extensions.File**: Utilities for file operations.
    *   **AlirezaMahDev.Extensions.File.Abstractions**: Abstractions for file operations.
    *   **AlirezaMahDev.Extensions.File.Data**: Components for handling data files.
        *   **AlirezaMahDev.Extensions.File.Data.Abstractions**: Abstractions for data files.
        *   **AlirezaMahDev.Extensions.File.Data.Collection**: For collections of data files.
            *   **AlirezaMahDev.Extensions.File.Data.Collection.Abstractions**: Abstractions for collections of data files.
        *   **AlirezaMahDev.Extensions.File.Data.Stack**: For stack-based data files.
            *   **AlirezaMahDev.Extensions.File.Data.Stack.Abstractions**: Abstractions for stack-based data files.
        *   **AlirezaMahDev.Extensions.File.Data.Table**: For table-structured data files.
            *   **AlirezaMahDev.Extensions.File.Data.Table.Abstractions**: Abstractions for table-structured data files.
    *   **AlirezaMahDev.Extensions.File.Json**: Utilities for JSON file operations.
        *   **AlirezaMahDev.Extensions.File.Json.Abstractions**: Abstractions for JSON file operations.
*   **AlirezaMahDev.Extensions.Node**: Provides node-based logic or structures.
*   **AlirezaMahDev.Extensions.ParameterInstance**: Helpers for creating and managing instances of parameters.
    *   **AlirezaMahDev.Extensions.ParameterInstance.Abstractions**: Abstractions for parameter instances.
*   **AlirezaMahDev.Extensions.PersianTimestamp**: Provides utilities for handling Persian dates and timestamps.
*   **AlirezaMahDev.Extensions.Progress**: Components for reporting progress of long-running operations.
    *   **AlirezaMahDev.Extensions.Progress.Abstractions**: Abstractions for progress reporting.

## How to Use

To use these extensions, you can reference the desired projects in your solution.
