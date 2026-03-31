namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataMapFilePartOwner
{
    ref byte GetRef(int offset);
}
