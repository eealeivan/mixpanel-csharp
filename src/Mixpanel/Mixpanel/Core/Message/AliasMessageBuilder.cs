using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal class AliasMessageBuilder : TrackMessageBuilderBase
    {
        private static readonly IDictionary<string, string> SpecialPropsBindingsInternal =
            new Dictionary<string, string>
            {
                {MixpanelProperty.Alias, MixpanelProperty.TrackAlias}
            };

        static AliasMessageBuilder()
        {
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindingsInternal.Add(binding.Key, binding.Value);
            }
        }
        
        public override IDictionary<string, string> SpecialPropsBindings
        {
            get { return SpecialPropsBindingsInternal; }
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            messageData.SetProperty(MixpanelProperty.TrackEvent, MixpanelProperty.TrackCreateAlias);

            var msg = GetCoreMessageObject(messageData);
            var properties = (IDictionary<string, object>)msg[MixpanelProperty.TrackProperties];

            // distinct_id
            SetSpecialRequiredProperty(properties, messageData, MixpanelProperty.TrackDistinctId,
                x => ThrowIfPropertyIsNullOrEmpty(x, MixpanelProperty.TrackDistinctId),
                x => x.ToString());

            // alias
            SetSpecialRequiredProperty(properties, messageData, MixpanelProperty.TrackAlias,
                x => ThrowIfPropertyIsNullOrEmpty(x, MixpanelProperty.TrackAlias),
                x => x.ToString());

            return msg;
        }
    }
}