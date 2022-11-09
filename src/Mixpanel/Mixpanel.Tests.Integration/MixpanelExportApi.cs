using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;

namespace Mixpanel.Tests.Integration
{
    public static class MixpanelExportApi
    {
        private const string ExportBaseUri = "https://data.mixpanel.com/api/2.0/export";

        private static readonly AsyncRetryPolicy<HttpResponseMessage> EmptyResponsePolicy = Policy
            .HandleResult<HttpResponseMessage>(response => response.Content.Headers.ContentLength == 0)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2));

        private static readonly HttpClient HttpClient = new HttpClient
        {
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    $"{SecretsProvider.MixpanelServiceAccountUsername}:{SecretsProvider.MixpanelServiceAccountSecret}")
            }
        };

        public static async Task<JObject> GetRecentMessage(string insertId)
        {
            var uri = UriHelper.CreateUri(
                ExportBaseUri,
                ("project_id", SecretsProvider.MixpanelProjectId),
                ("from_date", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                ("to_date", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                ("where", $"properties[\"$insert_id\"] == \"{insertId}\""));

            var response = await EmptyResponsePolicy.ExecuteAsync(async _ => await HttpClient.GetAsync(uri), new Context());
            response.EnsureSuccessStatusCode();

            string jsonl = await response.Content.ReadAsStringAsync();
            return ParseResponse(jsonl).Single();
        }

        private static List<JObject> ParseResponse(string jsonl)
        {
            List<JObject> items = new List<JObject>();

            var jsonReader = new JsonTextReader(new StringReader(jsonl))
            {
                SupportMultipleContent = true,
                DateParseHandling = DateParseHandling.None
            };
            
            while (jsonReader.Read())
            {
                items.Add(JObject.Load(jsonReader));
            }

            return items;
        }
    }
}