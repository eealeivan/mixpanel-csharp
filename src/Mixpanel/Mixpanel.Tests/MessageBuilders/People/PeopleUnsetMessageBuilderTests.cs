using System.Collections.Generic;
using System.Linq;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageBuilders.People;
using Mixpanel.MessageProperties;
using NUnit.Framework;

namespace Mixpanel.Tests.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$unset": [ "Days Overdue" ]
    // }

    [TestFixture]
    public class PeopleUnsetMessageBuilderTests : MixpanelTestsBase
    {

        [Test]
        public void When_DistinctIdParameter_Then_DistinctIdSetInMessage()
        {
            MessageBuildResult messageBuildResult =
                PeopleUnsetMessageBuilder.Build(Token, null, null, DistinctId, null);

            AssertMessageSuccess(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, DistinctId)
                },
                null);
        }

        [Test]
        public void When_DistinctIdFromSuperProperties_Then_DistinctIdSetInMessage()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId));

            MessageBuildResult messageBuildResult =
                PeopleUnsetMessageBuilder.Build(Token, superProperties, null, null, null);

            AssertMessageSuccess(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, SuperDistinctId)
                },
                null);
        }

        [Test]
        public void When_NonSpecialSuperProperties_Then_Ignored()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId),
                // Should be ignored
                ObjectProperty.Default(DecimalSuperPropertyName, PropertyOrigin.SuperProperty, DecimalSuperPropertyValue));


            MessageBuildResult messageBuildResult =
                PeopleUnsetMessageBuilder.Build(Token, superProperties, null, null, null);

            AssertMessageSuccess(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, SuperDistinctId)
                },
                null);
        }

        [Test]
        public void When_PropertyNames_Then_AllPropertyNamesInMessage()
        {
            MessageBuildResult messageBuildResult =
                PeopleUnsetMessageBuilder.Build(Token, null, StringPropertyArray, DistinctId, null);

            AssertMessageSuccess(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, DistinctId)
                },
                StringPropertyArray);
        }

        [Test]
        public void When_NoToken_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleUnsetMessageBuilder.Build(null, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoDistinctId_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleUnsetMessageBuilder.Build(Token, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        private List<ObjectProperty> CreateSuperProperties(params ObjectProperty[] objectProperties)
        {
            return objectProperties.ToList();
        }

        private void AssertMessageSuccess(
            MessageBuildResult messageBuildResult,
            (string name, object value)[] messageProperties,
            IEnumerable<string> expectedPropertyNames)
        {
            Assert.That(messageBuildResult.Success, Is.True);
            Assert.That(messageBuildResult.Error, Is.Null);

            IDictionary<string, object> message = messageBuildResult.Message;
            Assert.That(message.Count, Is.EqualTo(messageProperties.Length + 1 /*OPERATION_NAME*/));

            foreach ((string propertyName, object expectedValue) in messageProperties)
            {
                bool propertyExists = message.TryGetValue(propertyName, out var actualValue);

                Assert.That(propertyExists, "Missing property: " + propertyName);
                Assert.That(expectedValue, Is.EqualTo(actualValue));
            }

            Assert.That(message.ContainsKey("$unset"));
            var unset = (IEnumerable<string>)message["$unset"];

            if (expectedPropertyNames == null)
            {
                Assert.That(unset.Count(), Is.EqualTo(0));
                return;
            }

            Assert.That(unset, Is.EquivalentTo(expectedPropertyNames));
        }

        private void AssertMessageFail(MessageBuildResult messageBuildResult)
        {
            Assert.That(messageBuildResult.Success, Is.False);
            Assert.That(messageBuildResult.Error, Is.Not.Null);
            Assert.That(messageBuildResult.Message, Is.Null);
        }
    }
}