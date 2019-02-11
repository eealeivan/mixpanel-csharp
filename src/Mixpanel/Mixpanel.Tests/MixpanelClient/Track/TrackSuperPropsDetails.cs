using System;

namespace Mixpanel.Tests.MixpanelClient.Track
{
    [Flags]
    public enum TrackSuperPropsDetails
    {
        DistinctId = 0x0,
        SpecialProperties = 0x1,
        UserProperties = 0x2,
        All = DistinctId | SpecialProperties | UserProperties
    }
}