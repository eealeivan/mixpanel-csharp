using System;
using NUnit.Framework;

namespace Mixpanel.Tests.Unit.MixpanelClient.Track
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TrackSuperPropsAttribute : PropertyAttribute
    {
        public const string Name = "TrackSuperProps";

        public TrackSuperPropsAttribute(TrackSuperPropsDetails details)
            : base(details)
        {
        }
    }
}