using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;

namespace Benchmarks.Json
{
    [HtmlExporter]
    [MemoryDiagnoser]
    public class DictionaryArrayBenchmark
    {
        private const int N = 1000;
        private const int ArraySize = 10;
        private readonly List<Dictionary<string, object>[]> dictionaryArrays;

        public DictionaryArrayBenchmark()
        {
            dictionaryArrays = new List<Dictionary<string, object>[]>();
            for (int i = 0; i < N; i++)
            {
                var dictionaryArray = new Dictionary<string, object>[ArraySize];
                for (int j = 0; j < ArraySize; j++)
                {
                    dictionaryArray[j] = MessageCreator.Dictionary();
                }

                dictionaryArrays.Add(dictionaryArray);
            }
        }

        [Benchmark]
        public long Default()
        {
            long totalLength = 0L;
            foreach (Dictionary<string, object>[] dictionaryArray in dictionaryArrays)
            {
                string json = Mixpanel.Json.MixpanelJsonSerializer.Serialize(dictionaryArray);
                totalLength += json.Length;
            }

            return totalLength;
        }

        [Benchmark]
        public long JsonNet()
        {
            long totalLength = 0L;
            foreach (Dictionary<string, object>[] dictionaryArray in dictionaryArrays)
            {
                string json = JsonConvert.SerializeObject(dictionaryArray);
                totalLength += json.Length;
            }

            return totalLength;
        }
    }
}