using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Benchmarks.Json;

[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 0, targetCount: 5)]
[MemoryDiagnoser]
public class ImprovementsBenchmark
{
    private const int N = 250;
    private readonly Dictionary<string, object> message;

    public ImprovementsBenchmark()
    {
        message = MessageCreator.Static();
    }

    [Benchmark]
    public long Existing()
    {
        long totalLength = 0L;

        for (int i = 0; i < N; i++)
        {
            var json = Mixpanel.Json.MixpanelJsonSerializer.Serialize(message);
            totalLength += json.Length;
        }

        return totalLength;
    }

    [Benchmark]
    public long Optimized()
    {
        long totalLength = 0L;

        for (int i = 0; i < N; i++)
        {
            var json = Mixpanel.Json.MixpanelJsonSerializerOptimized.Serialize(message);
            totalLength += json.Length;
        }

        return totalLength;
    }
}