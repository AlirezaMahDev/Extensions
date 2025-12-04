using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataLocationFactory(IServiceProvider provider, DataAccess access)
    : ParameterInstanceFactory<DataLocation, DataLocationArgs>(provider)
{
    public DataLocation GetOrCreate(long parameter)
    {
        return base.GetOrCreate(new(access, parameter));
    }
}