using System;
using System.Collections.Generic;
using Mixpanel.Exceptions;

namespace Mixpanel.Core.Message
{
    internal class TrackMessageBuilder : MessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindings =
            new Dictionary<string, string>
            {
                {MixpanelProperty.Event, MixpanelProperty.TrackEvent},

                {MixpanelProperty.Token, MixpanelProperty.TrackToken},

                {MixpanelProperty.DistinctId, MixpanelProperty.TrackDistinctId},
                {"distinctid", MixpanelProperty.TrackDistinctId},

                {MixpanelProperty.Ip, MixpanelProperty.TrackIp},

                {MixpanelProperty.Time, MixpanelProperty.TrackTime},
            };

        public TrackMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            var msg = new Dictionary<string, object>();

            // event
            SetSpecialRequiredProperty(msg, messageData, MixpanelProperty.TrackEvent,
                x =>
                {
                    if (String.IsNullOrWhiteSpace(x.ToString()))
                        throw new MixpanelRequiredPropertyNullOrEmptyException(
                            string.Format("'event{0}' property can't be empty.", MixpanelProperty.TrackEvent));
                },
                x => x.ToString());
           

            var properties = new Dictionary<string, object>();
            msg[MixpanelProperty.TrackProperties] = properties;

            // token
            SetSpecialRequiredProperty(properties, messageData, MixpanelProperty.TrackToken,
                x =>
                {
                    if (String.IsNullOrWhiteSpace(x.ToString()))
                        throw new MixpanelRequiredPropertyNullOrEmptyException(
                            string.Format("'{0}' property can't be empty.", MixpanelProperty.TrackToken));
                },
                x => x.ToString());

            // distinct_id, ip and time
            SetSpecialProperties(properties, messageData, new Dictionary<string, Func<object, object>>
            {
                { MixpanelProperty.TrackDistinctId, x => x.ToString() },
                { MixpanelProperty.TrackIp, x => x.ToString() },
                { MixpanelProperty.TrackTime, ConvertToUnixTime }
            });

            SetNormalProperties(properties, messageData);

            return msg;
        }
    }
}
