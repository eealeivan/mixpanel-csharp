using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleAppendMessageBuilder : PeopleMessageBuilderBase
    {
        public PeopleAppendMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, string> SpecialPropsBindings
        {
            get { return CoreSpecialPropsBindings; }
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            IDictionary<string, object> msg = GetCoreMessageObject(messageData, 3);

            var append = new Dictionary<string, object>();
            msg[MixpanelProperty.PeopleAppend] = append;

            SetNormalProperties(append, messageData);

            return msg;
        }
    }
}