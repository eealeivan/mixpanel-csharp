using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixpanel.Tests.MixpanelClient
{
    public class HttpMockMixpanelConfig<TMessage> where TMessage : class
    {
        public MixpanelConfig Instance { get; }
        public List<(string Endpoint, TMessage Message)> Messages { get; }
        public bool RequestCancelled { get; private set; }

        public HttpMockMixpanelConfig(MixpanelConfig config = null)
        {
            Instance = config ?? new MixpanelConfig();
            Messages = new List<(string Endpoint, TMessage Message)>();

            if (Instance.AsyncHttpPostFn != null)
            {
                throw new Exception("AsyncHttpPostFn is expected to be null.");
            }

            Instance.AsyncHttpPostFn = async (endpoint, data, cancellationToken) =>
            {
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                }

                RequestCancelled = cancellationToken.IsCancellationRequested;
                cancellationToken.ThrowIfCancellationRequested();

                Messages.Add((endpoint, ParseMessageData(data)));
                return await Task.FromResult(true);
            };
        }

        private TMessage ParseMessageData(string data)
        {
            // Can't use JObject.Parse because it's not possible to disable DateTime parsing
            using (var stringReader = new StringReader(GetJsonFromData(data)))
            {
                using (JsonReader jsonReader = new JsonTextReader(stringReader))
                {
                    jsonReader.DateParseHandling = DateParseHandling.None;

                    if (typeof(TMessage) == typeof(JObject))
                    {
                        return JObject.Load(jsonReader) as TMessage;
                    }

                    if (typeof(TMessage) == typeof(JArray))
                    {
                        return JArray.Load(jsonReader) as TMessage;
                    }

                    throw new NotSupportedException($"{typeof(TMessage)} is not supported.");
                }
            }
        }

        private string GetJsonFromData(string data)
        {
            // Remove "data=" to get raw BASE64
            string base64 = data.Remove(0, 5);
            byte[] bytesString = Convert.FromBase64String(base64);

            var json = Encoding.UTF8.GetString(bytesString, 0, bytesString.Length);
            return json;
        }
    }
}