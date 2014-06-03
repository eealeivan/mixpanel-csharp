using System;
using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleSetMessageBuilder : PeopleMessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindings =
           new Dictionary<string, string>
            {
                {MixpanelProperty.PeopleIp, MixpanelProperty.PeopleIp},
                {MixpanelProperty.TrackIp, MixpanelProperty.PeopleIp},

                {MixpanelProperty.PeopleTime, MixpanelProperty.PeopleTime},
                {MixpanelProperty.TrackTime, MixpanelProperty.PeopleTime},
                
                {MixpanelProperty.PeopleIgnoreTime, MixpanelProperty.PeopleIgnoreTime},
                {"ignore_time", MixpanelProperty.PeopleIgnoreTime},
                {"ignoretime", MixpanelProperty.PeopleIgnoreTime},

                {MixpanelProperty.PeopleFirstName, MixpanelProperty.PeopleFirstName},
                {"first_name", MixpanelProperty.PeopleFirstName},
                {"firstname", MixpanelProperty.PeopleFirstName}, 
                
                {MixpanelProperty.PeopleLastName, MixpanelProperty.PeopleLastName},
                {"last_name", MixpanelProperty.PeopleLastName},
                {"lastname", MixpanelProperty.PeopleLastName},

                {MixpanelProperty.PeopleName, MixpanelProperty.PeopleName},
                {"name", MixpanelProperty.PeopleName},

                {MixpanelProperty.PeopleCreated, MixpanelProperty.PeopleCreated},
                {"created", MixpanelProperty.PeopleCreated},

                {MixpanelProperty.PeopleEmail, MixpanelProperty.PeopleEmail},
                {"email", MixpanelProperty.PeopleEmail},
                {"e-mail", MixpanelProperty.PeopleEmail},

                {MixpanelProperty.PeoplePhone, MixpanelProperty.PeoplePhone},
                {"phone", MixpanelProperty.PeoplePhone},
            };

        static PeopleSetMessageBuilder()
        {
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindings.Add(binding.Key, binding.Value);
            }
        }

        public PeopleSetMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            IDictionary<string, object> msg = GetCoreMessageObject(messageData);

            // $ip, $time and $ignore_time
            SetSpecialProperties(msg, messageData, new Dictionary<string, Func<object, object>>
            {
                { MixpanelProperty.PeopleIp, null },
                { MixpanelProperty.PeopleTime, ConvertToUnixTime },
                { MixpanelProperty.PeopleIgnoreTime, null }
            });
            
            // $set
            var set = new Dictionary<string, object>();
            msg[MixpanelProperty.PeopleSet] = set;

            SetSpecialProperties(set, messageData, new Dictionary<string, Func<object, object>>
            {
                { MixpanelProperty.PeopleFirstName, null },
                { MixpanelProperty.PeopleLastName, null },
                { MixpanelProperty.PeopleName, null },
                { MixpanelProperty.PeopleCreated, null },
                { MixpanelProperty.PeopleEmail, null },
                { MixpanelProperty.PeoplePhone, null },
            });

            SetNormalProperties(set, messageData);

            return msg;
        }
    }
}