using System;
using System.Collections.Generic;
using System.Globalization;
using Mixpanel.Exceptions;
using Mixpanel.Misc;

namespace Mixpanel.Builders
{
    internal class TrackBuilder : BuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindings =
            new Dictionary<string, string>
            {
                {"event", MixpanelProperty.Event},

                {"token", MixpanelProperty.Token},

                {"distinct_id", MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},

                {"ip", MixpanelProperty.Ip},

                {"time", MixpanelProperty.Time},
            };

        public TrackBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, object> GetObject(MixpanelData mixpanelData)
        {
            var obj = new Dictionary<string, object>();

            // event
            obj["event"] = mixpanelData.GetSpecialRequiredProp(MixpanelProperty.Event,
                x =>
                {
                    if(String.IsNullOrWhiteSpace(x.ToString()))
                        throw new MixpanelRequiredPropertyNullOrEmptyException(
                            "'event' property can't be empty.");
                },
                x => x.ToString());

            var properties = new Dictionary<string, object>();
            obj["properties"] = properties;

            // token
            properties["token"] = mixpanelData.GetSpecialRequiredProp(MixpanelProperty.Token,
                x =>
                {
                    if (String.IsNullOrWhiteSpace(x.ToString()))
                        throw new MixpanelRequiredPropertyNullOrEmptyException(
                            "'token' property can't be empty.");
                },
                x => x.ToString());

            // distinct_id
            var distinctId = mixpanelData.GetSpecialProp(MixpanelProperty.DistinctId, x => x.ToString());
            if (distinctId != null)
            {
                properties["distinct_id"] = distinctId;
            }

            // ip
            object ip = mixpanelData.GetSpecialProp(MixpanelProperty.Ip, x => x.ToString());
            if (ip != null)
            {
                properties["ip"] = ip;
            }

            // time
            object time = mixpanelData.GetSpecialProp(MixpanelProperty.Time,
                x =>
                {
                    DateTime dateTime;
                    if (DateTime.TryParseExact(x.ToString(), ValueParser.MixpanelDateFormat,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
                    {
                        return dateTime.ToUnixTime();
                    }
                    return null;
                });
            if (time != null)
            {
                properties["time"] = time;
            }

            // Other properties
            foreach (var prop in mixpanelData.Props)
            {
                properties[prop.Key] = prop.Value;
            }

            return obj;
        }
    }
}
