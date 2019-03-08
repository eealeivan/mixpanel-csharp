using System;

namespace Mixpanel.Parsers
{
    internal static class GenericPropertyParser
    {
        public static ValueParseResult Parse(object rawValue, bool allowCollections = false)
        {
            if (rawValue == null ||
                rawValue is char ||
                rawValue is string || 
                rawValue is bool ||
                NumberParser.IsNumber(rawValue) ||
                rawValue is Guid ||
                rawValue is TimeSpan)
            {
                return ValueParseResult.CreateSuccess(rawValue);
            }

            if (rawValue is DateTime || rawValue is DateTimeOffset)
            {
                return TimeParser.ParseMixpanelFormat(rawValue);
            }

            if (allowCollections && CollectionParser.IsCollection(rawValue))
            {
                return CollectionParser.Parse(rawValue, _ => Parse(_, allowCollections: false));
            }

            return ValueParseResult.CreateFail(
                "Expected types are: string, bool, char, number (int, double etc), Guid, DateTime, DateTimeOffset or TimeSpan.");
        }
    }
}