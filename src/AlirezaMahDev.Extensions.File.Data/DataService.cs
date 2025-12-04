using Microsoft.Extensions.Hosting;

using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataService(IFileService fileService, IHostEnvironment hostEnvironment) : IDataService
{
    public IDataAccess Default { get; }
        = fileService.Access($"{hostEnvironment.ApplicationName}.db").AsData();
}