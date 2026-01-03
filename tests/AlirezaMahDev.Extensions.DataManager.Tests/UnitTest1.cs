using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager.Tests;

public class UnitTest1(ITestOutputHelper helper)
{
    [Fact]
    public void Test1()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddDataManager();
        var host = builder.Build();
        var dataManager = host.Services.GetRequiredService<IDataManager>();
        using var tempDataAccess = dataManager.OpenTemp();
        var locationWrap = tempDataAccess.RootWrap.Wrap(x => x.TreeDictionary());
        locationWrap.GetOrAdd("t1");
        tempDataAccess.Flush();
        foreach (var location in locationWrap.GetChildren())
        {
            helper.WriteLine(location.Wrap(tempDataAccess).RefValue.Key.ToString());
        }
    }
}