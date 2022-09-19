using System;

namespace Mixpanel.Tests.Unit.MixpanelClient.People
{
    [Flags]
    public enum PeopleSuperPropsDetails
    {
        DistinctId = 0x0,
        MessageSpecialProperties = 0x1,
        UserProperties = 0x2,
        All = DistinctId | MessageSpecialProperties | UserProperties
    }
}