using System;
using System.Collections.Generic;
using Mixpanel.Exceptions;

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
                x =>
                {
                    if (String.IsNullOrWhiteSpace(x.ToString()))
                        throw new MixpanelRequiredPropertyNullOrEmptyException(
                            string.Format("'{0}' property can't be empty.", MixpanelProperty.PeopleToken));
                },
                x => x.ToString());


            // $distinct_id
            SetSpecialRequiredProperty(msg, messageData, MixpanelProperty.PeopleDistinctId,
                x =>
                {
                    if (String.IsNullOrWhiteSpace(x.ToString()))
                        throw new MixpanelRequiredPropertyNullOrEmptyException(
                            string.Format("'{0}' property can't be empty.", MixpanelProperty.PeopleDistinctId));
                },
                x => x.ToString());

            return msg;
        }
    }
}