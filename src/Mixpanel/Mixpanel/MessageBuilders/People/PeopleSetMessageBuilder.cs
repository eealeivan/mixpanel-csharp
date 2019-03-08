using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$ip": "123.123.123.123",
    //     "$set": {
    //         "Address": "1313 Mockingbird Lane"
    //     }
    // }

    internal static class PeopleSetMessageBuilder
    {
        public static MessageBuildResult BuildSet(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config)
        {
            return Build("$set", token, superProperties, rawProperties, distinctId, config);
        }

        public static MessageBuildResult BuildSetOnce(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config)
        {
            return Build("$set_once", token, superProperties, rawProperties, distinctId, config);
        }

        private static MessageBuildResult Build(
            string operation,
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config)
        {
            MessageCandidate messageCandidate = PeopleMessageBuilderBase.CreateValidMessageCandidate(
                token,
                superProperties,
                rawProperties,
                distinctId,
                config,
                out string messageCandidateErrorMessage);

            if (messageCandidate == null)
            {
                return MessageBuildResult.CreateFail(messageCandidateErrorMessage);
            }

            var message = new Dictionary<string, object>();
            var set = new Dictionary<string, object>();
            message[operation] = set;

            // Special properties
            PeopleMessageBuilderBase.RunForValidSpecialProperties(
                messageCandidate,
                (specialPropertyName, isMessageSpecialProperty, value) =>
                {
                    if (isMessageSpecialProperty)
                    {
                        message[specialPropertyName] = value;
                    }
                    else
                    {
                        set[specialPropertyName] = value;
                    }
                });

            // User properties
            PeopleMessageBuilderBase.RunForValidUserProperties(
                messageCandidate,
                rawValue => GenericPropertyParser.Parse(rawValue, allowCollections: true),
                (formattedPropertyName, value) =>
                {
                    set[formattedPropertyName] = value;
                });
     
            return MessageBuildResult.CreateSuccess(message);
        }
    }
}