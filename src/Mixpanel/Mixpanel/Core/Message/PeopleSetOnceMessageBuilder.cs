using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleSetOnceMessageBuilder : PeopleMessageBuilderBase
    {
        public PeopleSetOnceMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, string> SpecialPropsBindings
        {
            get { return CoreSpecialPropsBindings; }
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            IDictionary<string, object> msg = GetCoreMessageObject(messageData);

            // $set_once
            var setOnce = new Dictionary<string, object>();
            msg[MixpanelProperty.PeopleSetOnce] = setOnce;

            SetNormalProperties(setOnce, messageData);

            return msg;
        }
    }
}