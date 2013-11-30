using System;
using Mixpanel.Core;
using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class ObjectDataTests
    {
        private ObjectData _od;

        [SetUp]
        public void SetUp()
        {
            _od = new ObjectData(TrackBuilder.SpecialPropsBindings);
        }

        [Test]
        public void SetProperty_Works()
        {
            _od.ParseAndSetProperties(new
            {
                Event = "test-event",
                DistinctId = "12345",
                Prop1 = "val1"
            });
            _od.SetProperty(MixpanelProperty.Token, "token1");
            _od.SetProperty("Prop2", "val2");

            Assert.That(_od.SpecialProps.Count, Is.EqualTo(3));
            Assert.That(_od.SpecialProps[MixpanelProperty.Event], Is.EqualTo("test-event"));
            Assert.That(_od.SpecialProps[MixpanelProperty.DistinctId], Is.EqualTo("12345"));
            Assert.That(_od.SpecialProps[MixpanelProperty.Token], Is.EqualTo("token1"));

            Assert.That(_od.Props.Count, Is.EqualTo(2));
            Assert.That(_od.Props["Prop1"], Is.EqualTo("val1"));
            Assert.That(_od.Props["Prop2"], Is.EqualTo("val2"));
        }

        [Test]
        public void SetProperty_ManyTimes_Overwritten()
        {
            _od.ParseAndSetProperties(new
            {
                Event = "test-event1",
                Prop1 = "val1"
            });
            _od.SetProperty(MixpanelProperty.Event, "test-event2");
            _od.SetProperty("Prop1", "val2");

            Assert.That(_od.SpecialProps.Count, Is.EqualTo(1));
            Assert.That(_od.SpecialProps[MixpanelProperty.Event], Is.EqualTo("test-event2"));

            Assert.That(_od.Props.Count, Is.EqualTo(1));
            Assert.That(_od.Props["Prop1"], Is.EqualTo("val2"));
        }

        [Test]
        public void GetSpecialProp_Works()
        {
            _od.SetProperty(MixpanelProperty.Event, "test-event");
            _od.SetProperty(MixpanelProperty.DistinctId, 12345);

            Assert.That(_od.GetSpecialProp(MixpanelProperty.Event), Is.EqualTo("test-event"));
            Assert.That(_od.GetSpecialProp(MixpanelProperty.DistinctId), Is.EqualTo(12345));
            Assert.That(
                _od.GetSpecialProp(MixpanelProperty.DistinctId, x => x.ToString()),
                Is.EqualTo("12345"));
        }
        
        [Test]
        public void GetSpecialRequiredProp_Works()
        {
            _od.SetProperty(MixpanelProperty.Event, "test-event");
            _od.SetProperty(MixpanelProperty.DistinctId, 12345);

            Assert.That(_od.GetSpecialRequiredProp(MixpanelProperty.Event), Is.EqualTo("test-event"));
            Assert.That(_od.GetSpecialRequiredProp(MixpanelProperty.DistinctId), Is.EqualTo(12345));
            Assert.That(
                _od.GetSpecialRequiredProp(MixpanelProperty.DistinctId, convertFn: x => x.ToString()),
                Is.EqualTo("12345"));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyNotSet_ThrowsException()
        {
            Assert.That(
             () => { _od.GetSpecialRequiredProp(MixpanelProperty.Token); },
             Throws
                 .TypeOf<MixpanelObjectStructureException>()
                 .And.Message.EqualTo("'token' property is not set."));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyIsNull_ThrowsException()
        {
            _od.SetProperty(MixpanelProperty.Token, null);
            Assert.That(
            () => { _od.GetSpecialRequiredProp(MixpanelProperty.Token); },
            Throws
                .TypeOf<MixpanelRequiredPropertyNullOrEmptyException>()
                .And.Message.EqualTo("'token' property can't be null."));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyValidationFails_ThrowsException()
        {
            _od.SetProperty(MixpanelProperty.Token, "123");
            Assert.That(
                () =>
                {
                    _od.GetSpecialRequiredProp(MixpanelProperty.Token,
                        x =>
                        {
                            if (!x.ToString().Equals("321"))
                                throw new Exception("'token' should be equal to '321'.");
                        });
                },
                Throws
                    .TypeOf<Exception>()
                    .And.Message.EqualTo("'token' should be equal to '321'."));
        }
    }
}