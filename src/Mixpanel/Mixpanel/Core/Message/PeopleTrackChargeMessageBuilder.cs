using System.Collections.Generic;

namespace Mixpanel.Core.Message
{
    internal sealed class PeopleTrackChargeMessageBuilder : PeopleMessageBuilderBase
    {
        public static readonly Dictionary<string, string> SpecialPropsBindings =
            new Dictionary<string, string>
            {
                {MixpanelProperty.PeopleTime, MixpanelProperty.PeopleTime},
                {MixpanelProperty.TrackTime, MixpanelProperty.PeopleTime},
                
                {MixpanelProperty.PeopleAmount, MixpanelProperty.PeopleAmount}
            };

        static PeopleTrackChargeMessageBuilder()
        {
            foreach (var binding in CoreSpecialPropsBindings)
            {
                SpecialPropsBindings.Add(binding.Key, binding.Value);
            }
        }

        public override IDictionary<string, object> GetMessageObject(MessageData messageData)
        {
            IDictionary<string, object> msg = GetCoreMessageObject(messageData);

            var append = new Dictionary<string, object>(1);
            msg[MixpanelProperty.PeopleAppend] = append;

            var transactions = new Dictionary<string, object>(2);
            append[MixpanelProperty.PeopleTransactions] = transactions;

            SetSpecialRequiredProperty(transactions, messageData, MixpanelProperty.PeopleTime);
            SetSpecialRequiredProperty(transactions, messageData, MixpanelProperty.PeopleAmount);

            return msg;
        }
    }
}