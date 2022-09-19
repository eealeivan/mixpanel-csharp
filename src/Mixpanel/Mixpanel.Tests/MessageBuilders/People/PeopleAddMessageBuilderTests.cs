using System.Collections.Generic;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageBuilders.People;
using Mixpanel.MessageProperties;
using NUnit.Framework;

namespace Mixpanel.Tests.Unit.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$add": { "Coins Gathered": 12 }
    // }

    [TestFixture]
    public class PeopleAddMessageBuilderTests : PeopleTestsBase
    {
        protected override string OperationName => "$add";

        [Test]
        public void When_DistinctIdParameter_Then_DistinctIdSetInMessage()
        {
            MessageBuildResult messageBuildResult =
                PeopleAddMessageBuilder.Build(Token, null, null, DistinctId, null);

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
                PeopleAddMessageBuilder.Build(Token, superProperties, null, null, null);

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
                PeopleAddMessageBuilder.Build(Token, superProperties, null, null, null);

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
        public void When_RawProperties_Then_OnlyNumericPropertiesInMessage()
        {
            var rawProperties = new Dictionary<string, object>
            {
                {DistinctIdPropertyName, DistinctId },
                {DecimalPropertyName, DecimalPropertyValue},
                {IntPropertyName, IntPropertyValue },
                // Must be ignored
                {StringPropertyName, StringPropertyValue}
            };

            MessageBuildResult messageBuildResult =
                PeopleAddMessageBuilder.Build(Token, null, rawProperties, null, null);

            AssertMessageSuccess(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, DistinctId)
                },
                new (string name, object value)[]
                {
                    (DecimalPropertyName, DecimalPropertyValue),
                    (IntPropertyName, IntPropertyValue)
                });
        }

        [Test]
        public void When_NoToken_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleAddMessageBuilder.Build(null, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoDistinctId_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleAddMessageBuilder.Build(Token, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }
    }
}