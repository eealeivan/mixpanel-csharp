using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleUnionMessageBuilder : PeopleMessageBuilderBase
    {
        public PeopleUnionMessageBuilder(MixpanelConfig config = null)
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

            var union = new Dictionary<string, object>();
            msg[MixpanelProperty.PeopleUnion] = union;

            SetNormalProperties(union, messageData);

            return msg;
        }
    }
}