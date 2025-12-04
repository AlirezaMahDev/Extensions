using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal record struct TableDataArgs(TableRow Row, int Index, String64 Key);