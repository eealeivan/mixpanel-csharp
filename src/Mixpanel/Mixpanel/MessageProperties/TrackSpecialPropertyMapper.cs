using System.Collections.Generic;
using Mixpanel.Parsers;

namespace Mixpanel.MessageProperties
{
    internal static class TrackSpecialPropertyMapper
    {
        private static readonly Dictionary<string, string> RawNameToSpecialPropertyMap;

        static TrackSpecialPropertyMapper()
        {
            RawNameToSpecialPropertyMap = new Dictionary<string, string>(new PropertyNameComparer())
            {
                {"token", TrackSpecialProperty.Token },
                {"$token", TrackSpecialProperty.Token },

                { "distinctid", TrackSpecialProperty.DistinctId },
                { "distinct_id", TrackSpecialProperty.DistinctId },
                { "$distinctid", TrackSpecialProperty.DistinctId },
                { "$distinct_id", TrackSpecialProperty.DistinctId },

                { "time", TrackSpecialProperty.Time },
                { "$time", TrackSpecialProperty.Time },

                { "ip", TrackSpecialProperty.Ip },
                { "$ip", TrackSpecialProperty.Ip },

                { "duration", TrackSpecialProperty.Duration },
                { "$duration", TrackSpecialProperty.Duration },

                {"os", TrackSpecialProperty.Os },
                {"$os", TrackSpecialProperty.Os },

                {"screenwidth", TrackSpecialProperty.ScreenWidth },
                {"screen_width", TrackSpecialProperty.ScreenWidth },
                {"$screenwidth", TrackSpecialProperty.ScreenWidth },
                {"$screen_width", TrackSpecialProperty.ScreenWidth },

                {"screenheight", TrackSpecialProperty.ScreenHeight },
                {"screen_height", TrackSpecialProperty.ScreenHeight },
                {"$screenheight", TrackSpecialProperty.ScreenHeight },
                {"$screen_height", TrackSpecialProperty.ScreenHeight }
            };
        }

        public static string RawNameToSpecialProperty(string propertyName)
        {
            return RawNameToSpecialPropertyMap.TryGetValue(propertyName, out var specialProperty)
                ? specialProperty
                : null;
        }
    }
}