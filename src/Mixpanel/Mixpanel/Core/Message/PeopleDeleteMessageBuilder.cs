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

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            IDictionary<string, object> obj = GetCoreMessageObject(messageData);
            obj[MixpanelProperty.PeopleDelete] = string.Empty;
            return obj;
        }
    }
}