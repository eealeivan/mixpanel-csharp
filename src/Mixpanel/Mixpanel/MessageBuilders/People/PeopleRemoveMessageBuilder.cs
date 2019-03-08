using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$remove": { "Items purchased": "socks" }
    // }

    internal static class PeopleRemoveMessageBuilder
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
                "$remove",
                rawValue => GenericPropertyParser.Parse(rawValue, allowCollections: false));
        }
    }
}