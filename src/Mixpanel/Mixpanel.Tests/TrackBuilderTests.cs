using System;
using System.Collections.Generic;
using Mixpanel.Builders;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class TrackBuilderTests
    {
        [Test]
        public void TrackBuilder_builds_simple_object()
        {
            var now = new DateTime(2013, 9, 26, 22, 33, 44, DateTimeKind.Utc);
            var builder = new TrackBuilder();

            builder.Add(MixpanelProperty.Event, "test_event");
            builder.Add(MixpanelProperty.Token, "aa11qq");
            builder.Add(MixpanelProperty.DistinctId, "123");
            builder.Add(MixpanelProperty.Ip, "111.111.111.111");
            builder.Add(MixpanelProperty.Time, now);
            builder.Add("TestProp1", "test value 1");
            builder.Add("TestProp2", 5);

            var obj = builder.Object;
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual("test_event", obj["event"]);

            Assert.IsInstanceOf<IDictionary<string, object>>(obj["properties"]);
            var properties = (IDictionary<string, object>) obj["properties"];

            Assert.AreEqual("aa11qq", properties["token"]);
            Assert.AreEqual("123", properties["distinct_id"]);
            Assert.AreEqual("111.111.111.111", properties["ip"]);
            Assert.IsInstanceOf<long>(properties["time"]);
            Assert.AreEqual(1380234824L, properties["time"]);
            Assert.AreEqual("test value 1", properties["TestProp1"]);
            Assert.AreEqual(5, properties["TestProp2"]);
        }

        [Test]
        public void TrackBuilder_respects_property_weight()
        {
            var builder = new TrackBuilder();
            builder.Add(MixpanelProperty.Event, "test_event1", 1);
            builder.Add(MixpanelProperty.Event, "test_event2", 2);

            builder.Add(MixpanelProperty.Token, "token2", 2);
            builder.Add(MixpanelProperty.Token, "token1", 1);

            builder.Add("TestProp1", "1", 1);
            builder.Add("TestProp1", "2", 2);
            builder.Add("TestProp1", "3", 3);
            
            builder.Add("TestProp2", "3", 3);
            builder.Add("TestProp2", "2", 2);
            builder.Add("TestProp2", "1", 1);

            var obj = builder.Object;

            Assert.AreEqual("test_event2", obj["event"]);
            var properties = (IDictionary<string, object>)obj["properties"];
            Assert.AreEqual("token2", properties["token"]);

            // Weights are ignored for custum properties
            Assert.AreEqual("3", properties["TestProp1"]);
            Assert.AreEqual("1", properties["TestProp2"]);
        }
    }
}
