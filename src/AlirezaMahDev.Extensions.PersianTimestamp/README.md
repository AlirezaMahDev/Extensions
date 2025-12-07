# AlirezaMahDev.Extensions.PersianTimestamp

## Project Description

This project provides a comprehensive implementation of Persian (Jalali) date and time functionality for .NET applications. It offers a strongly-typed, high-performance implementation of Persian calendar operations, including date parsing, formatting, and arithmetic operations.

## Key Features

- **PersianDateTime Struct**: A value type representing dates and times in the Persian calendar
- **Type Safety**: Strongly-typed API for working with Persian dates and times
- **High Performance**: Optimized for performance with minimal allocations
- **Modern .NET Integration**: Implements modern .NET interfaces like `ISpanFormattable`, `IUtf8SpanFormattable`
- **Calendar Support**: Built on top of .NET's `GregorianCalendar` with Persian calendar rules

## Core Components

### PersianDateTime

A value type that represents a point in time in the Persian calendar system:
- Implements standard .NET interfaces for consistent behavior
- Supports parsing and formatting in various formats
- Provides methods for date and time arithmetic
- Thread-safe and immutable

### PersianDefault

Provides default calendar instances and settings:
- `Calendar`: Default `GregorianCalendar` instance configured for Persian calendar operations

## Usage Example

```csharp
// Create a new PersianDateTime
var persianDate = new PersianDateTime(
    year: 1402,
    month: 10,
    day: 17,
    hour: 14,
    minute: 30,
    second: 0,
    millisecond: 0,
    microsecond: 0,
    DateTimeKind.Local);

// Format the date
string formatted = persianDate.ToString("yyyy/MM/dd HH:mm:ss");
// Output: "1402/10/17 14:30:00"

// Parse a Persian date string
if (PersianDateTime.TryParse("1402/10/17 14:30:00", out var parsedDate))
{
    // Use the parsed date
    Console.WriteLine(parsedDate);
}

// Date arithmetic
var tomorrow = persianDate.AddDays(1);
var lastMonth = persianDate.AddMonths(-1);
```

## Dependencies

- **.NET 10.0**: Base framework requirements
- **System.Globalization**: For calendar and culture-specific operations

## Design Principles

1. **Immutability**: All types are immutable for thread safety and predictable behavior
2. **Performance**: Optimized for high-performance scenarios with minimal allocations
3. **Consistency**: Follows .NET's design patterns for date and time handling
4. **Extensibility**: Designed to be extended with additional functionality as needed

## Implementation Notes

- The implementation uses .NET's built-in calendar system for accurate date calculations
- All operations are culture-aware and respect Persian calendar rules
- The API is designed to be familiar to .NET developers while providing Persian-specific functionality
- The code is annotated with nullability attributes for better static analysis

## Best Practices

1. **Use Value Types**: `PersianDateTime` is a value type, so it's best used for local variables and method parameters
2. **Format Strings**: Use standard .NET format strings for consistent formatting
3. **Error Handling**: Always use the `TryParse` pattern when parsing user input
4. **Time Zones**: Be mindful of time zone considerations when working with Persian dates and times
5. **Performance**: For high-performance scenarios, use the `ISpanFormattable` interface to avoid string allocations

## Extension Methods

This project also provides extension methods for working with Persian dates and times:

```csharp
// Convert DateTime to PersianDateTime
var now = DateTime.Now.ToPersianDateTime();

// Convert PersianDateTime to DateTime
DateTime gregorian = now.ToDateTime();

// Get Persian month name
string monthName = now.GetPersianMonthName();
```

## Contributing

Contributions are welcome! Please ensure that any changes maintain backward compatibility and include appropriate unit tests.