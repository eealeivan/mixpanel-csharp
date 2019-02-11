using System.Collections.Generic;
using System.Linq;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageBuilders.Track;
using Mixpanel.MessageProperties;
using NUnit.Framework;

namespace Mixpanel.Tests.MessageBuilders.Track
{
    [TestFixture]
    public class AliasMessageBuilderTests : MixpanelTestsBase
    {

        // Message example:
        // {
        //     "event": "$create_alias",
        //     "properties": {
        //         "token": "e3bc4100330c35722740fb8c6f5abddc",
        //         "distinct_id": "ORIGINAL_ID",
        //         "alias": "NEW_ID"
        //     }
        // }

        [Test]
        public void When_DistinctIdAndAliasFromParams_Then_AllPropertiesSet()
        {
            MessageBuildResult messageBuildResult = 
                AliasMessageBuilder.Build(Token, null, DistinctId, Alias);

            AssertMessageSuccess(messageBuildResult, Token, DistinctId, Alias);
        }

        [Test]
        public void When_DistinctIdFromSuperPropsAndAliasFromParams_Then_AllPropertiesSet()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId));

            MessageBuildResult messageBuildResult = AliasMessageBuilder.Build(Token, superProperties, null, Alias);

            AssertMessageSuccess(messageBuildResult, Token, SuperDistinctId, Alias);
        }

        [Test]
        public void When_DistinctIdFromParamsAndSuperProps_Then_DistinctIdFromParamsOverwritesSuperProps()
        {
            var superProperties = CreateSuperProperties(
                ObjectProperty.Default(DistinctIdPropertyName, PropertyOrigin.SuperProperty, SuperDistinctId));

            MessageBuildResult messageBuildResult = AliasMessageBuilder.Build(Token, superProperties, DistinctId, Alias);
            AssertMessageSuccess(messageBuildResult, Token, DistinctId, Alias);
        }

        [Test]
        public void When_NoToken_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = AliasMessageBuilder.Build(null, null, DistinctId, Alias);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoDistinctId_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = AliasMessageBuilder.Build(Token, null, null, Alias);
            AssertMessageFail(messageBuildResult);
        }

        [Test]
        public void When_NoAlias_Then_MessageBuildFails()
        {
            MessageBuildResult messageBuildResult = AliasMessageBuilder.Build(Token, null, DistinctId, null);
            AssertMessageFail(messageBuildResult);
        }

        private List<ObjectProperty> CreateSuperProperties(params ObjectProperty[] objectProperties)
        {
            return objectProperties.ToList();
        }

        private void AssertMessageSuccess(MessageBuildResult messageBuildResult, string token, string distinctId, string alias)
        {
            Assert.That(messageBuildResult.Success, Is.True);
            Assert.That(messageBuildResult.Error, Is.Null);

            IDictionary<string, object> message = messageBuildResult.Message;
            Assert.That(message.Count, Is.EqualTo(2));

            Assert.That(message["event"], Is.EqualTo("$create_alias"));

            var properties = message["properties"] as IDictionary<string, object>;
            Assert.That(properties, Is.Not.Null);
            Assert.That(properties.Count, Is.EqualTo(3));
            Assert.That(properties["token"], Is.EqualTo(token));
            Assert.That(properties["distinct_id"], Is.EqualTo(distinctId));
            Assert.That(properties["alias"], Is.EqualTo(alias));
        }

        private void AssertMessageFail(MessageBuildResult messageBuildResult)
        {
            Assert.That(messageBuildResult.Success, Is.False);
            Assert.That(messageBuildResult.Error, Is.Not.Null);
            Assert.That(messageBuildResult.Message, Is.Null);
        }
    }
}