using System;
using System.Collections.Generic;
using Mixpanel.Builders;
using Mixpanel.PropertyTypes;
using NUnit.Framework;

namespace Mixpanel.Tests.Builders
{
    [TestFixture]
    public class TrackBuilderTests
    {
        [Test]
        public void TrackBuilder_builds_simple_object()
        {
            var now = DateTime.Now;
            var builder = new TrackBuilder();

            builder.Add(MixpanelTrackProperty.Event, "test_event");
            builder.Add(MixpanelTrackProperty.Token, "aa11qq");
            builder.Add(MixpanelTrackProperty.IpAddress, "111.111.111.111");
            builder.Add(MixpanelTrackProperty.Time, now);
            builder.Add("TestProp1", "test value 1");

            var obj = builder.Object;
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual("test_event", obj[MixpanelTrackProperty.Event]);

            Assert.IsInstanceOf<IDictionary<string, object>>(obj["properties"]);
            var properties = (IDictionary<string, object>) obj["properties"];

            Assert.AreEqual("aa11qq", properties[MixpanelTrackProperty.Token]);
            Assert.AreEqual("111.111.111.111", properties[MixpanelTrackProperty.IpAddress]);
            Assert.AreEqual(now, properties[MixpanelTrackProperty.Time]);
            Assert.AreEqual("test value 1", properties["TestProp1"]);
            
        }
    }
}
