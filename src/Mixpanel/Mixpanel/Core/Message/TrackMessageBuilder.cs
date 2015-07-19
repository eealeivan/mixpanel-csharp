using System;
using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal class TrackMessageBuilder : TrackMessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindingsInternal =
            new Dictionary<string, string>
            {
                {MixpanelProperty.Ip, MixpanelProperty.TrackIp},
                {MixpanelProperty.Time, MixpanelProperty.TrackTime},
            };

        static TrackMessageBuilder()
        {
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindingsInternal.Add(binding.Key, binding.Value);
            }
        }
        
        public override IDictionary<string, string> SpecialPropsBindings
        {
            get { return SpecialPropsBindingsInternal; }
        }

        public override SuperPropertiesRules SuperPropertiesRules
        {
            get { return SuperPropertiesRules.All; }
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            var msg = GetCoreMessageObject(messageData);
            var properties = (IDictionary<string, object>)msg[MixpanelProperty.TrackProperties];

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
