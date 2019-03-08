using System.Collections.Generic;
using Mixpanel.MessageProperties;

namespace Mixpanel.MessageBuilders.People
{
    internal static class PeopleUnsetMessageBuilder
    {
        // Message example:
        // {
        //     "$token": "36ada5b10da39a1347559321baf13063",
        //     "$distinct_id": "13793",
        //     "$unset": [ "Days Overdue" ]
        // }

        public static MessageBuildResult Build(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            IEnumerable<string> propertyNames,
            object distinctId,
            MixpanelConfig config)
        {
            MessageCandidate messageCandidate = PeopleMessageBuilderBase.CreateValidMessageCandidate(
                token,
                superProperties,
                null,
                distinctId,
                config,
                out string messageCandidateErrorMessage);

            if (messageCandidate == null)
            {
                return MessageBuildResult.CreateFail(messageCandidateErrorMessage);
            }

            var message = new Dictionary<string, object>();

            // Special properties
            PeopleMessageBuilderBase.RunForValidSpecialProperties(
                messageCandidate,
                (specialPropertyName, isMessageSpecialProperty, value) =>
                {
                    // Ignore non-message specific special properties as they are not valid in profile update messages
                    if (isMessageSpecialProperty)
                    {
                        message[specialPropertyName] = value;
                    }
                });

            message["$unset"] = propertyNames ?? new string[0];

            return MessageBuildResult.CreateSuccess(message);
        }
    }
}