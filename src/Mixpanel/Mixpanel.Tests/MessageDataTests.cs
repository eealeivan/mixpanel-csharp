using System;
using System.Collections.Generic;
using Mixpanel.Core.Message;
using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MessageDataTests : MixpanelTestsBase
    {
        private MessageData _md;

        [SetUp]
        public void SetUp()
        {
            _md = new MessageData(new Dictionary<string, string>
            {
                {MixpanelProperty.Token, MixpanelProperty.Token},
                {MixpanelProperty.DistinctId, MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},
                {MixpanelProperty.Event, MixpanelProperty.Event}
            });
        }

        [Test]
        public void SetProperty_Works()
        {
            _md.ParseAndSetProperties(new
            {
                Event, 
                DistinctId,
                StringProperty = StringPropertyValue
            });
            _md.SetProperty(MixpanelProperty.Token, Token);
            _md.SetProperty(DecimalPropertyName, DecimalPropertyValue);

            Assert.That(_md.SpecialProps.Count, Is.EqualTo(3));
            Assert.That(_md.SpecialProps[MixpanelProperty.Event], Is.EqualTo(Event));
            Assert.That(_md.SpecialProps[MixpanelProperty.DistinctId], Is.EqualTo(DistinctId));
            Assert.That(_md.SpecialProps[MixpanelProperty.Token], Is.EqualTo(Token));

            Assert.That(_md.Props.Count, Is.EqualTo(2));
            Assert.That(_md.Props[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(_md.Props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        [Test]
        public void SetProperty_ManyTimes_Overwritten()
        {
            _md.ParseAndSetProperties(new
            {
                Event,
                StringProperty = StringPropertyValue
            });
            _md.SetProperty(MixpanelProperty.Event, Event + "2");
            _md.SetProperty(StringPropertyName, StringPropertyValue + "2");

            Assert.That(_md.SpecialProps.Count, Is.EqualTo(1));
            Assert.That(_md.SpecialProps[MixpanelProperty.Event], Is.EqualTo(Event + "2"));

            Assert.That(_md.Props.Count, Is.EqualTo(1));
            Assert.That(_md.Props[StringPropertyName], Is.EqualTo(StringPropertyValue + "2"));
        }

        [Test]
        public void GetSpecialProp_Works()
        {
            _md.SetProperty(MixpanelProperty.Event, Event);
            _md.SetProperty(MixpanelProperty.DistinctId, DistinctIdInt);

            Assert.That(_md.GetSpecialProp(MixpanelProperty.Event), Is.EqualTo(Event));
            Assert.That(_md.GetSpecialProp(MixpanelProperty.DistinctId), Is.EqualTo(DistinctIdInt));
            Assert.That(
                _md.GetSpecialProp(MixpanelProperty.DistinctId, x => x.ToString()),
                Is.EqualTo(DistinctId));
        }
        
        [Test]
        public void GetSpecialRequiredProp_Works()
        {
            _md.SetProperty(MixpanelProperty.Event, Event);
            _md.SetProperty(MixpanelProperty.DistinctId, DistinctIdInt);

            Assert.That(_md.GetSpecialRequiredProp(MixpanelProperty.Event), Is.EqualTo(Event));
            Assert.That(_md.GetSpecialRequiredProp(MixpanelProperty.DistinctId), Is.EqualTo(DistinctIdInt));
            Assert.That(
                _md.GetSpecialRequiredProp(MixpanelProperty.DistinctId, convertFn: x => x.ToString()),
                Is.EqualTo(DistinctId));
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
            _md.SetProperty(MixpanelProperty.Token, Token);
            Assert.That(
                () =>
                {
                    _md.GetSpecialRequiredProp(MixpanelProperty.Token,
                        x =>
                        {
                            if (!x.ToString().Equals(Token + "2"))
                                throw new Exception(
                                    string.Format("'token' should be equal to '{0}'.", Token + "2"));
                        });
                },
                Throws
                    .TypeOf<Exception>()
                    .And.Message.EqualTo(
                        string.Format("'token' should be equal to '{0}'.", Token + "2")));
        }
    }
}