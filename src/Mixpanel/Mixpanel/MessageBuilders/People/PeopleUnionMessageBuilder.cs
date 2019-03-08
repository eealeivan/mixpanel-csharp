using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$union": { "Items purchased": ["socks", "shirts"] }
    // }

    internal static class PeopleUnionMessageBuilder
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
                "$union",
                rawValue => CollectionParser.Parse(rawValue, _ => GenericPropertyParser.Parse(_, allowCollections: false)));
        }
    }
}