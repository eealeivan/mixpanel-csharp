using System;
using System.Collections.Generic;
using Mixpanel.MessageProperties;
using static System.String;

namespace Mixpanel.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$delete": "",
    //     "$ignore_alias": true 
    // }

    internal static class PeopleDeleteMessageBuilder 
    {
        public static MessageBuildResult Build(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            object distinctId,
            bool ignoreAlias,
            MixpanelConfig config)
        {
            MessageBuildResult messageBuildResult = PeopleMessageBuilderBase.CreateMessage(
                token,
                superProperties,
                null,
                distinctId,
                config,
                "$delete",
                rawValue => throw new InvalidOperationException());

            if (!messageBuildResult.Success)
            {
                return messageBuildResult;
            }

            messageBuildResult.Message["$delete"] = Empty;

            if (ignoreAlias)
            {
                messageBuildResult.Message[PeopleSpecialProperty.IgnoreAlias] = true;
            }
            
            return messageBuildResult;
        }
    }
}