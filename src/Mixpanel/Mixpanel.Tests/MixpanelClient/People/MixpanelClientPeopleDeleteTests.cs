using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Mixpanel.Tests.Unit.MixpanelClient.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$delete": "",
    //     "$ignore_alias": true 
    // }

    [TestFixture]
    public class MixpanelClientPeopleDeleteTests
    {
        [Test]
        public async Task PeopleDeleteAsync_SendMessage_EngageUrlCalled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            IMixpanelClient client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            await client.PeopleDeleteAsync(distinctId);

            // Assert
            (string endpoint, _) = httpMockMixpanelConfig.Messages.Single();
            endpoint.Should().Be("https://api.mixpanel.com/engage");
        }

        [TestCase(DistinctIdType.Argument, true)]
        [TestCase(DistinctIdType.Argument, false)]
        [TestCase(DistinctIdType.Argument, null)]
        [TestCase(DistinctIdType.SuperProps, true)]
        [TestCase(DistinctIdType.SuperProps, false)]
        [TestCase(DistinctIdType.SuperProps, null)]
        public async Task PeopleDeleteAsync_AllArgumentVariations_CorrectPropertiesInMessage(DistinctIdType distinctIdType, bool? ignoreAlias)
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var superProps = distinctIdType == DistinctIdType.SuperProps ? new { DistinctId = distinctId } : null;
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance, superProps);

            // Act
            bool result;
            switch (distinctIdType)
            {
                case DistinctIdType.Argument when ignoreAlias is null:
                    result = await client.PeopleDeleteAsync(distinctId);
                    break;
                case DistinctIdType.Argument: // ignoreAlias is not null
                    result = await client.PeopleDeleteAsync(distinctId, ignoreAlias: ignoreAlias.Value);
                    break;
                case DistinctIdType.SuperProps when ignoreAlias is null:
                    result = await client.PeopleDeleteAsync();
                    break;
                case DistinctIdType.SuperProps: // ignoreAlias is not null
                    result = await client.PeopleDeleteAsync(ignoreAlias: ignoreAlias.Value);
                    break;
                default:
                    throw new Exception();
            }

            // Assert
            result.Should().Be(true);
            (_, JObject message) = httpMockMixpanelConfig.Messages.Single();
            message.Should().HaveCount(ignoreAlias.Equals(true) ? 4 : 3);
            message.Should().ContainKey("$token").WhoseValue.Value<string>().Should().Be(token);
            message.Should().ContainKey("$distinct_id").WhoseValue.Value<string>().Should().Be(distinctId);
            message.Should().ContainKey("$delete").WhoseValue.Value<string>().Should().Be("");
            if (ignoreAlias.Equals(true))
            {
                message.Should().ContainKey("$ignore_alias").WhoseValue.Value<bool>().Should().Be(true);
            }
        }

        [Test]
        public void PeopleDeleteAsync_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var superProps = new { DistinctId = distinctId };
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance, superProps);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleDeleteAsync(cancellationToken: cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void PeopleDeleteAsyncWithDistinctId_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleDeleteAsync(distinctId, cancellationToken: cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [TestCase(DistinctIdType.Argument, true)]
        [TestCase(DistinctIdType.Argument, false)]
        [TestCase(DistinctIdType.Argument, null)]
        [TestCase(DistinctIdType.SuperProps, true)]
        [TestCase(DistinctIdType.SuperProps, false)]
        [TestCase(DistinctIdType.SuperProps, null)]
        public void GetPeopleDeleteMessage_AllArgumentVariations_CorrectPropertiesInMessage(DistinctIdType distinctIdType, bool? ignoreAlias)
        {
            // Arrange
            var (token, distinctId, _) = GenerateInputs();
            var superProps = distinctIdType == DistinctIdType.SuperProps ? new { DistinctId = distinctId } : null;
            var client = new Mixpanel.MixpanelClient(token, null, superProps);

            // Act
            MixpanelMessage message;
            switch (distinctIdType)
            {
                case DistinctIdType.Argument when ignoreAlias is null:
                    message = client.GetPeopleDeleteMessage(distinctId);
                    break;
                case DistinctIdType.Argument: // ignoreAlias is not null
                    message = client.GetPeopleDeleteMessage(distinctId, ignoreAlias: ignoreAlias.Value);
                    break;
                case DistinctIdType.SuperProps when ignoreAlias is null:
                    message = client.GetPeopleDeleteMessage();
                    break;
                case DistinctIdType.SuperProps: // ignoreAlias is not null
                    message = client.GetPeopleDeleteMessage(ignoreAlias: ignoreAlias.Value);
                    break;
                default:
                    throw new Exception();
            }

            // Assert
            message.Kind.Should().Be(MessageKind.PeopleDelete);
            message.Data.Should().HaveCount(ignoreAlias.Equals(true) ? 4 : 3);
            message.Data.Should().ContainKey("$token").WhoseValue.Should().Be(token);
            message.Data.Should().ContainKey("$distinct_id").WhoseValue.Should().Be(distinctId);
            message.Data.Should().ContainKey("$delete").WhoseValue.Should().Be("");
            if (ignoreAlias.Equals(true))
            {
                message.Data.Should().ContainKey("$ignore_alias").WhoseValue.Should().Be(true);
            }
        }

        [TestCase(DistinctIdType.Argument, true)]
        [TestCase(DistinctIdType.Argument, false)]
        [TestCase(DistinctIdType.Argument, null)]
        [TestCase(DistinctIdType.SuperProps, true)]
        [TestCase(DistinctIdType.SuperProps, false)]
        [TestCase(DistinctIdType.SuperProps, null)]
        public void PeopleDeleteTest_AllArgumentVariations_CorrectPropertiesInMessage(DistinctIdType distinctIdType, bool? ignoreAlias)
        {
            // Arrange
            var (token, distinctId, _) = GenerateInputs();
            var superProps = distinctIdType == DistinctIdType.SuperProps ? new { DistinctId = distinctId } : null;
            var client = new Mixpanel.MixpanelClient(token, null, superProps);

            // Act
            MixpanelMessageTest messageTest;
            switch (distinctIdType)
            {
                case DistinctIdType.Argument when ignoreAlias is null:
                    messageTest = client.PeopleDeleteTest(distinctId);
                    break;
                case DistinctIdType.Argument: // ignoreAlias is not null
                    messageTest = client.PeopleDeleteTest(distinctId, ignoreAlias:ignoreAlias.Value);
                    break;
                case DistinctIdType.SuperProps when ignoreAlias is null:
                    messageTest = client.PeopleDeleteTest();
                    break;
                case DistinctIdType.SuperProps: // ignoreAlias is not null
                    messageTest = client.PeopleDeleteTest(ignoreAlias: ignoreAlias.Value);
                    break;
                default:
                    throw new Exception();
            }

            // Assert
            messageTest.Data.Should().HaveCount(ignoreAlias.Equals(true) ? 4 : 3);
            messageTest.Data.Should().ContainKey("$token").WhoseValue.Should().Be(token);
            messageTest.Data.Should().ContainKey("$distinct_id").WhoseValue.Should().Be(distinctId);
            messageTest.Data.Should().ContainKey("$delete").WhoseValue.Should().Be("");
            if (ignoreAlias.Equals(true))
            {
                messageTest.Data.Should().ContainKey("$ignore_alias").WhoseValue.Should().Be(true);
            }

            messageTest.Json.Should().NotBeNull();
            messageTest.Base64.Should().NotBeNull();
            messageTest.Exception.Should().BeNull();
        }

        private (string token, string distinctId, HttpMockMixpanelConfig<JObject> httpMockMixpanelConfig) GenerateInputs()
        {
            return
                (
                    new Randomizer().AlphaNumeric(32),
                    new Randomizer().AlphaNumeric(10),
                    new HttpMockMixpanelConfig<JObject>()
                );
        }
    }
}