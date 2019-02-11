using System.Collections.Generic;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageBuilders.People;
using Mixpanel.MessageProperties;
using NUnit.Framework;

namespace Mixpanel.Tests.MessageBuilders.People
{
    // Message example:
    // {
    //     "$append": {
    //         "$transactions": {
    //             "$time": "2013-01-03T09:00:00",
    //             "$amount": 25.34
    //         }
    //     },
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793"
    // }

    [TestFixture]
    public class PeopleTrackChargeMessageBuilderTests : PeopleTestsBase
    {
        protected override string OperationName => "$append";

        [Test]
        public void When_DistinctIdParameter_Then_DistinctIdSetInMessage()
        {
            MessageBuildResult messageBuildResult =
                PeopleTrackChargeMessageBuilder.Build(
                    Token, null, DecimalPropertyValue, Time, DistinctId, null);

            AssertTrackChargeMessageSuccess(messageBuildResult, DistinctId);
        }

        [Test]
        public void When_DistinctIdFromSuperProperties_Then_DistinctIdSetInMessage()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId));

            MessageBuildResult messageBuildResult =
                PeopleTrackChargeMessageBuilder.Build(
                    Token, superProperties, DecimalPropertyValue, Time, null, null);

            AssertTrackChargeMessageSuccess(messageBuildResult, SuperDistinctId);
        }

        [Test]
        public void When_NonSpecialSuperProperties_Then_Ignored()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId),
                // Should be ignored
                ObjectProperty.Default(DecimalSuperPropertyName, PropertyOrigin.SuperProperty, DecimalSuperPropertyValue));


            MessageBuildResult messageBuildResult =
                PeopleTrackChargeMessageBuilder.Build(
                    Token, superProperties, DecimalPropertyValue, Time, null, null);

            AssertTrackChargeMessageSuccess(messageBuildResult, SuperDistinctId);
        }

        [Test]
        public void When_NoToken_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleTrackChargeMessageBuilder.Build(
                null, null, DecimalPropertyValue, Time, null, null);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoDistinctId_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleTrackChargeMessageBuilder.Build(
                Token, null, DecimalPropertyValue, Time, null, null);
            AssertMessageFail(messageBuildResult);
        }

        private void AssertTrackChargeMessageSuccess(MessageBuildResult messageBuildResult, object expectedDistinctId)
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
                    var append = (IDictionary<string, object>)operation;
                    Assert.That(append.Count, Is.EqualTo(1));

                    var transactions = (IDictionary<string, object>)append["$transactions"];
                    Assert.That(transactions.Count, Is.EqualTo(2));
                    Assert.That(transactions["$time"], Is.EqualTo(TimeFormat));
                    Assert.That(transactions["$amount"], Is.EqualTo(DecimalPropertyValue));
                });
        }
    }
}