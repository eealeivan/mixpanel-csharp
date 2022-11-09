using System;
using System.Collections.Specialized;
using System.Web;

namespace Mixpanel.Tests.Integration
{
    internal static class UriHelper
    {
        public static Uri CreateUri(string baseUri, params (string name, string value)[] query)
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