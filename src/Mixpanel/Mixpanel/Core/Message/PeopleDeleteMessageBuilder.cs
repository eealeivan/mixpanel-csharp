using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    /// <summary>
    /// Message builder for 'people $delete' operation.
    /// </summary>
    internal sealed class PeopleDeleteMessageBuilder : PeopleMessageBuilderBase
    {
        public PeopleDeleteMessageBuilder(MixpanelConfig config = null)
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
            msg[MixpanelProperty.PeopleDelete] = string.Empty;
            return msg;
        }
    }
}