using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Mixpanel
{
    internal sealed class DefaultHttpClient
    {
        public bool Post(string url, string formData)
        {
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
        }

#if !(NET40 || NET35)
        public async Task<bool> PostAsync(string url, string formData)
        {
            var req = CreateWebRequest(url);
            using (var reqStream = await req.GetRequestStreamAsync())
            {
                using (var writer = new StreamWriter(reqStream))
                {
                    writer.Write(formData);
                }
            }

            using (var res = await req.GetResponseAsync())
            {
                using (var resStream = res.GetResponseStream())
                {
                    using (var reader = new StreamReader(resStream))
                    {
                        return await reader.ReadToEndAsync() == "1";
                    }
                }
            }
        }
#endif


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
    }
}