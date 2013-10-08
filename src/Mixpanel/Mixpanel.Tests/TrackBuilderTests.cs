using System;
using System.Collections.Generic;
using Mixpanel.Builders;
using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class TrackBuilderTests
    {
        private readonly Dictionary<string, string> _specialPropsBindings =
           new Dictionary<string, string>
            {
                {"event", MixpanelProperty.Event},

                {"token", MixpanelProperty.Token},

                {"distinct_id", MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},

                {"ip", MixpanelProperty.Ip},

                {"time", MixpanelProperty.Time},
            };

        [Test]
        public void TrackBuilder_builds_simple_object()
        {
            var now = new DateTime(2013, 9, 26, 22, 33, 44, DateTimeKind.Utc);
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);

            md.SetProperty(MixpanelProperty.Event, "test_event");
            md.SetProperty(MixpanelProperty.Token, "aa11qq");
            md.SetProperty(MixpanelProperty.DistinctId, "123");
            md.SetProperty(MixpanelProperty.Ip, "111.111.111.111");
            md.SetProperty(MixpanelProperty.Time, now);
            md.SetProperty("TestProp1", "test value 1");
            md.SetProperty("TestProp2", 5);

            var obj = builder.GetObject(md);
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual("test_event", obj["event"]);

            Assert.IsInstanceOf<IDictionary<string, object>>(obj["properties"]);
            var properties = (IDictionary<string, object>)obj["properties"];

            Assert.AreEqual("aa11qq", properties["token"]);
            Assert.AreEqual("123", properties["distinct_id"]);
            Assert.AreEqual("111.111.111.111", properties["ip"]);
            Assert.IsInstanceOf<long>(properties["time"]);
            Assert.AreEqual(1380234824L, properties["time"]);
            Assert.AreEqual("test value 1", properties["TestProp1"]);
            Assert.AreEqual(5, properties["TestProp2"]);
        }

        [Test]
        public void TrackBuilder_overrides_properties()
        {
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Event, "test_event1");
            md.SetProperty(MixpanelProperty.Event, "test_event2");

            md.SetProperty(MixpanelProperty.Token, "token2");
            md.SetProperty(MixpanelProperty.Token, "token1");

            md.SetProperty("TestProp1", "1");
            md.SetProperty("TestProp1", "2");
            md.SetProperty("TestProp1", "3");

            md.SetProperty("TestProp2", "3");
            md.SetProperty("TestProp2", "2");
            md.SetProperty("TestProp2", "1");

            var obj = builder.GetObject(md);

            Assert.AreEqual("test_event2", obj["event"]);
            var properties = (IDictionary<string, object>)obj["properties"];
            Assert.AreEqual("token1", properties["token"]);

            Assert.AreEqual("3", properties["TestProp1"]);
            Assert.AreEqual("1", properties["TestProp2"]);
        }

        [Test]
        [ExpectedException(typeof(MixpanelObjectStructureException),
            ExpectedMessage = "'event'", MatchType = MessageMatch.Contains)]
        public void TrackBuilder_throws_exception_if_event_is_not_set()
        {
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);
            builder.GetObject(md);
        }

        [Test]
        [ExpectedException(typeof(MixpanelRequiredPropertyNullOrEmptyException),
            ExpectedMessage = "'event'", MatchType = MessageMatch.Contains)]
        public void TrackBuilder_throws_exception_if_event_is_null()
        {
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Event, null);
            builder.GetObject(md);
        }

        [Test]
        [ExpectedException(typeof(MixpanelRequiredPropertyNullOrEmptyException),
            ExpectedMessage = "'event' property can't be empty.")]
        public void TrackBuilder_throws_exception_if_event_is_empty()
        {
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Event, "");
            builder.GetObject(md);
        }

        [Test]
        [ExpectedException(typeof(MixpanelObjectStructureException),
            ExpectedMessage = "'token'", MatchType = MessageMatch.Contains)]
        public void TrackBuilder_throws_exception_if_token_is_not_set()
        {
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Event, "event");
            builder.GetObject(md);
        }

        [Test]
        [ExpectedException(typeof(MixpanelRequiredPropertyNullOrEmptyException),
            ExpectedMessage = "'token'", MatchType = MessageMatch.Contains)]
        public void TrackBuilder_throws_exception_if_token_is_null()
        {
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Event, "event");
            md.SetProperty(MixpanelProperty.Token, null);
            builder.GetObject(md);
        }

        [Test]
        [ExpectedException(typeof(MixpanelRequiredPropertyNullOrEmptyException),
            ExpectedMessage = "'token' property can't be empty.")]
        public void TrackBuilder_throws_exception_if_token_is_empty()
        {
            var builder = new TrackBuilder();
            var md = new MixpanelData(_specialPropsBindings);
            md.SetProperty(MixpanelProperty.Event, "event");
            md.SetProperty(MixpanelProperty.Token, "");
            builder.GetObject(md);
        }
    }
}
