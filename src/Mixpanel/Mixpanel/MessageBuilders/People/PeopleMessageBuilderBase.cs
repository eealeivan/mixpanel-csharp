using System;
using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.People
{
    internal static class PeopleMessageBuilderBase
    {
        public static MessageBuildResult CreateMessage(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config,
            string actionName,
            Func<object, ValueParseResult> userPropertyParseFn)
        {
            MessageCandidate messageCandidate = CreateValidMessageCandidate(
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
            var action = new Dictionary<string, object>();
            message[actionName] = action;

            // Special properties
            RunForValidSpecialProperties(
                messageCandidate,
                (specialPropertyName, isMessageSpecialProperty, value) =>
                {
                    // Ignore non-message specific special properties as they are not valid in profile update messages
                    if (isMessageSpecialProperty)
                    {
                        message[specialPropertyName] = value;
                    }
                });

            // User properties
            RunForValidUserProperties(
                messageCandidate,
                userPropertyParseFn,
                (formattedPropertyName, value) =>
                {
                    action[formattedPropertyName] = value;
                });

            return MessageBuildResult.CreateSuccess(message);
        }

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
                PeopleSpecialPropertyMapper.RawNameToSpecialProperty);

            ObjectProperty tokenProp = messageCandidate.GetSpecialProperty(PeopleSpecialProperty.Token);
            if (tokenProp == null)
            {
                errorMessage = $"'{PeopleSpecialProperty.Token}' is not set.";
                return null;
            }

            ValueParseResult tokenParseResult = 
                PeopleSpecialPropertyParser.Parse(PeopleSpecialProperty.Token, tokenProp.Value);
            if (!tokenParseResult.Success)
            {
                errorMessage = $"Error parsing '{PeopleSpecialProperty.Token}'. {tokenParseResult.ErrorDetails}";
                return null;
            }

            ObjectProperty distinctIdProp = messageCandidate.GetSpecialProperty(PeopleSpecialProperty.DistinctId);
            if (distinctIdProp == null)
            {
                errorMessage = $"'{PeopleSpecialProperty.DistinctId}' is not set.";
                return null;
            }

            ValueParseResult distinctIdParseResult = 
                PeopleSpecialPropertyParser.Parse(PeopleSpecialProperty.DistinctId, distinctIdProp.Value);
            if (!distinctIdParseResult.Success)
            {
                errorMessage = $"Error parsing '{PeopleSpecialProperty.DistinctId}'. {distinctIdParseResult.ErrorDetails}";
                return null;
            }

            errorMessage = null;
            return messageCandidate;
        }

        public static void RunForValidSpecialProperties(
            MessageCandidate messageCandidate,
            Action<string, bool, object> fn)
        {
            foreach (KeyValuePair<string, ObjectProperty> pair in messageCandidate.SpecialProperties)
            {
                string specialPropertyName = pair.Key;
                ObjectProperty objectProperty = pair.Value;

                ValueParseResult result = PeopleSpecialPropertyParser.Parse(specialPropertyName, objectProperty.Value);
                if (!result.Success)
                {
                    continue;
                }

                bool isMessageSpecialProperty = PeopleSpecialProperty.IsMessageSpecialProperty(specialPropertyName);

                fn(specialPropertyName, isMessageSpecialProperty, result.Value);
            }
        }

        public static void RunForValidUserProperties(
            MessageCandidate messageCandidate,
            Func<object, ValueParseResult> parseFn,
            Action<string, object> fn)
        {
            foreach (KeyValuePair<string, ObjectProperty> pair in messageCandidate.UserProperties)
            {
                string formattedPropertyName = pair.Key;
                ObjectProperty objectProperty = pair.Value;

                if (objectProperty.Origin == PropertyOrigin.SuperProperty)
                {
                    // Skip all non special super properties as they are not valid for people message
                    continue;
                }

                ValueParseResult result = parseFn(objectProperty.Value);
                if (!result.Success)
                {
                    continue;
                }

                fn(formattedPropertyName, result.Value);
            }
        }
    }
}