using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleUnionMessageBuilder : PeopleMessageBuilderBase
    {
        public override IDictionary<string, string> SpecialPropsBindings
        {
            get { return CoreSpecialPropsBindings; }
        }

        public override MessagePropetiesRules MessagePropetiesRules
        {
            get { return MessagePropetiesRules.ListsOnly; }
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