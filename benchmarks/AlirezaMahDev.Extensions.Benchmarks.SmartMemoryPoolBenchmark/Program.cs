using System.Buffers;

using AlirezaMahDev.Extensions.Abstractions;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace AlirezaMahDev.Extensions.Benchmarks.SmartMemoryPoolBenchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[Config(typeof(Config))]
public class MemoryPoolBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            var job = Job.Default
                .WithRuntime(CoreRuntime.Core10_0)
                .WithLaunchCount(1)
                .WithWarmupCount(3)
                .WithIterationCount(10)
                .WithInvocationCount(1024);

            AddJob(job);
            AddColumnProvider(DefaultColumnProviders.Instance);
        }
    }

    private const int Iterations = 10;
    private static readonly int[] Sizes = [256, 1024, 4096, 16384, 65536, 262144, 1048576, 4194304];

    private readonly MemoryPool<int> _smartPool = SmartMemoryPool<int>.Shared;
    private readonly MemoryPool<int> _defaultPool = MemoryPool<int>.Shared;

    [Benchmark]
    public void SmartMemoryPool_SmallSize() => TestPool(_smartPool, 256, Iterations);

    [Benchmark]
    public void MemoryPool_SmallSize() => TestPool(_defaultPool, 256, Iterations);

    [Benchmark]
    public void SmartMemoryPool_MediumSize() => TestPool(_smartPool, 65536, Iterations);

    [Benchmark]
    public void MemoryPool_MediumSize() => TestPool(_defaultPool, 65536, Iterations);

    [Benchmark]
    public void SmartMemoryPool_LargeSize() => TestPool(_smartPool, 1048576, Iterations);

    [Benchmark]
    public void MemoryPool_LargeSize() => TestPool(_defaultPool, 1048576, Iterations);

    [Benchmark]
    public void SmartMemoryPool_VariousSizes()
    {
        foreach (var size in Sizes)
        {
            TestPool(_smartPool, size, Iterations / Sizes.Length);
        }
    }

    [Benchmark]
    public void MemoryPool_VariousSizes()
    {
        foreach (var size in Sizes)
        {
            TestPool(_defaultPool, size, Iterations / Sizes.Length);
        }
    }

    private static void TestPool(MemoryPool<int> pool, int size, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            var owner = pool.Rent(size);
            if (owner.Memory.Length > 0)
            {
                owner.Memory.Span[0] = i;
            }
            owner.Dispose();
        }
    }

    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<MemoryPoolBenchmark>();
    }
}
