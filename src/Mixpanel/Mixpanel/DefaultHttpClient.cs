using System;
using System.IO;
using System.Net;
#if !(NET40 || NET35)
using System.Threading.Tasks;
#endif


namespace Mixpanel
{
    internal sealed class DefaultHttpClient
    {
        public bool Post(string url, string formData)
        {
#if (PORTABLE || PORTABLE40)
            throw new NotImplementedException(
                "There is no default HTTP POST method in portable builds. Please use configuration to set it.");
#else
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
#endif
        }

#if !(NET40 || NET35)
        public async Task<bool> PostAsync(string url, string formData)
        {
#if (PORTABLE || PORTABLE40)
            throw new NotImplementedException(
                "There is no default async HTTP POST method in portable builds. Please use configuration to set it.");
#else
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
#endif
        }
#endif

#if !(PORTABLE || PORTABLE40)
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