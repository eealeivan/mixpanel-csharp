using System;
using BenchmarkDotNet.Running;
using Benchmarks.Json;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary1 = BenchmarkRunner.Run<DictionaryBenchmark>();
            //var summary2 = BenchmarkRunner.Run<DictionaryArrayBenchmark>();
            BenchmarkRunner.Run<ImprovementsBenchmark>();
        }
    }
}
