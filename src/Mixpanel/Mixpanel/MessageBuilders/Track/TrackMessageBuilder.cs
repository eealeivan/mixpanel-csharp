using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.Track
{
    // Message example:
    // {
    //     "event": "Signed Up",
    //     "properties": {
    //         "token": "e3bc4100330c35722740fb8c6f5abddc",
    //         "distinct_id": "13793",
    //         "Referred By": "Friend"
    //     }
    // }

    internal static class TrackMessageBuilder
    {
        public static MessageBuildResult Build(
            string token,
            string @event,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config)
        {
            if (string.IsNullOrWhiteSpace(@event))
            {
                return MessageBuildResult.CreateFail($"'{nameof(@event)}' is not set.");
            }

            MessageCandidate messageCandidate = TrackMessageBuilderBase.CreateValidMessageCandidate(
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

            var message = new Dictionary<string, object>(2);
            message["event"] = @event;

            var properties = new Dictionary<string, object>();
            message["properties"] = properties;

            // Special properties
            foreach (KeyValuePair<string, ObjectProperty> pair in messageCandidate.SpecialProperties)
            {
                string specialPropertyName = pair.Key;
                ObjectProperty objectProperty = pair.Value;

                ValueParseResult result = 
                    TrackSpecialPropertyParser.Parse(specialPropertyName, objectProperty.Value);
                if (!result.Success)
                {
                    // The only required special properties are 'event' and 'token' which are controlled separately
                    continue;
                }

                properties[specialPropertyName] = result.Value;
            }

            // User properties
            foreach (KeyValuePair<string, ObjectProperty> pair in messageCandidate.UserProperties)
            {
                string formattedPropertyName = pair.Key;
                ObjectProperty objectProperty = pair.Value;

                ValueParseResult result;

                if (config.CustomPropertiesParser != null)
                {
                    result = config.CustomPropertiesParser(objectProperty.Value, true);
                }
                else
                {
                    result = GenericPropertyParser.Parse(objectProperty.Value, allowCollections: true);
                }

                if (!result.Success)
                {
                    continue;
                }

                properties[formattedPropertyName] = result.Value;
            }

            return MessageBuildResult.CreateSuccess(message);
        }
    }
}