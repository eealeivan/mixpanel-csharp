using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;

namespace Benchmarks.Json
{
    [HtmlExporter]
    [MemoryDiagnoser]
    public class DictionaryBenchmark
    {
        private const int N = 1000;
        private readonly Dictionary<string, object>[] dictionaries;

        public DictionaryBenchmark()
        {
            dictionaries = new Dictionary<string, object>[N];
            for (int i = 0; i < N; i++)
            {
                dictionaries[i] = MessageCreator.Dynamic();
            }
        }

        [Benchmark]
        public long Default()
        {
            long totalLength = 0L;
            foreach (var dictionary in dictionaries)
            {
                string json = Mixpanel.Json.MixpanelJsonSerializer.Serialize(dictionary);
                totalLength += json.Length;
            }

            return totalLength;
        }

        [Benchmark]
        public long JsonNet()
        {
            long totalLength = 0L;
            foreach (var dictionary in dictionaries)
            {
                string json = JsonConvert.SerializeObject(dictionary);
                totalLength += json.Length;
            }

            return totalLength;
        }
    }
}