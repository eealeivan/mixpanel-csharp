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
            if (string.IsNullOrWhiteSpace(token))
            {
                return MessageBuildResult.CreateFail("'token' is not set.");
            }

            if (string.IsNullOrWhiteSpace(@event))
            {
                return MessageBuildResult.CreateFail("'event' is not set.");
            }

            var message = new Dictionary<string, object>(2);
            message["event"] = @event;

            var properties = new Dictionary<string, object>();
            message["properties"] = properties;

            var messageCandidate = new MessageCandidate(
                token, 
                superProperties,
                rawProperties,
                distinctId, 
                config,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            // Special properties
            foreach (KeyValuePair<string, ObjectProperty> pair in messageCandidate.SpecialProperties)
            {
                string specialPropertyName = pair.Key;
                ObjectProperty objectProperty = pair.Value;

                ValueParseResult result = TrackSpecialPropertyParser.Parse(specialPropertyName, objectProperty.Value);
                if (!result.Success)
                {
                    // The only required special properties are 'event' and 'token' which are set separately
                    continue;
                }

                properties[specialPropertyName] = result.Value;
            }

            // User properties
            foreach (KeyValuePair<string, ObjectProperty> pair in messageCandidate.UserProperties)
            {
                string formattedPropertyName = pair.Key;
                ObjectProperty objectProperty = pair.Value;

                ValueParseResult result = GenericPropertyParser.Parse(objectProperty.Value, allowCollections: true);
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