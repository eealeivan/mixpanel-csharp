using System;
using System.Collections.Generic;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;

namespace Mixpanel.MessageBuilders.People
{
    // Message example:
    // {
    //     "$append": {
    //         "$transactions": {
    //             "$time": "2013-01-03T09:00:00",
    //             "$amount": 25.34
    //         }
    //     },
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793"
    // }

    internal static class PeopleTrackChargeMessageBuilder
    {
        public static MessageBuildResult Build(
            string token,
            IEnumerable<ObjectProperty> superProperties,
            decimal amount,
            DateTime time,
            object distinctId,
            MixpanelConfig config)
        {
            MessageBuildResult messageBuildResult = PeopleMessageBuilderBase.CreateMessage(
                token,
                superProperties,
                null,
                distinctId,
                config,
                "$append",
                rawValue => throw new InvalidOperationException());

            if (!messageBuildResult.Success)
            {
                return messageBuildResult;
            }

            messageBuildResult.Message["$append"] = new Dictionary<string, object>(1)
            {
                {
                    "$transactions", new Dictionary<string, object>(2)
                    {
                        {"$time", TimeParser.ParseMixpanelFormat(time).Value},
                        {"$amount", amount}
                    }
                }
            };

            return messageBuildResult;
        }
    }
}