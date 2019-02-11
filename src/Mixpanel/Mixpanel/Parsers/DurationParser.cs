using System;

namespace Mixpanel.Parsers
{
    internal static class DurationParser
    {
        public static ValueParseResult Parse(object rawDuration)
        {
            if (rawDuration == null)
            {
                return ValueParseResult.CreateFail("Can't be null.");
            }

            if (rawDuration is TimeSpan timeSpan)
            {
                return ValueParseResult.CreateSuccess(timeSpan.TotalSeconds);
            }

            if (NumberParser.IsNumber(rawDuration))
            {
                return ValueParseResult.CreateSuccess(rawDuration);
            }

            return ValueParseResult.CreateFail(
                "Expected types are: TimeSpan or number (double, int etc).");
        }
    }
}