using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Abstractions;

using Microsoft.Extensions.Hosting;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataService(IFileService fileService, IHostEnvironment hostEnvironment) : IDataService
{
    public IDataAccess Default { get; }
        = fileService.Access($"{hostEnvironment.ApplicationName}.db").AsData();
}