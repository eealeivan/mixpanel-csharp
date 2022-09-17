using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mixpanel
{
    internal sealed class DefaultHttpClient
    {
        static readonly HttpClient HttpClient = new HttpClient();

        public async Task<bool> PostAsync(string url, string formData, CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage =
                await HttpClient.PostAsync(url, new StringContent(formData), cancellationToken).ConfigureAwait(false);
            if (!responseMessage.IsSuccessStatusCode)
            {
                return false;
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseContent == "1";
        }
    }
}