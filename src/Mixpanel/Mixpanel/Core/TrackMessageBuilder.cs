using System;
using System.Collections.Generic;
using System.Globalization;
using Mixpanel.Exceptions;
using Mixpanel.Misc;

namespace Mixpanel.Core
{
    internal class TrackMessageBuilder : MessageBuilderBase
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

        public TrackMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, object> GetObject(ObjectData objectData)
        {
            var obj = new Dictionary<string, object>();

            // event
            obj["event"] = objectData.GetSpecialRequiredProp(MixpanelProperty.Event,
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
            properties["token"] = objectData.GetSpecialRequiredProp(MixpanelProperty.Token,
                x =>
                {
                    if (String.IsNullOrWhiteSpace(x.ToString()))
                        throw new MixpanelRequiredPropertyNullOrEmptyException(
                            "'token' property can't be empty.");
                },
                x => x.ToString());

            // distinct_id
            var distinctId = objectData.GetSpecialProp(MixpanelProperty.DistinctId, x => x.ToString());
            if (distinctId != null)
            {
                properties["distinct_id"] = distinctId;
            }

            // ip
            object ip = objectData.GetSpecialProp(MixpanelProperty.Ip, x => x.ToString());
            if (ip != null)
            {
                properties["ip"] = ip;
            }

            // time
            object time = objectData.GetSpecialProp(MixpanelProperty.Time, ConvertToUnixTime);
            if (time != null)
            {
                properties["time"] = time;
            }

            // Other properties
            foreach (var prop in objectData.Props)
            {
                properties[prop.Key] = prop.Value;
            }

            return obj;
        }
    }
}
