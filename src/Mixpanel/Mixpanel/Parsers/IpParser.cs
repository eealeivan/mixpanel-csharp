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
            if (rawIp == null)
            {
                return ValueParseResult.CreateFail("Can't be null.");
            }

            if (rawIp is string)
            {
                return ParseString((string)rawIp);
            }

#if !NETSTANDARD11

            if (rawIp is IPAddress)
            {
                return ValueParseResult.CreateSuccess(((IPAddress)rawIp).ToString());
            }

            return ValueParseResult.CreateFail(
                "Expected types are: string (example: 192.168.0.136) or IPAddress.");

#else

            return ValueParseResult.CreateFail(
                "Expected types are: string (example: 192.168.0.136).");

#endif


        }

        private static ValueParseResult ParseString(string rawIp)
        {
            if (!Regex.IsMatch(rawIp, RegexPattern))
            {
                return ValueParseResult.CreateFail("Not a valid IP address.");
            }

            return ValueParseResult.CreateSuccess(rawIp);
        }
    }
}