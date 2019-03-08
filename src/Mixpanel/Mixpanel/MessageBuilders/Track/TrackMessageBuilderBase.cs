using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.Track
{
    internal static class TrackMessageBuilderBase
    {
        public static MessageCandidate CreateValidMessageCandidate(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config,
            out string errorMessage)
        {
            var messageCandidate = new MessageCandidate(
                token,
                superProperties,
                rawProperties,
                distinctId,
                config,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            ObjectProperty tokenProp = messageCandidate.GetSpecialProperty(TrackSpecialProperty.Token);
            if (tokenProp == null)
            {
                errorMessage = $"'{TrackSpecialProperty.Token}' is not set.";
                return null;
            }

            ValueParseResult tokenParseResult =
                TrackSpecialPropertyParser.Parse(TrackSpecialProperty.Token, tokenProp.Value);
            if (!tokenParseResult.Success)
            {
                errorMessage = $"Error parsing '{TrackSpecialProperty.Token}'. {tokenParseResult.ErrorDetails}";
                return null;
            }

            errorMessage = null;
            return messageCandidate;
        }
    }
}