using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$append": { "Power Ups": "Bubble Lead" }
    // }

    internal static class PeopleAppendMessageBuilder
    {
        public static MessageBuildResult Build(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object rawProperties,
            object distinctId,
            MixpanelConfig config)
        {
            return PeopleMessageBuilderBase.CreateMessage(
                token,
                superProperties,
                rawProperties,
                distinctId,
                config,
                "$append",
                rawValue => GenericPropertyParser.Parse(rawValue, allowCollections: false));
        }
    }
}