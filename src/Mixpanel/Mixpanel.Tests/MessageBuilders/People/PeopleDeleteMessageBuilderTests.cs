using Bogus;
using FluentAssertions;
using Mixpanel.MessageBuilders;
using Mixpanel.MessageBuilders.People;
using Mixpanel.MessageProperties;
using NUnit.Framework;

// ReSharper disable ExpressionIsAlwaysNull

namespace Mixpanel.Tests.Unit.MessageBuilders.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$delete": "",
    //     "$ignore_alias": true 
    // }

    [TestFixture]
    public class PeopleDeleteMessageBuilderTests
    {
        [Test]
        public void Build_FullMessage_AllPropertiesSetInMessage()
        {
            // Arrange
            var (token, distinctId) = GenerateInputs();
            const bool ignoreAlias = true;

            // Act
            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(token, null, distinctId, ignoreAlias, null);

            // Assert
            messageBuildResult.Success.Should().BeTrue();
            messageBuildResult.Error.Should().BeNull();
            messageBuildResult.Message.Count.Should().Be(4);
            messageBuildResult.Message.Should().ContainKey("$delete").WhoseValue.Should().Be("");
            messageBuildResult.Message.Should().ContainKey("$token").WhoseValue.Should().Be(token);
            messageBuildResult.Message.Should().ContainKey("$distinct_id").WhoseValue.Should().Be(distinctId);
            messageBuildResult.Message.Should().ContainKey("$ignore_alias").WhoseValue.Should().Be(ignoreAlias);
        }

        [Test]
        public void Build_DistinctIdProvidedAsArgument_DistinctIdSetInMessage()
        {
            // Arrange
            var (token, distinctId) = GenerateInputs();

            // Act
            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(token, null, distinctId, false, null);

            // Assert
            messageBuildResult.Message.Should().ContainKey("$distinct_id").WhoseValue.Should().Be(distinctId);
        }

        [Test]
        public void Build_DistinctIdProvidedAsSuperProperty_DistinctIdSetInMessage()
        {
            // Arrange
            var (token, distinctId) = GenerateInputs();
            var superProperties = new[]
            {
                new ObjectProperty(MixpanelProperty.DistinctId, PropertyNameSource.Default, PropertyOrigin.SuperProperty, distinctId)
            };

            // Act
            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(token, superProperties, null, false, null);

            // Assert
            messageBuildResult.Message.Should().ContainKey("$distinct_id").WhoseValue.Should().Be(distinctId);
        }

        [Test]
        public void Build_DistinctIdProvidedAsArgumentAndSuperProperty_DistinctIdFromArgumentSetInMessage()
        {
            // Arrange
            var (token, _) = GenerateInputs();
            string argDistinctId = new Randomizer().AlphaNumeric(10);
            string superPropsDistinctId = new Randomizer().AlphaNumeric(10);
            var superProperties = new[]
            {
                new ObjectProperty(MixpanelProperty.DistinctId, PropertyNameSource.Default, PropertyOrigin.SuperProperty, superPropsDistinctId)
            };

            // Act
            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(token, superProperties, argDistinctId, false, null);

            // Assert
            messageBuildResult.Message.Should().ContainKey("$distinct_id").WhoseValue.Should().Be(argDistinctId);
        }

        [Test]
        public void Build_NoTokenProvided_MessageBuildFails()
        {
            // Arrange
            string token = null;
            var (_, distinctId) = GenerateInputs();

            // Act
            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(token, null, distinctId, false, null);

            // Assert
            messageBuildResult.Success.Should().BeFalse();
            messageBuildResult.Message.Should().BeNull();
            messageBuildResult.Error.Should().NotBeNull();
        }

        [Test]
        public void Build_NoDistinctIdProvided_MessageBuildFails()
        {
            // Arrange
            string distinctId = null;
            var (token, _) = GenerateInputs();

            // Act
            MessageBuildResult messageBuildResult =
                PeopleDeleteMessageBuilder.Build(token, null, distinctId, false, null);

            // Assert
            messageBuildResult.Success.Should().BeFalse();
            messageBuildResult.Message.Should().BeNull();
            messageBuildResult.Error.Should().NotBeNull();
        }

        private (string token, string distinctId) GenerateInputs()
        {
            return
            (
                new Randomizer().AlphaNumeric(32),
                new Randomizer().AlphaNumeric(10)
            );
        }
    }
}