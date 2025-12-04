using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public interface ICollectionProperties : IDataLocationItem, IDataDictionary<String64, ICollectionProperty>;