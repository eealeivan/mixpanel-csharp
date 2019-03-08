using Mixpanel.MessageProperties;

namespace Mixpanel.Parsers
{
    internal static class TrackSpecialPropertyParser
    {
        public static ValueParseResult Parse(
            string specialPropertyName,
            object rawValue)
        {
            switch (specialPropertyName)
            {
                case TrackSpecialProperty.Token:
                    return StringParser.Parse(rawValue);

                case TrackSpecialProperty.DistinctId:
                    return DistinctIdParser.Parse(rawValue);

                case TrackSpecialProperty.Time:
                    return TimeParser.ParseUnix(rawValue);

                case TrackSpecialProperty.Ip:
                    return IpParser.Parse(rawValue);

                case TrackSpecialProperty.Duration:
                    return DurationParser.Parse(rawValue);

                case TrackSpecialProperty.Os:
                    return StringParser.Parse(rawValue);

                case TrackSpecialProperty.ScreenWidth:
                case TrackSpecialProperty.ScreenHeight:
                    return NumberParser.Parse(rawValue);

                default:
                    return ValueParseResult.CreateFail($"No parser for '{nameof(specialPropertyName)}'.");
            }
        }
    }
}