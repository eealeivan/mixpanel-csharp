using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal abstract class PeopleMessageBuilderBase : MessageBuilderBase
    {
        protected PeopleMessageBuilderBase(MixpanelConfig config = null)
            : base(config)
        {
        }

        public static readonly Dictionary<string, string> CoreSpecialPropsBindings =
            new Dictionary<string, string>
            {
                {MixpanelProperty.PeopleToken, MixpanelProperty.PeopleToken},
                {MixpanelProperty.TrackToken, MixpanelProperty.PeopleToken},

                {MixpanelProperty.PeopleDistinctId, MixpanelProperty.PeopleDistinctId},
                {MixpanelProperty.TrackDistinctId, MixpanelProperty.PeopleDistinctId},
                {"distinctid", MixpanelProperty.PeopleDistinctId}
            };

        protected IDictionary<string, object> GetCoreMessageObject(MessageData messageData)
        {
            var msg = new Dictionary<string, object>();

            // $token
            SetSpecialRequiredProperty(msg, messageData, MixpanelProperty.PeopleToken,
                x => ThrowIfPropertyIsNullOrEmpty(x, MixpanelProperty.PeopleToken),
                x => x.ToString());


            // $distinct_id
            SetSpecialRequiredProperty(msg, messageData, MixpanelProperty.PeopleDistinctId,
                x => ThrowIfPropertyIsNullOrEmpty(x, MixpanelProperty.PeopleDistinctId),
                x => x.ToString());

            return msg;
        }
    }
}