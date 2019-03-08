using System;

namespace Mixpanel.Parsers
{
    internal static class DistinctIdParser
    {
        public static ValueParseResult Parse(object rawDistinctId)
        {
            if (rawDistinctId == null)
            {
                return ValueParseResult.CreateFail("Can't be null.");
            }
            
            if (!(rawDistinctId is string) &&
                !NumberParser.IsNumber(rawDistinctId) &&
                !(rawDistinctId is Guid))
            {
                return ValueParseResult.CreateFail(
                    "Expected types are: string, number (int, long etc) or Guid.");
            }

            string distinctId = rawDistinctId.ToString();
            if (string.IsNullOrWhiteSpace(distinctId))
            {
                return ValueParseResult.CreateFail("Can't be empty string.");
            }


            return ValueParseResult.CreateSuccess(distinctId);
        }
    }
}