using System;
using System.Globalization;

namespace Mixpanel.Parsers
{
    internal static class TimeParser
    {
        private const long UnixEpoch = 621355968000000000L;
        private const string MixpanelDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public static ValueParseResult ParseUnix(object rawDateTime)
        {
            switch (rawDateTime)
            {
                case null:
                    return ValueParseResult.CreateFail("Can't be null.");

                case DateTime dateTime:
                    return ValueParseResult.CreateSuccess(
                        ToUnixTime(dateTime.ToUniversalTime().Ticks));

                case DateTimeOffset dateTimeOffset:
                    return ValueParseResult.CreateSuccess(
                        ToUnixTime(dateTimeOffset.ToUniversalTime().Ticks));

                case long unixTime:
                    return ValueParseResult.CreateSuccess(unixTime);

                default:
                    return ValueParseResult.CreateFail(
                        "Expected types are: DateTime, DateTimeOffset or long (unix time).");
            }

            long ToUnixTime(long ticks)
            {
                return (ticks - UnixEpoch) / TimeSpan.TicksPerSecond;
            }
        }

        public static ValueParseResult ParseMixpanelFormat(object rawDateTime)
        {
            switch (rawDateTime)
            {
                case null:
                    return ValueParseResult.CreateFail("Can't be null.");

                case DateTime dateTime:
                    return ValueParseResult.CreateSuccess(
                        dateTime.ToUniversalTime().ToString(MixpanelDateFormat));

                case DateTimeOffset dateTimeOffset:
                    return ValueParseResult.CreateSuccess(
                        dateTimeOffset.ToUniversalTime().ToString(MixpanelDateFormat));

                case string str:
                    bool isValidDateTimeString = DateTime.TryParseExact(
                        str,
                        MixpanelDateFormat,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal,
                        out var _);
                    return isValidDateTimeString
                        ? ValueParseResult.CreateSuccess(str)
                        : ValueParseResult.CreateFail($"Expected date time format is: '{MixpanelDateFormat}'."); 

                default:
                    return ValueParseResult.CreateFail(
                        "Expected types are: DateTime, DateTimeOffset or correctly formatted Mixpanel date string (yyyy-MM-ddTHH:mm:ss).");
            }
        }
    }
}