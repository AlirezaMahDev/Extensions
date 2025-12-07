using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

public interface IDataManager
{
    IDataAccess Open(string key);
    bool Close(string key);
    ITempDataAccess OpenTemp();
}