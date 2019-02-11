using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageBuilders.Track;
using Mixpanel.MessageProperties;
using Mixpanel.Parsers;
using NUnit.Framework;

namespace Mixpanel.Tests.MessageBuilders.Track
{
    // Message example:
    // {
    //     "event": "Signed Up",
    //     "properties": {
    //         "distinct_id": "13793",
    //         "token": "e3bc4100330c35722740fb8c6f5abddc",
    //         "Referred By": "Friend"
    //     }
    // }

    [TestFixture]
    public class TrackMessageBuilderTests : MixpanelTestsBase
    {
        [Test]
        public void When_DistinctIdParameter_Then_DistinctIdSetInMessage()
        {
            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, null, null, DistinctId, null);

            AssertMessageSuccess(messageBuildResult, Token, Event, DistinctId);
        }

        [Test]
        public void When_DistinctIdFromSuperProperties_Then_DistinctIdSetInMessage()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId));

            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, superProperties, null, null, null);

            AssertMessageSuccess(messageBuildResult, Token, Event, SuperDistinctId);
        }

        [Test]
        public void When_DistinctIdFromRawProperties_Then_DistinctIdSetInMessage()
        {
            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, null, new { DistinctId }, null, null);

            AssertMessageSuccess(messageBuildResult, Token, Event, DistinctId);
        }

        [Test]
        public void When_SuperProperties_Then_AllPropertiesInMessage()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(StringPropertyName, PropertyOrigin.SuperProperty, StringPropertyValue),
                ObjectProperty.Default(DecimalPropertyName, PropertyOrigin.SuperProperty, DecimalPropertyValue));

            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, superProperties, null, DistinctId, null);

            AssertMessageSuccess(
                messageBuildResult,
                Token,
                Event,
                DistinctId,
                new KeyValuePair<string, object>(StringPropertyName, StringPropertyValue),
                new KeyValuePair<string, object>(DecimalPropertyName, DecimalPropertyValue));
        }

        [Test]
        public void When_RawProperties_Then_AllPropertiesInMessage()
        {
            var rawProperties = new Dictionary<string, object>
            {
                {StringPropertyName, StringPropertyValue},
                {IntPropertyName, IntPropertyValue },
                {DecimalPropertyName, DecimalPropertyArray}
            };

            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, null, rawProperties, DistinctId, null);

            AssertMessageSuccess(
                messageBuildResult,
                Token,
                Event,
                DistinctId,
                new KeyValuePair<string, object>(StringPropertyName, StringPropertyValue),
                new KeyValuePair<string, object>(IntPropertyName, IntPropertyValue),
                new KeyValuePair<string, object>(DecimalPropertyName, DecimalPropertyArray));
        }

        [Test]
        public void When_SuperPropertiesAndRawProperties_Then_RawPropertiesOverwriteSuperProperties()
        {
            var superProperties = CreateSuperProperties(
                // Must be overwritten by message property
                ObjectProperty.Default(StringPropertyName2, PropertyOrigin.SuperProperty, StringPropertyValue2 + "-Super"),
                ObjectProperty.Default(IntPropertyName, PropertyOrigin.SuperProperty, IntPropertyValue));

            var rawProperties = new Dictionary<string, object>
            {
                {StringPropertyName, StringPropertyValue},
                {StringPropertyName2, StringPropertyValue2},
                {DecimalPropertyName, DecimalPropertyValue }
            };


            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, superProperties, rawProperties, DistinctId, null);

            AssertMessageSuccess(
                messageBuildResult,
                Token,
                Event,
                DistinctId,
                new KeyValuePair<string, object>(StringPropertyName, StringPropertyValue),
                new KeyValuePair<string, object>(StringPropertyName2, StringPropertyValue2),
                new KeyValuePair<string, object>(DecimalPropertyName, DecimalPropertyValue),
                new KeyValuePair<string, object>(IntPropertyName, IntPropertyValue));
        }

        [Test]
        public void When_AllSpecialProperties_Then_AllPropertiesInMessage()
        {
            var rawProperties = new
            {
                DistinctId,
                Time,
                Ip,
                Duration,
                Os,
                ScreenWidth,
                ScreenHeight
            };

            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, null, rawProperties, null, null);

            AssertMessageSuccess(
                messageBuildResult,
                Token,
                Event,
                DistinctId,
                new KeyValuePair<string, object>(TrackSpecialProperty.Time, TimeUnix),
                new KeyValuePair<string, object>(TrackSpecialProperty.Ip, Ip),
                new KeyValuePair<string, object>(TrackSpecialProperty.Duration, DurationSeconds),
                new KeyValuePair<string, object>(TrackSpecialProperty.Os, Os),
                new KeyValuePair<string, object>(TrackSpecialProperty.ScreenWidth, ScreenWidth),
                new KeyValuePair<string, object>(TrackSpecialProperty.ScreenHeight, ScreenHeight));
        }

        [Test]
        public void When_PredefinedPropertiesUsed_Then_AllPropertiesInMessage()
        {
            var rawProperties = new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId },
                {MixpanelProperty.Time, Time },
                {MixpanelProperty.Ip, Ip },
                {MixpanelProperty.Duration, Duration },
                {MixpanelProperty.Os, Os },
                {MixpanelProperty.ScreenWidth, ScreenWidth },
                {MixpanelProperty.ScreenHeight, ScreenHeight }
            };

            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, null, rawProperties, null, null);

            AssertMessageSuccess(
                messageBuildResult,
                Token,
                Event,
                DistinctId,
                new KeyValuePair<string, object>(TrackSpecialProperty.Time, TimeUnix),
                new KeyValuePair<string, object>(TrackSpecialProperty.Ip, Ip),
                new KeyValuePair<string, object>(TrackSpecialProperty.Duration, DurationSeconds),
                new KeyValuePair<string, object>(TrackSpecialProperty.Os, Os),
                new KeyValuePair<string, object>(TrackSpecialProperty.ScreenWidth, ScreenWidth),
                new KeyValuePair<string, object>(TrackSpecialProperty.ScreenHeight, ScreenHeight));
        }

        [Test]
        public void When_NameFormattingConfigured_Then_FormattingAppliedToPropertyNames()
        {
            var config = new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase };

            var superProperties = CreateSuperProperties(
                // Must be overwritten by message property
                ObjectProperty.Default("PropertyA", PropertyOrigin.SuperProperty, 10),
                ObjectProperty.Default("PropertyC", PropertyOrigin.SuperProperty, 3));

            var rawProperties = new
            {
                PropertyA = 1,
                PropertyB = 2
            };

            MessageBuildResult messageBuildResult =
                TrackMessageBuilder.Build(Token, Event, superProperties, rawProperties, DistinctId, config);

            AssertMessageSuccess(
                messageBuildResult,
                Token,
                Event,
                DistinctId,
                new KeyValuePair<string, object>("Property A", 1),
                new KeyValuePair<string, object>("Property B", 2),
                new KeyValuePair<string, object>("Property C", 3));
        }

        [Test]
        public void When_NoToken_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = TrackMessageBuilder.Build(null, Event, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoEvent_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = TrackMessageBuilder.Build(Token, null, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        private List<ObjectProperty> CreateSuperProperties(params ObjectProperty[] objectProperties)
        {
            return objectProperties.ToList();
        }

        private void AssertMessageSuccess(
            MessageBuildResult messageBuildResult,
            string token,
            string @event,
            string distinctId,
            params KeyValuePair<string, object>[] customProperties)
        {
            Assert.That(messageBuildResult.Success, Is.True);
            Assert.That(messageBuildResult.Error, Is.Null);

            IDictionary<string, object> message = messageBuildResult.Message;
            Assert.That(message.Count, Is.EqualTo(2));

            Assert.That(message["event"], Is.EqualTo(@event));

            var properties = message["properties"] as IDictionary<string, object>;
            Assert.That(properties, Is.Not.Null);
            Assert.That(properties.Count, Is.EqualTo(2 + (customProperties != null ? customProperties.Length : 0)));
            Assert.That(properties["token"], Is.EqualTo(token));
            Assert.That(properties["distinct_id"], Is.EqualTo(distinctId));

            foreach (KeyValuePair<string, object> kv in customProperties ?? new KeyValuePair<string, object>[0])
            {
                string propertyName = kv.Key;
                object expectedValue = kv.Value;

                bool propertyExists = properties.TryGetValue(propertyName, out var actualValue);
                Assert.That(propertyExists, "Missing property: " + propertyName);

                if (CollectionParser.IsCollection(actualValue))
                {
                    Assert.That(expectedValue, Is.EquivalentTo((IEnumerable)actualValue));
                }
                else
                {
                    Assert.That(expectedValue, Is.EqualTo(actualValue));
                }
            }
        }

        private void AssertMessageFail(MessageBuildResult messageBuildResult)
        {
            Assert.That(messageBuildResult.Success, Is.False);
            Assert.That(messageBuildResult.Error, Is.Not.Null);
            Assert.That(messageBuildResult.Message, Is.Null);
        }
    }
}