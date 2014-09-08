using System;
using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleSetOnceMessageBuilder : PeopleMessageBuilderBase
    {
        private static readonly Dictionary<string, string> SpecialPropsBindingsInternal =
           new Dictionary<string, string>
            {
                {MixpanelProperty.PeopleIgnoreTime, MixpanelProperty.PeopleIgnoreTime},
                {"ignore_time", MixpanelProperty.PeopleIgnoreTime},
                {"ignoretime", MixpanelProperty.PeopleIgnoreTime}
            };

        static PeopleSetOnceMessageBuilder()
        {
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindingsInternal.Add(binding.Key, binding.Value);
            }
        }

        public PeopleSetOnceMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, string> SpecialPropsBindings
        {
            get { return SpecialPropsBindingsInternal; }
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            IDictionary<string, object> msg = GetCoreMessageObject(messageData);

            // $ignore_time
            SetSpecialProperties(msg, messageData, new Dictionary<string, Func<object, object>>
            {
                { MixpanelProperty.PeopleIgnoreTime, null }
            });

            // $set_once
            var setOnce = new Dictionary<string, object>();
            msg[MixpanelProperty.PeopleSetOnce] = setOnce;

            SetNormalProperties(setOnce, messageData);

            return msg;
        }
    }
}