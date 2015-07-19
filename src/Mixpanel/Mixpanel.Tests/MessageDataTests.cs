using System;
using System.Collections.Generic;
using Mixpanel.Core;
using Mixpanel.Core.Message;
using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MessageDataTests : MixpanelTestsBase
    {
        private static readonly IDictionary<string, string> SpecialPropsBindings =
            new Dictionary<string, string>
            {
                {MixpanelProperty.Token, MixpanelProperty.Token},
                {MixpanelProperty.DistinctId, MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId},
                {MixpanelProperty.Event, MixpanelProperty.Event}
            };
        
        private static readonly IDictionary<string, string> DistinctIdPropsBindings =
            new Dictionary<string, string>
            {
                {MixpanelProperty.DistinctId, MixpanelProperty.DistinctId},
                {"distinctid", MixpanelProperty.DistinctId}
            };

        private MessageData GetMessageDataObject(
            MessagePropetiesRules messagePropetiesRules = MessagePropetiesRules.None, 
            SuperPropertiesRules superPropertiesRules = SuperPropertiesRules.All)
        {
            return new MessageData(
                SpecialPropsBindings, 
                DistinctIdPropsBindings, 
                messagePropetiesRules,
                superPropertiesRules);
        }

        [Test]
        public void SetProperty_Works()
        {
            var md = GetMessageDataObject();
            md.ParseAndSetProperties(new
            {
                Event, 
                DistinctId,
                StringProperty = StringPropertyValue
            });
            md.SetProperty(MixpanelProperty.Token, Token);
            md.SetProperty(DecimalPropertyName, DecimalPropertyValue);

            Assert.That(md.SpecialProps.Count, Is.EqualTo(3));
            Assert.That(md.SpecialProps[MixpanelProperty.Event], Is.EqualTo(Event));
            Assert.That(md.SpecialProps[MixpanelProperty.DistinctId], Is.EqualTo(DistinctId));
            Assert.That(md.SpecialProps[MixpanelProperty.Token], Is.EqualTo(Token));

            Assert.That(md.Props.Count, Is.EqualTo(2));
            Assert.That(md.Props[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(md.Props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        [Test]
        public void SetProperty_ManyTimes_Overwritten()
        {
            var md = GetMessageDataObject();
            md.ParseAndSetProperties(new
            {
                Event,
                StringProperty = StringPropertyValue
            });
            md.SetProperty(MixpanelProperty.Event, Event + "2");
            md.SetProperty(StringPropertyName, StringPropertyValue + "2");

            Assert.That(md.SpecialProps.Count, Is.EqualTo(1));
            Assert.That(md.SpecialProps[MixpanelProperty.Event], Is.EqualTo(Event + "2"));

            Assert.That(md.Props.Count, Is.EqualTo(1));
            Assert.That(md.Props[StringPropertyName], Is.EqualTo(StringPropertyValue + "2"));
        }

        [Test]
        public void GetSpecialProp_Works()
        {
            var md = GetMessageDataObject();
            md.SetProperty(MixpanelProperty.Event, Event);
            md.SetProperty(MixpanelProperty.DistinctId, DistinctIdInt);

            Assert.That(md.GetSpecialProp(MixpanelProperty.Event), Is.EqualTo(Event));
            Assert.That(md.GetSpecialProp(MixpanelProperty.DistinctId), Is.EqualTo(DistinctIdInt));
            Assert.That(
                md.GetSpecialProp(MixpanelProperty.DistinctId, x => x.ToString()),
                Is.EqualTo(DistinctId));
        }
        
        [Test]
        public void GetSpecialRequiredProp_Works()
        {
            var md = GetMessageDataObject();
            md.SetProperty(MixpanelProperty.Event, Event);
            md.SetProperty(MixpanelProperty.DistinctId, DistinctIdInt);

            Assert.That(md.GetSpecialRequiredProp(MixpanelProperty.Event), Is.EqualTo(Event));
            Assert.That(md.GetSpecialRequiredProp(MixpanelProperty.DistinctId), Is.EqualTo(DistinctIdInt));
            Assert.That(
                md.GetSpecialRequiredProp(MixpanelProperty.DistinctId, convertFn: x => x.ToString()),
                Is.EqualTo(DistinctId));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyNotSet_ThrowsException()
        {
            var md = GetMessageDataObject();
            Assert.That(
             () => { md.GetSpecialRequiredProp(MixpanelProperty.Token); },
             Throws
                 .TypeOf<MixpanelObjectStructureException>()
                 .And.Message.EqualTo("'token' property is not set."));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyIsNull_ThrowsException()
        {
            var md = GetMessageDataObject();
            md.SetProperty(MixpanelProperty.Token, null);
            Assert.That(
            () => { md.GetSpecialRequiredProp(MixpanelProperty.Token); },
            Throws
                .TypeOf<MixpanelRequiredPropertyNullOrEmptyException>()
                .And.Message.EqualTo("'token' property can't be null."));
        }

        [Test]
        public void GetSpecialRequiredProp_RequiredPropertyValidationFails_ThrowsException()
        {
            var md = GetMessageDataObject();
            md.SetProperty(MixpanelProperty.Token, Token);
            Assert.That(
                () =>
                {
                    md.GetSpecialRequiredProp(MixpanelProperty.Token,
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

        [Test]
        public void RemoveProperty_Works()
        {
            var md = GetMessageDataObject();
            md.SetProperty(MixpanelProperty.Event, Event, PropertyNameSource.MixpanelName);
            md.SetProperty(MixpanelProperty.DistinctId, DistinctId);
            md.SetProperty(DecimalPropertyName, DecimalPropertyValue, PropertyNameSource.DataMember);
            md.SetProperty(DoublePropertyName, DoublePropertyValue);

            Assert.That(md.SpecialProps.Count, Is.EqualTo(2));
            Assert.That(md.Props.Count, Is.EqualTo(2));

            md.RemoveProperty(MixpanelProperty.Event);
            md.RemoveProperty(MixpanelProperty.DistinctId);
            md.RemoveProperty(DecimalPropertyName);
            md.RemoveProperty(DoublePropertyName);

            Assert.That(md.SpecialProps.Count, Is.EqualTo(0));
            Assert.That(md.Props.Count, Is.EqualTo(0));
        }

        [Test]
        public void ClearAllProperties_Works()
        {
            var md = GetMessageDataObject();
            md.SetProperty(MixpanelProperty.Event, Event, PropertyNameSource.MixpanelName);
            md.SetProperty(MixpanelProperty.DistinctId, DistinctId);
            md.SetProperty(DecimalPropertyName, DecimalPropertyValue, PropertyNameSource.DataMember);
            md.SetProperty(DoublePropertyName, DoublePropertyValue);

            Assert.That(md.SpecialProps.Count, Is.EqualTo(2));
            Assert.That(md.Props.Count, Is.EqualTo(2));

            md.ClearAllProperties();

            Assert.That(md.SpecialProps.Count, Is.EqualTo(0));
            Assert.That(md.Props.Count, Is.EqualTo(0));
        }
    }
}