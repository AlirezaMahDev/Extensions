using System.Diagnostics;

using AlirezaMahDev.Extensions.Brain.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Tests;

public class UnitTest1(ITestOutputHelper helper)
{
    [Fact]
    public void Test1()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddBrain();
        using var host = builder.Build();
        var brainService = host.Services.GetRequiredService<IBrainService>();
        var nerve = brainService.GetOrAddTemp<int, double>();

        var stopwatch = Stopwatch.StartNew();
        nerve.Learn(CalculateSlope(2, 4, 6, 8), new([2, 4, 6, 8]));
        helper.WriteLine($"learn1:{stopwatch.Elapsed}");

        stopwatch.Restart();
        nerve.Learn(CalculateSlope(1, 3, 5, 7), new([1, 3, 5, 7]));
        helper.WriteLine($"learn2:{stopwatch.Elapsed}");

        stopwatch.Restart();
        nerve.Flush();
        helper.WriteLine($"save:{stopwatch.Elapsed}");

        stopwatch.Restart();
        var think1 = nerve.Think(CalculateSlope(1, 3, 5), new([1, 3, 5]));
        helper.WriteLine($"think1:{stopwatch.Elapsed}");

        Assert.Equal(7, think1?.Data);

        stopwatch.Restart();
        var think2 = nerve.Think(CalculateSlope(2, 4, 6), new([2, 4, 6]));
        helper.WriteLine($"think2:{stopwatch.Elapsed}");

        Assert.Equal(8, think2?.Data);

        stopwatch.Restart();
        var think3 = nerve.Think(CalculateSlope(1, 4, 6), new([1, 4, 6]));
        helper.WriteLine($"think3:{stopwatch.Elapsed}");

        Assert.Equal(8, think3?.Data);
    }


    [Fact]
    public async Task Test1Async()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddBrain();
        using var host = builder.Build();
        var brainService = host.Services.GetRequiredService<IBrainService>();
        var nerve = brainService.GetOrAddTemp<int, double>();

        var stopwatch = Stopwatch.StartNew();
        await nerve.LearnAsync(CalculateSlope(2, 4, 6, 8), new([2, 4, 6, 8]));
        helper.WriteLine($"learn1:{stopwatch.Elapsed}");

        stopwatch.Restart();
        await nerve.LearnAsync(CalculateSlope(1, 3, 5, 7), new([1, 3, 5, 7]));
        helper.WriteLine($"learn2:{stopwatch.Elapsed}");

        stopwatch.Restart();
        nerve.Flush();
        helper.WriteLine($"save:{stopwatch.Elapsed}");

        stopwatch.Restart();
        var think1 = await nerve.ThinkAsync(CalculateSlope(1, 3, 5), new([1, 3, 5]));
        helper.WriteLine($"think1:{stopwatch.Elapsed}");

        Assert.Equal(7, think1?.Data);

        stopwatch.Restart();
        var think2 = await nerve.ThinkAsync(CalculateSlope(2, 4, 6), new([2, 4, 6]));
        helper.WriteLine($"think2:{stopwatch.Elapsed}");

        Assert.Equal(8, think2?.Data);

        stopwatch.Restart();
        var think3 = await nerve.ThinkAsync(CalculateSlope(1, 4, 6), new([1, 4, 6]));
        helper.WriteLine($"think3:{stopwatch.Elapsed}");

        Assert.Equal(8, think3?.Data);
    }

    private static double CalculateSlope(params ReadOnlySpan<int> readOnlySpan)
    {
        var xSum = 0d;
        var ySum = 0d;
        var xySum = 0d;
        var xSquaredSum = 0d;
        var n = readOnlySpan.Length;
        for (int i = 0; i < n; i++)
        {
            var x = i;
            var y = readOnlySpan[i];
            xSum += x;
            ySum += y;
            xySum += x * y;
            xSquaredSum += x * x;
        }

        var slope = (n * xySum - xSum * ySum) / (n * xSquaredSum - xSum * xSum);
        return slope;
    }
}