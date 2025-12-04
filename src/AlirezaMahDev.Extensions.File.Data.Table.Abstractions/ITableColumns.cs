using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public interface ITableColumns : IDataLocationItem, IDataDictionary<String64, ITableColumn>;