using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient
{
    [TestFixture]
    public class MixpanelClientSendTests
    {
        [TestCase(5, 7, 1 + 1)]
        [TestCase(49, 51, 1 + 2)]
        [TestCase(50, 50, 1 + 1)]
        [TestCase(51, 49, 2 + 1)]
        [TestCase(0, 5, 0 + 1)]
        [TestCase(0, 75, 0 + 2)]
        [TestCase(5, 0, 1 + 0)]
        [TestCase(75, 0, 2 + 0)]
        [TestCase(125, 200, 3  + 4)]
        public async Task SendAsync_VariousNumberOfMessages_CorrectNumberOfBatchesSent(
            int trackMessagesCount, int engageMessagesCount, int expectedBatchesCount)
        {
            // Arrange
            var (token, httpMockMixpanelConfig) = GenerateInputs();
            IMixpanelClient client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);
            var messages = GenerateMessages(client, trackMessagesCount, engageMessagesCount);

            // Act
            SendResult sendResult = await client.SendAsync(messages);

            // Assert
            sendResult.Success.Should().BeTrue();
            sendResult.FailedBatches.Should().BeNullOrEmpty();
            sendResult.SentBatches.Should().HaveCount(expectedBatchesCount);
            httpMockMixpanelConfig.Messages.Should().HaveCount(expectedBatchesCount);
        }

        [TestCase(5, 7, 1 + 1)]
        [TestCase(49, 51, 1 + 2)]
        [TestCase(50, 50, 1 + 1)]
        [TestCase(51, 49, 2 + 1)]
        [TestCase(0, 5, 0 + 1)]
        [TestCase(0, 75, 0 + 2)]
        [TestCase(5, 0, 1 + 0)]
        [TestCase(75, 0, 2 + 0)]
        [TestCase(125, 200, 3 + 4)]
        public void SendTest_VariousNumberOfMessages_CorrectNumberOfBatchesCreated(
            int trackMessagesCount, int engageMessagesCount, int expectedBatchesCount)
        {
            // Arrange
            var (token, _) = GenerateInputs();
            IMixpanelClient client = new Mixpanel.MixpanelClient(token);
            var messages = GenerateMessages(client, trackMessagesCount, engageMessagesCount);

            // Act
            ReadOnlyCollection<MixpanelBatchMessageTest> mixpanelBatchMessageTests =
                client.SendTest(messages);

            // Assert
            mixpanelBatchMessageTests.Should().HaveCount(expectedBatchesCount);
        }

        [Test]
        public void SendAsync_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, httpMockMixpanelConfig) = GenerateInputs();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.SendAsync(GenerateMessages(client, 1, 1), cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        private IList<MixpanelMessage> GenerateMessages(
            IMixpanelClient client, int trackMessagesCount = 0, int engageMessagesCount = 0)
        {
            var randomizer = new Randomizer();

            var messages = new List<MixpanelMessage>(trackMessagesCount + engageMessagesCount);
            for (int i = 0; i < trackMessagesCount; i++)
            {
                var properties = new Dictionary<string, object>
                {
                    { randomizer.Words(), randomizer.Int() }
                };

                messages.Add(client.GetTrackMessage(randomizer.Words(), properties));
            }

            for (int i = 0; i < engageMessagesCount; i++)
            {
                var properties = new Dictionary<string, object>
                {
                    { MixpanelProperty.DistinctId, randomizer.Uuid() },
                    { randomizer.Words(), randomizer.Bool() }
                };

                messages.Add(client.GetPeopleSetMessage(properties));
            }

            return messages;
        }

        private (string token, HttpMockMixpanelConfig<JArray> httpMockMixpanelConfig) GenerateInputs()
        {
            var randomizer = new Randomizer();
            return
            (
                randomizer.AlphaNumeric(32),
                new HttpMockMixpanelConfig<JArray>()
            );
        }
    }
}