using System.IO;
using System.Net;

namespace Mixpanel
{
    internal sealed class DefaultHttpClient
    {
        public bool Post(string url, string formData)
        {
            var req = WebRequest.CreateHttp(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "*/*";
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

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
    }
}