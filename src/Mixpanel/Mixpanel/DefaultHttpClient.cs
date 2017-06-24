using System;
using System.IO;
using System.Net;
#if HTTP_CLIENT
using System.Net.Http;
#endif
#if ASYNC
using System.Threading.Tasks;
#endif


namespace Mixpanel
{
    internal sealed class DefaultHttpClient
    {
#if HTTP_CLIENT
        static readonly HttpClient HttpClient = new HttpClient();
#endif

        public bool Post(string url, string formData)
        {
#if HTTP_CLIENT
            HttpResponseMessage responseMessage = HttpClient.PostAsync(url, new StringContent(formData)).Result;
            if (!responseMessage.IsSuccessStatusCode)
            {
                return false;
            }

            string responseContent = responseMessage.Content.ReadAsStringAsync().Result;
            return responseContent == "1";
#elif WEB_REQUEST
            var req = CreateWebRequest(url);

            using (var reqStream = req.GetRequestStream())
            {
                using (var writer = new StreamWriter(reqStream))
                {
                    writer.Write(formData);
                }
            }

            using (var res = req.GetResponse())
            {
                using (var resStream = res.GetResponseStream())
                {
                    using (var reader = new StreamReader(resStream))
                    {
                        return reader.ReadToEnd() == "1";
                    }
                }
            }
#else
            throw new NotImplementedException(
                "There is no default HTTP POST method in this build of Mixpanel C#. Please use configuration to set it.");
#endif
        }

#if ASYNC
        public async Task<bool> PostAsync(string url, string formData)
        {
#if HTTP_CLIENT
            HttpResponseMessage responseMessage =
                await HttpClient.PostAsync(url, new StringContent(formData)).ConfigureAwait(false);
            if (!responseMessage.IsSuccessStatusCode)
            {
                return false;
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseContent == "1";
#elif WEB_REQUEST
            HttpWebRequest req = CreateWebRequest(url);
            using (Stream reqStream = await req.GetRequestStreamAsync().ConfigureAwait(false))
            {
                using (var writer = new StreamWriter(reqStream))
                {
                    writer.Write(formData);
                }
            }
            
            using (WebResponse res = await req.GetResponseAsync().ConfigureAwait(false))
            {
                using (Stream resStream = res.GetResponseStream())
                {
                    using (var reader = new StreamReader(resStream))
                    {
                        return await reader.ReadToEndAsync().ConfigureAwait(false) == "1";
                    }
                }
            }
#else
             throw new NotImplementedException(
                "There is no default HTTP POST method in this build of Mixpanel C#. Please use configuration to set it.");
#endif
        }
#endif

#if WEB_REQUEST
        private HttpWebRequest CreateWebRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "*/*";
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return req;
        }
#endif
    }
}