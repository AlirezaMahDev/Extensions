// using System.Diagnostics.CodeAnalysis;
// using System.Runtime.InteropServices;
// using System.Runtime.Serialization;
// using System.Text.Json;
//
// namespace AlirezaMahDev.Extensions.PersianTimestamp;
//
// public static class PersianDefault
// {
//     public static System.Globalization.GregorianCalendar Calendar { get; } = new();
// }
//
// [StructLayout(LayoutKind.Sequential)]
// public readonly struct PersianDateTime :
//     IComparable,
//     IComparable<PersianDateTime>,
//     IFormattable,
//     ISpanFormattable,
//     IEquatable<PersianDateTime>,
//     IParsable<PersianDateTime>,
//     ISpanParsable<PersianDateTime>,
//     IUtf8SpanFormattable
// {
//     private readonly DateTime _dateTime;
//
//     public PersianDateTime(PersianDateOnly date, TimeOnly time)
//     {
//         _dateTime = new(date, time);
//     }
//
//     public PersianDateTime(int year,
//         int mouth,
//         int day,
//         int hour,
//         int minute,
//         int second,
//         int millisecond,
//         DateTimeKind dateTimeKind)
//     {
//         _dateTime = new(year,
//             mouth,
//             day,
//             hour,
//             minute,
//             second,
//             millisecond,
//             PersianDefault.Calendar,
//             dateTimeKind);
//     }
//     
//     public PersianDateTime(int year,
//         int mouth,
//         int day,
//         int hour,
//         int minute,
//         int second,
//         int millisecond,
//         DateTimeKind dateTimeKind)
//     {
//         _dateTime = new(year,
//             mouth,
//             day,
//             hour,
//             minute,
//             second,
//             millisecond,
//             PersianDefault.Calendar,
//             dateTimeKind);
//     }
//     
//     public PersianDateTime(int year,
//         int mouth,
//         int day,
//         int hour,
//         int minute,
//         int second,
//         int millisecond,
//         int microsecond)
//     {
//         _dateTime = new(year,
//             mouth,
//             day,
//             hour,
//             minute,
//             second,
//             millisecond,
//             microsecond,
//             PersianDefault.Calendar);
//     }
//
//     public PersianDateTime(int year,
//         int mouth,
//         int day,
//         int hour,
//         int minute,
//         int second,
//         int millisecond,
//         int microsecond,
//         DateTimeKind dateTimeKind)
//     {
//         _dateTime = new(year,
//             mouth,
//             day,
//             hour,
//             minute,
//             second,
//             millisecond,
//             microsecond,
//             PersianDefault.Calendar,
//             dateTimeKind);
//     }
//
//     public DateTime ToDateTime() => _dateTime;
// }
//
// [StructLayout(LayoutKind.Sequential)]
// public struct PersianDateTimeOffset
// {
//     private readonly DateTimeOffset _dateTimeOffset;
//
//     public PersianDateTimeOffset(PersianDateOnly date, TimeOnly time, TimeSpan offset)
//     {
//         _dateTimeOffset = new(date, time, offset);
//     }
//
//     public PersianDateTimeOffset(int year,
//         int mouth,
//         int day,
//         int hour,
//         int minute,
//         int second,
//         int millisecond,
//         int microsecond,
//         TimeSpan offset)
//     {
//         _dateTimeOffset = new(year,
//             mouth,
//             day,
//             hour,
//             minute,
//             second,
//             millisecond,
//             microsecond,
//             PersianDefault.Calendar,
//             offset);
//     }
//
//     public DateTime ToDateTime() => _dateTimeOffset;
// }
//
// [StructLayout(LayoutKind.Sequential)]
// public readonly struct PersianDateOnly(int year, int month, int day) :
//     IComparable,
//     IComparable<PersianDateOnly>,
//     IEquatable<PersianDateOnly>,
//     ISpanFormattable,
//     ISpanParsable<PersianDateOnly>,
//     IUtf8SpanFormattable
// {
//     private readonly DateOnly _dateOnly = new(year, month, day, PersianDefault.Calendar);
//     public DateOnly ToDateOnly() => _dateOnly;
//
//     public static implicit operator DateOnly(PersianDateOnly persianDateOnly) => persianDateOnly.ToDateOnly();
// }

