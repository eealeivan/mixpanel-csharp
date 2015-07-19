using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal abstract class PeopleMessageBuilderBase : MessageBuilderBase
    {
        private static readonly IDictionary<string, string> DistinctIdPropsBindingsInternal =
            new Dictionary<string, string>(3)
            {
                {MixpanelProperty.PeopleDistinctId, MixpanelProperty.PeopleDistinctId},
                {MixpanelProperty.TrackDistinctId, MixpanelProperty.PeopleDistinctId},
                {"distinctid", MixpanelProperty.PeopleDistinctId}
            };

        protected static readonly IDictionary<string, string> CoreSpecialPropsBindings =
            new Dictionary<string, string>
            {
                {MixpanelProperty.PeopleToken, MixpanelProperty.PeopleToken},
                {MixpanelProperty.TrackToken, MixpanelProperty.PeopleToken}
            };

        static PeopleMessageBuilderBase()
        {
            foreach (var binding in DistinctIdPropsBindingsInternal)
            {
                CoreSpecialPropsBindings.Add(binding.Key, binding.Value);
            }
        }

        public override IDictionary<string, string> DistinctIdPropsBindings
        {
            get { return DistinctIdPropsBindingsInternal; }
        }

        /// <summary>
        /// Creates core people message object that can be used as base for
        /// building other people messages. Sets $token and $distinct_id properties.
        /// </summary>
        /// <param name="messageData">Parsed message data.</param>
        /// <param name="messageItemsCount">
        /// If set then message dictionary will be initialized with this value as capacity.
        /// </param>
        protected IDictionary<string, object> GetCoreMessageObject(
            MessageData messageData, int? messageItemsCount = null)
        {
            var msg = messageItemsCount == null
                ? new Dictionary<string, object>()
                : new Dictionary<string, object>(messageItemsCount.Value);

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