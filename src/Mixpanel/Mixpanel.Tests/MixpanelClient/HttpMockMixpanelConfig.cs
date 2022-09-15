using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixpanel.Tests.MixpanelClient
{
    public class HttpMockMixpanelConfig
    {
        public MixpanelConfig Instance { get; }
        public List<(string Endpoint, JObject Message)> Messages { get; }

        public HttpMockMixpanelConfig(MixpanelConfig config)
        {
            Instance = config ?? new MixpanelConfig();
            Messages = new List<(string Endpoint, JObject Message)>();

            if (Instance.AsyncHttpPostFn != null)
            {
                throw new Exception("AsyncHttpPostFn is expected to be null.");
            }

            Instance.AsyncHttpPostFn = (endpoint, data) =>
            {
                Messages.Add((endpoint, ParseMessageData(data)));
                return Task.FromResult(true);
            };
        }

        private JObject ParseMessageData(string data)
        {
            // Can't use JObject.Parse because it's not possible to disable DateTime parsing
            using (var stringReader = new StringReader(GetJsonFromData(data)))
            {
                using (JsonReader jsonReader = new JsonTextReader(stringReader))
                {
                    jsonReader.DateParseHandling = DateParseHandling.None;
                    JObject msg = JObject.Load(jsonReader);
                    return msg;
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