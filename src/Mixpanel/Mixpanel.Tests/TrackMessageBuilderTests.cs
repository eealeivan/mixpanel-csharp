using System;
using System.Collections.Generic;
using Mixpanel.Core;
using Mixpanel.Core.Message;
using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class TrackMessageBuilderTests
    {
        private TrackMessageBuilder _builder;
        private MessageData _md;

        [SetUp]
        public void SetUp()
        {
            MixpanelConfig.Global.Reset();
            _builder = new TrackMessageBuilder();
            _md = new MessageData(
                _builder.SpecialPropsBindings,
                _builder.DistinctIdPropsBindings,
                _builder.MessagePropetiesRules,
                _builder.SuperPropertiesRules);
        }

        [Test]
        public void GetObject_BuildSimpleObject_Builed()
        {
            var now = new DateTime(2013, 9, 26, 22, 33, 44, DateTimeKind.Utc);
            _md.SetProperty(MixpanelProperty.Event, "test_event");
            _md.SetProperty(MixpanelProperty.Token, "aa11qq");
            _md.SetProperty(MixpanelProperty.DistinctId, "123");
            _md.SetProperty(MixpanelProperty.Ip, "111.111.111.111");
            _md.SetProperty(MixpanelProperty.Time, now);
            _md.SetProperty("TestProp1", "test value 1");
            _md.SetProperty("TestProp2", 5);

            var obj = _builder.GetMessageObject(_md);
            Assert.That(obj.Count, Is.EqualTo(2));
            Assert.That(obj["event"], Is.EqualTo("test_event"));

            Assert.That(obj["properties"], Is.InstanceOf<IDictionary<string, object>>());
            var properties = (IDictionary<string, object>)obj["properties"];

            Assert.That(properties.Count, Is.EqualTo(6));
            Assert.That(properties["token"], Is.EqualTo("aa11qq"));
            Assert.That(properties["distinct_id"], Is.EqualTo("123"));
            Assert.That(properties["ip"], Is.EqualTo("111.111.111.111"));
            Assert.That(properties["time"], Is.TypeOf<long>());
            Assert.That(properties["time"], Is.EqualTo(1380234824L));
            Assert.That(properties["TestProp1"], Is.EqualTo("test value 1"));
            Assert.That(properties["TestProp2"], Is.EqualTo(5));
        }

        [Test]
        public void GetObject_SetOnePropertyManyTimes_Overwritten()
        {
            _md.SetProperty(MixpanelProperty.Event, "test_event1");
            _md.SetProperty(MixpanelProperty.Event, "test_event2");

            _md.SetProperty(MixpanelProperty.Token, "token2");
            _md.SetProperty(MixpanelProperty.Token, "token1");

            _md.SetProperty("TestProp1", "1");
            _md.SetProperty("TestProp1", "2");
            _md.SetProperty("TestProp1", "3");

            _md.SetProperty("TestProp2", "3");
            _md.SetProperty("TestProp2", "2");
            _md.SetProperty("TestProp2", "1");

            var obj = _builder.GetMessageObject(_md);

            Assert.That(obj["event"], Is.EqualTo("test_event2"));

            var properties = (IDictionary<string, object>)obj["properties"];

            Assert.That(properties.Count, Is.EqualTo(3));
            Assert.That(properties["token"], Is.EqualTo("token1"));
            Assert.That(properties["TestProp1"], Is.EqualTo("3"));
            Assert.That(properties["TestProp2"], Is.EqualTo("1"));
        }

        [Test]
        public void GetObject_EventPropertyNotSet_ThrowsException()
        {
            Assert.That(
               () => { _builder.GetMessageObject(_md); },
               Throws
                   .TypeOf<MixpanelObjectStructureException>()
                   .And.Message.EqualTo("'event' property is not set."));
        }

        [Test]
        public void GetObject_EventPropertyIsNull_ThrowsException()
        {
            _md.SetProperty(MixpanelProperty.Event, null);
            Assert.That(
               () => { _builder.GetMessageObject(_md); },
               Throws
                   .TypeOf<MixpanelRequiredPropertyNullOrEmptyException>()
                   .And.Message.EqualTo("'event' property can't be null."));
        }

        [Test]
        public void GetObject_EventPropertyIsEmpty_ThrowsException()
        {
            _md.SetProperty(MixpanelProperty.Event, "");
            Assert.That(
               () => { _builder.GetMessageObject(_md); },
               Throws
                   .TypeOf<MixpanelRequiredPropertyNullOrEmptyException>()
                   .And.Message.EqualTo("'event' property can't be empty."));
        }

        [Test]
        public void GetObject_TokenPropertyNotSet_ThrowsException()
        {
            _md.SetProperty(MixpanelProperty.Event, "event");
            Assert.That(
                () => { _builder.GetMessageObject(_md); },
                Throws
                    .TypeOf<MixpanelObjectStructureException>()
                    .And.Message.EqualTo("'token' property is not set."));
        }

        [Test]
        public void GetObject_TokenPropertyIsNull_ThrowsException()
        {
            _md.SetProperty(MixpanelProperty.Event, "event");
            _md.SetProperty(MixpanelProperty.Token, null);

            Assert.That(
                () => { _builder.GetMessageObject(_md); },
                Throws
                    .TypeOf<MixpanelRequiredPropertyNullOrEmptyException>()
                    .And.Message.EqualTo("'token' property can't be null."));
        }

        [Test]
        public void GetObject_TokenPropertyIsEmpty_ThrowsException()
        {
            _md.SetProperty(MixpanelProperty.Event, "event");
            _md.SetProperty(MixpanelProperty.Token, "");

            Assert.That(
                () => { _builder.GetMessageObject(_md); },
                Throws
                    .TypeOf<MixpanelRequiredPropertyNullOrEmptyException>()
                    .And.Message.EqualTo("'token' property can't be empty."));
        }
    }
}