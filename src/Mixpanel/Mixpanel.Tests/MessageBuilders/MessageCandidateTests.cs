using System.Collections.Generic;
using System.Linq;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageProperties;
using NUnit.Framework;

namespace Mixpanel.Tests.MessageBuilders
{
    [TestFixture]
    public class MessageCandidateTests : MixpanelTestsBase
    {
        [Test]
        public void When_AllInputsAreNull_Then_NoProperties()
        {
            var messageCandidate = new MessageCandidate(
                null, 
                null, 
                null,
                null, 
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            Assert.That(messageCandidate.SpecialProperties.Count, Is.EqualTo(0));
            Assert.That(messageCandidate.UserProperties.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_SuperProperties_Then_AllPropertiesSet()
        {
            var messageCandidate = new MessageCandidate(
                null,
                CreateSuperProperties(DistinctIdPropertyName, StringPropertyName, DecimalPropertyName),
                null,
                null,
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.DistinctId);

            AssertUserProperties(
                messageCandidate.UserProperties,
                StringPropertyName,
                DecimalPropertyName);
        }

        [Test]
        public void When_RawProperties_Then_AllPropertiesSet()
        {
            var messageCandidate = new MessageCandidate(
                null,
                null,
                CreateRawProperties(DistinctIdPropertyName, StringPropertyName, DecimalPropertyName),
                null,
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.DistinctId);

            AssertUserProperties(
                messageCandidate.UserProperties,
                StringPropertyName,
                DecimalPropertyName);
        }

        [Test]
        public void When_Token_Then_OnlyTokenIsSet()
        {
            var messageCandidate = new MessageCandidate(
                Token,
                null,
                null,
                null,
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.Token);

            Assert.That(messageCandidate.UserProperties.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_DistinctId_Then_OnlyDistinctIdIsSet()
        {
            var messageCandidate = new MessageCandidate(
                null,
                null,
                null,
                DistinctId,
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.DistinctId);

            Assert.That(messageCandidate.UserProperties.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_SuperPropertiesAndRawProperties_Then_AllPropertiesAreSet()
        {
            var messageCandidate = new MessageCandidate(
                null,
                CreateSuperProperties(DistinctIdPropertyName, StringPropertyName, DecimalPropertyName),
                CreateRawProperties(StringPropertyName2, DecimalPropertyName2),
                null,
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.DistinctId);

            AssertUserProperties(
                messageCandidate.UserProperties,
                StringPropertyName,
                DecimalPropertyName,
                StringPropertyName2,
                DecimalPropertyName2);
        }

        [Test]
        public void When_SuperPropertiesAndRawPropertiesHasSameProperties_Then_RawPropertiesOverwritesSuperProperties()
        {
            var messageCandidate = new MessageCandidate(
                null,
                CreateSuperProperties(DistinctIdPropertyName, StringPropertyName, DecimalPropertyName),
                new Dictionary<string, object>
                {
                    { DistinctIdPropertyName, DistinctId },
                    { StringPropertyName, StringPropertyValue }
                },
                null,
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.DistinctId);
            Assert.That(messageCandidate.SpecialProperties[TrackSpecialProperty.DistinctId].Value, Is.EqualTo(DistinctId));

            AssertUserProperties(
                messageCandidate.UserProperties,
                StringPropertyName,
                DecimalPropertyName);
            Assert.That(messageCandidate.UserProperties[StringPropertyName].Value, Is.EqualTo(StringPropertyValue));
            Assert.That(messageCandidate.UserProperties[DecimalPropertyName].Value, Is.Null);
        }

        [Test]
        public void When_DistinctId_Then_DistinctIdOverwritesSuperPropertiesAndRawProperties()
        {
            var messageCandidate = new MessageCandidate(
                null,
                CreateSuperProperties(DistinctIdPropertyName),
                new Dictionary<string, object>
                {
                    { DistinctIdPropertyName, DistinctId }
                },
                DistinctIdInt,
                null,
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.DistinctId);
            Assert.That(messageCandidate.SpecialProperties[TrackSpecialProperty.DistinctId].Value, Is.EqualTo(DistinctIdInt));

            Assert.That(messageCandidate.UserProperties.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_ConfigHasNameFormatting_Then_AllUserPropetiesFormatted()
        {
            var messageCandidate = new MessageCandidate(
                null,
                CreateSuperProperties(DistinctIdPropertyName, "PropertyOne"),
                CreateRawProperties("PropertyTwo"),
                null,
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase },
                TrackSpecialPropertyMapper.RawNameToSpecialProperty);

            AssertSpecialProperties(
                messageCandidate.SpecialProperties,
                TrackSpecialProperty.DistinctId);

            AssertUserProperties(
                messageCandidate.UserProperties,
                "Property One",
                "Property Two");
        }

        private IEnumerable<ObjectProperty> CreateSuperProperties(params string[] propertyNames)
        {
            return propertyNames.Select(propertyName => ObjectProperty.Default(propertyName, PropertyOrigin.SuperProperty));
        }

        private Dictionary<string, object> CreateRawProperties(params string[] propertyNames)
        {
            var rawProperties = new Dictionary<string, object>();
            foreach (string propertyName in propertyNames)
            {
                rawProperties[propertyName] = null;
            }

            return rawProperties;
        }

        private void AssertSpecialProperties(
            Dictionary<string, ObjectProperty> specialProperties,
            params string[] expectedSpecialProperties)
        {
            Assert.That(specialProperties.Count, Is.EqualTo(expectedSpecialProperties.Length));

            foreach (string expectedSpecialProperty in expectedSpecialProperties)
            {
                Assert.That(specialProperties.ContainsKey(expectedSpecialProperty));
            }
        }

        private void AssertUserProperties(
            Dictionary<string, ObjectProperty> userProperties,
            params string[] expectedUserPropertyNames)
        {
            Assert.That(userProperties.Count, Is.EqualTo(expectedUserPropertyNames.Length));

            foreach (string expectedUserPropertyName in expectedUserPropertyNames)
            {
                Assert.That(userProperties.ContainsKey(expectedUserPropertyName));
            }
        }
    }
}