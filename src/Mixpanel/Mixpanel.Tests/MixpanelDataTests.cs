using System;
using Mixpanel.Builders;
using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelDataTests
    {
        private MixpanelData _md;

        [SetUp]
        public void SetUp()
        {
            _md = new MixpanelData(TrackBuilder.SpecialPropsBindings);
        }

        [Test]
        public void SetProperty_Works()
        {
            _md.ParseAndSetProperties(new
            {
                Event = "test-event",
                DistinctId = "12345",
                Prop1 = "val1"
            });
            _md.SetProperty(MixpanelProperty.Token, "token1");
            _md.SetProperty("Prop2", "val2");

            Assert.That(_md.SpecialProps.Count, Is.EqualTo(3));
            Assert.That(_md.SpecialProps[MixpanelProperty.Event], Is.EqualTo("test-event"));
            Assert.That(_md.SpecialProps[MixpanelProperty.DistinctId], Is.EqualTo("12345"));
            Assert.That(_md.SpecialProps[MixpanelProperty.Token], Is.EqualTo("token1"));

            Assert.That(_md.Props.Count, Is.EqualTo(2));
            Assert.That(_md.Props["Prop1"], Is.EqualTo("val1"));
            Assert.That(_md.Props["Prop2"], Is.EqualTo("val2"));
        }

        [Test]
        public void SetProperty_ManyTimes_Overwritten()
        {
            _md.ParseAndSetProperties(new
            {
                Event = "test-event1",
                Prop1 = "val1"
            });
            _md.SetProperty(MixpanelProperty.Event, "test-event2");
            _md.SetProperty("Prop1", "val2");

            Assert.That(_md.SpecialProps.Count, Is.EqualTo(1));
            Assert.That(_md.SpecialProps[MixpanelProperty.Event], Is.EqualTo("test-event2"));

            Assert.That(_md.Props.Count, Is.EqualTo(1));
            Assert.That(_md.Props["Prop1"], Is.EqualTo("val2"));
        }

        [Test]
        public void GetSpecialProp_Works()
        {
            _md.SetProperty(MixpanelProperty.Event, "test-event");
            _md.SetProperty(MixpanelProperty.DistinctId, 12345);

            Assert.That(_md.GetSpecialProp(MixpanelProperty.Event), Is.EqualTo("test-event"));
            Assert.That(_md.GetSpecialProp(MixpanelProperty.DistinctId), Is.EqualTo(12345));
            Assert.That(
                _md.GetSpecialProp(MixpanelProperty.DistinctId, x => x.ToString()),
                Is.EqualTo("12345"));
        }
        
        [Test]
        public void GetSpecialRequiredProp_Works()
        {
            _md.SetProperty(MixpanelProperty.Event, "test-event");
            _md.SetProperty(MixpanelProperty.DistinctId, 12345);

            Assert.That(_md.GetSpecialRequiredProp(MixpanelProperty.Event), Is.EqualTo("test-event"));
            Assert.That(_md.GetSpecialRequiredProp(MixpanelProperty.DistinctId), Is.EqualTo(12345));
            Assert.That(
                _md.GetSpecialRequiredProp(MixpanelProperty.DistinctId, convertFn: x => x.ToString()),
                Is.EqualTo("12345"));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyNotSet_ThrowsException()
        {
            Assert.That(
             () => { _md.GetSpecialRequiredProp(MixpanelProperty.Token); },
             Throws
                 .TypeOf<MixpanelObjectStructureException>()
                 .And.Message.EqualTo("'token' property is not set."));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyIsNull_ThrowsException()
        {
            _md.SetProperty(MixpanelProperty.Token, null);
            Assert.That(
            () => { _md.GetSpecialRequiredProp(MixpanelProperty.Token); },
            Throws
                .TypeOf<MixpanelRequiredPropertyNullOrEmptyException>()
                .And.Message.EqualTo("'token' property can't be null."));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyValidationFails_ThrowsException()
        {
            _md.SetProperty(MixpanelProperty.Token, "123");
            Assert.That(
                () =>
                {
                    _md.GetSpecialRequiredProp(MixpanelProperty.Token,
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