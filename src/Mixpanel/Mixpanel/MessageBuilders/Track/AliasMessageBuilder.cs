using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.Track
{
    // Message example:
    // {
    //     "event": "$create_alias",
    //     "properties": {
    //         "token": "e3bc4100330c35722740fb8c6f5abddc",
    //         "distinct_id": "ORIGINAL_ID",
    //         "alias": "NEW_ID"
    //     }
    // }

    internal static class AliasMessageBuilder
    {
        public static MessageBuildResult Build(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object distinctId, 
            object alias)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return MessageBuildResult.CreateFail("'token' is not set.");
            }

            var message = new Dictionary<string, object>(2);
            message["event"] = "$create_alias";

            var properties = new Dictionary<string, object>(3);
            message["properties"] = properties;

            // token
            properties["token"] = token;

            var messageCandidate = new MessageCandidate(
                token, 
                superProperties, 
                null,
                distinctId, 
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            // distinct_id
            ObjectProperty rawDistinctId = messageCandidate.GetSpecialProperty(TrackSpecialProperty.DistinctId);
            if (rawDistinctId == null)
            {
                return MessageBuildResult.CreateFail($"'{TrackSpecialProperty.DistinctId}' is not set.");
            }

            ValueParseResult distinctIdParseResult = DistinctIdParser.Parse(rawDistinctId.Value);
            if (!distinctIdParseResult.Success)
            {
                return MessageBuildResult.CreateFail(
                    $"Error parsing '{TrackSpecialProperty.DistinctId}'.", distinctIdParseResult.ErrorDetails);
            }

            properties["distinct_id"] = distinctIdParseResult.Value;
            
            // alias
            ValueParseResult aliasParseResult = DistinctIdParser.Parse(alias);
            if (!aliasParseResult.Success)
            {
                return MessageBuildResult.CreateFail("Error parsing 'alias'. " + aliasParseResult.ErrorDetails);
            }
            properties["alias"] = aliasParseResult.Value;


            return MessageBuildResult.CreateSuccess(message);
        }
    }
}