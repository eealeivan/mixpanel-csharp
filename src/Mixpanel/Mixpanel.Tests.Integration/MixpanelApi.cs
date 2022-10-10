using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Polly;
using Polly.Retry;

namespace Mixpanel.Tests.Integration
{
    public static class MixpanelApi
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
                    $"{SecretsManager.MixpanelServiceAccountUsername}:{SecretsManager.MixpanelServiceAccountSecret}")
            }
        };

        public static async Task<string> GetRecentEvent(string projectId, string insertId)
        {
            var uri = CreateUri(
                ExportBaseUri,
                ("project_id", projectId),
                ("from_date", DateTime.Now.ToString("yyyy-MM-dd")),
                ("to_date", DateTime.Now.ToString("yyyy-MM-dd")),
                ("where", $"properties[\"$insert_id\"] == \"{insertId}\""));

            var pollyContext = new Context("Retry 503");

            var response = await EmptyResponsePolicy.ExecuteAsync(async _ => await HttpClient.GetAsync(uri), pollyContext);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private static Uri CreateUri(string baseUri, params (string name, string value)[] query)
        {
            var uriBuilder = new UriBuilder(new Uri(baseUri));

            NameValueCollection finalQuery = HttpUtility.ParseQueryString(string.Empty);
            foreach ((string name, string value) in query)
            {
                finalQuery.Add(name, value);
            }

            uriBuilder.Query = finalQuery.ToString();

            return uriBuilder.Uri;
        }
    }
}