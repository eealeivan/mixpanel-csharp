using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleUnsetMessageBuilder : PeopleMessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindingsInternal =
            new Dictionary<string, string>
            {
                {MixpanelProperty.PeopleUnset, MixpanelProperty.PeopleUnset}
            };

        static PeopleUnsetMessageBuilder()
        {
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindingsInternal.Add(binding.Key, binding.Value);
            }
        }

        public PeopleUnsetMessageBuilder(MixpanelConfig config = null)
            : base(config)
        {
        }

        public override IDictionary<string, string> SpecialPropsBindings
        {
            get { return SpecialPropsBindingsInternal; }
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            IDictionary<string, object> msg = GetCoreMessageObject(messageData, 3);

            // $unset 
            SetSpecialRequiredProperty(msg, messageData, MixpanelProperty.PeopleUnset);

            return msg;
        }
    }
}