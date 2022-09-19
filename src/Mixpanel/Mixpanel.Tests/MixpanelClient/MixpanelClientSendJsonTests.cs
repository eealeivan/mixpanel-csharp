using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient
{
    [TestFixture]
    public class MixpanelClientSendJsonTests
    {
        [Test]
        public async Task SendAsync_TrackMessage_CorrectDataSent()
        {
            // Arrange
            var httpMockMixpanelConfig = new HttpMockMixpanelConfig<JObject>();
            var client = new Mixpanel.MixpanelClient(httpMockMixpanelConfig.Instance);
            var message = CreateJsonMessage();

            // Act
            var result = await client.SendJsonAsync(MixpanelMessageEndpoint.Track, message);

            // Assert
            result.Should().BeTrue();
            var (endpoint, sentMessage) = httpMockMixpanelConfig.Messages.Single();
            endpoint.Should().Be("https://api.mixpanel.com/track");
            sentMessage.ToString(Formatting.None).Should().Be(message);
        }

        [Test]
        public void SendAsync_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var httpMockMixpanelConfig = new HttpMockMixpanelConfig<JObject>();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(httpMockMixpanelConfig.Instance);
            var jsonMessage = CreateJsonMessage();

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.SendJsonAsync(
                    MixpanelMessageEndpoint.Track, jsonMessage, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        private string CreateJsonMessage()
        {
            var randomizer = new Randomizer();
            var message = new
            {
                @event = randomizer.Words(),
                properties = new
                {
                    token = randomizer.AlphaNumeric(30),
                    distinct_id = randomizer.AlphaNumeric(10)
                }
            };

            return JsonConvert.SerializeObject(message);
        }
    }
}