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
    //     "$union": { "Items purchased": ["socks", "shirts"] }
    // }

    [TestFixture]
    public class PeopleUnionMessageBuilderTests : PeopleTestsBase
    {
        protected override string OperationName => "$union";

        [Test]
        public void When_DistinctIdParameter_Then_DistinctIdSetInMessage()
        {
            MessageBuildResult messageBuildResult =
                PeopleUnionMessageBuilder.Build(Token, null, null, DistinctId, null);

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
                PeopleUnionMessageBuilder.Build(Token, superProperties, null, null, null);

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
                PeopleUnionMessageBuilder.Build(Token, superProperties, null, null, null);

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
        public void When_ValidRawProperties_Then_AllPropertiesInMessage()
        {
            var rawProperties = new Dictionary<string, object>
            {
                {DistinctIdPropertyName, DistinctId },
                {DecimalPropertyName, DecimalPropertyArray},
                {StringPropertyName, StringPropertyArray}
            };

            MessageBuildResult messageBuildResult =
                PeopleUnionMessageBuilder.Build(Token, null, rawProperties, null, null);

            AssertMessageSuccess(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, DistinctId)
                },
                new (string name, object value)[]
                {
                    (DecimalPropertyName, DecimalPropertyArray),
                    (StringPropertyName, StringPropertyArray)
                });
        }

        [Test]
        public void When_InvalidRawProperties_Then_InvalidPropertiesIgnored()
        {
            var rawProperties = new Dictionary<string, object>
            {
                {DistinctIdPropertyName, DistinctId },
                {DecimalPropertyName, DecimalPropertyArray},
                // Must be ignored
                {StringPropertyName, StringPropertyValue},
                {IntPropertyName, IntPropertyValue }
            };

            MessageBuildResult messageBuildResult =
                PeopleUnionMessageBuilder.Build(Token, null, rawProperties, null, null);

            AssertMessageSuccess(
                messageBuildResult,
                new (string name, object value)[]
                {
                    (PeopleSpecialProperty.Token, Token),
                    (PeopleSpecialProperty.DistinctId, DistinctId)
                },
                new (string name, object value)[]
                {
                    (DecimalPropertyName, DecimalPropertyArray)
                });
        }

        [Test]
        public void When_NoToken_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleUnionMessageBuilder.Build(null, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoDistinctId_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = PeopleUnionMessageBuilder.Build(Token, null, null, null, null);
            AssertMessageFail(messageBuildResult);
        }
    }
}