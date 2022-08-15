using System.Net;
using System.Text.RegularExpressions;

namespace Mixpanel.Parsers
{
    internal static class IpParser
    {
        private const string RegexPattern =
            @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";

        public static ValueParseResult Parse(object rawIp)
        {
            switch (rawIp)
            {
                case null:
                    return ValueParseResult.CreateFail("Can't be null.");
                case string ipString:
                    return ParseString(ipString);
                case IPAddress ipAddress:
                    return ValueParseResult.CreateSuccess(ipAddress.ToString());
                default:
                    return ValueParseResult.CreateFail(
                        "Expected types are: string (example: 192.168.0.136) or IPAddress.");
            }
        }

        private static ValueParseResult ParseString(string rawIp)
        {
            if (Regex.IsMatch(rawIp, RegexPattern))
            {
                return ValueParseResult.CreateSuccess(rawIp);
            }

            return ValueParseResult.CreateFail("Not a valid IP address.");

        }
    }
}