using System;
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
    //     "$delete": ""
    // }

    [TestFixture]
    public class PeopleDeleteMessageBuilderTests : PeopleTestsBase
    {
        protected override string OperationName => "$delete";

        [Test]
        public void When_DistinctIdParameter_Then_DistinctIdSetInMessage()
        {
            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(Token, null, DistinctId, null);

            AssertDeleteMessageSuccess(messageBuildResult, DistinctId);
        }

        [Test]
        public void When_DistinctIdFromSuperProperties_Then_DistinctIdSetInMessage()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId));

            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(Token, superProperties, null, null);

            AssertDeleteMessageSuccess(messageBuildResult, SuperDistinctId);
        }

        [Test]
        public void When_NonSpecialSuperProperties_Then_Ignored()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId),
                // Should be ignored
                ObjectProperty.Default(DecimalSuperPropertyName, PropertyOrigin.SuperProperty, DecimalSuperPropertyValue));


            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(Token, superProperties, null, null);

            AssertDeleteMessageSuccess(messageBuildResult, SuperDistinctId);
        }

        [Test]
        public void When_NoToken_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleDeleteMessageBuilder.Build(null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoDistinctId_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleDeleteMessageBuilder.Build(Token, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        private void AssertDeleteMessageSuccess(MessageBuildResult messageBuildResult, object expectedDistinctId)
        {
            AssertMessageSuccess(messageBuildResult);

            AssetMessageProperties(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, expectedDistinctId)
                });

            AssertOperation(
                messageBuildResult,
                operation =>
                {
                    Assert.That(operation, Is.TypeOf<string>());
                    Assert.That(operation, Is.EqualTo(String.Empty));
                });
        }
    }
}