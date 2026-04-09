#!/usr/local/share/dotnet/dotnet run
#:sdk Microsoft.NET.Sdk
#:project ../src/AlirezaMahDev.Extensions/AlirezaMahDev.Extensions.csproj
#:project ../src/AlirezaMahDev.Extensions.Abstractions/AlirezaMahDev.Extensions.Abstractions.csproj

using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.Abstractions;

const int Iterations = 100_000;
const int WarmupIterations = 10_000;
var sizes = new[] { 256, 1024, 4096, 16384, 65536, 262144, 1048576, 4194304 };

Console.WriteLine("=== Warmup ===");
RunBenchmark("SmartMemoryPool", SmartMemoryPool<int>.Shared, sizes, WarmupIterations);
RunBenchmark("MemoryPool", MemoryPool<int>.Shared, sizes, WarmupIterations);
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

Console.WriteLine("\n=== Benchmark (Rent + Dispose) ===");
Console.WriteLine($"Iterations: {Iterations:N0}\n");

RunBenchmark("SmartMemoryPool", SmartMemoryPool<int>.Shared, sizes, Iterations);
RunBenchmark("MemoryPool", MemoryPool<int>.Shared, sizes, Iterations);

Console.WriteLine("\n=== Concurrent Benchmark (4 threads) ===");
RunConcurrentBenchmark("SmartMemoryPool", SmartMemoryPool<int>.Shared, sizes, Iterations);
RunConcurrentBenchmark("MemoryPool", MemoryPool<int>.Shared, sizes, Iterations);

static void RunBenchmark(string name, MemoryPool<int> pool, int[] sizes, int iterations)
{
    Console.WriteLine($"--- {name} ---");
    foreach (var size in sizes)
    {
        var times = new long[iterations];
        for (int i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.GetTimestamp();
            var owner = pool.Rent(size);
            owner.Memory.Span[0] = 1;
            owner.Dispose();
            times[i] = Stopwatch.GetTimestamp() - sw;
        }

        var avg = times.Average() / (Stopwatch.Frequency / 1_000_000.0);
        var min = times.Min() / (Stopwatch.Frequency / 1_000_000.0);
        var max = times.Max() / (Stopwatch.Frequency / 1_000_000.0);
        var p95 = GetPercentile(times, 0.95) / (Stopwatch.Frequency / 1_000_000.0);

        Console.WriteLine($"Size {size,10:N0}: Avg={avg,8:F2}μs | Min={min,6:F2}μs | Max={max,8:F2}μs | P95={p95,7:F2}μs");
    }
}

static void RunConcurrentBenchmark(string name, MemoryPool<int> pool, int[] sizes, int iterations)
{
    Console.WriteLine($"--- {name} (4 threads) ---");
    foreach (var size in sizes)
    {
        var barrier = new Barrier(4);
        var allDone = new ManualResetEventSlim(false);
        var times = new ConcurrentQueue<long>();

        var threads = new Thread[4];
        for (int t = 0; t < 4; t++)
        {
            threads[t] = new Thread(() =>
            {
                barrier.SignalAndWait();
                var localTimes = new List<long>(iterations / 4);
                for (int i = 0; i < iterations / 4; i++)
                {
                    var sw = Stopwatch.GetTimestamp();
                    var owner = pool.Rent(size);
                    owner.Memory.Span[0] = 1;
                    owner.Dispose();
                    localTimes.Add(Stopwatch.GetTimestamp() - sw);
                }
                foreach (var t_ in localTimes) times.Enqueue(t_);
                if (t == 3) allDone.Set();
            });
            threads[t].Start();
        }
        allDone.Wait();

        var timeArray = times.ToArray();
        Array.Sort(timeArray);
        var avg = timeArray.Average() / (Stopwatch.Frequency / 1_000_000.0);
        var min = timeArray.Min() / (Stopwatch.Frequency / 1_000_000.0);
        var max = timeArray.Max() / (Stopwatch.Frequency / 1_000_000.0);
        var p95 = GetPercentile(timeArray, 0.95) / (Stopwatch.Frequency / 1_000_000.0);

        Console.WriteLine($"Size {size,10:N0}: Avg={avg,8:F2}μs | Min={min,6:F2}μs | Max={max,8:F2}μs | P95={p95,7:F2}μs");
    }
}

static long GetPercentile(long[] sortedValues, double percentile)
{
    var index = (int)Math.Ceiling(percentile * sortedValues.Length) - 1;
    return sortedValues[Math.Max(0, index)];
}