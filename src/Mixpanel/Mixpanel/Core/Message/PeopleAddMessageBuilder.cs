using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleAddMessageBuilder : PeopleMessageBuilderBase
    {
        public PeopleAddMessageBuilder(MixpanelConfig config = null)
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

            var add = new Dictionary<string, object>();
            msg[MixpanelProperty.PeopleAdd] = add;

            SetNormalProperties(add, messageData);

            return msg;
        }
    }
}