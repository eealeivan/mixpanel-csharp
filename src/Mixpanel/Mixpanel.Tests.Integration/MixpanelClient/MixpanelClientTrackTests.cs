using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Mixpanel.Tests.Integration.MixpanelClient
{
    public class MixpanelClientTrackTests
    {
        [Test]
        public async Task TrackAsync_SimpleMessage_TrueReturned()
        {
            // Arrange
            var (token, _, _, @event) = GenerateInputs();
            var client = new Mixpanel.MixpanelClient(token);

            // Act
            var result = await client.TrackAsync(@event, new { });

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task TrackAsync_SimpleMessage_CorrectDataSavedInMixpanel()
        {
            // Arrange
            var (token, distinctId, insertId, @event) = GenerateInputs();
            var client = new Mixpanel.MixpanelClient(token);

            // Act
            await client.TrackAsync(@event, new Dictionary<string, object>
            {
                { "$insert_id", insertId },
                { MixpanelProperty.DistinctId, distinctId }
            });

            // Assert
            JObject message = await MixpanelExportApi.GetRecentMessage(insertId);
            message.Should().ContainKey("event").WhoseValue.Value<string>().Should().Be(@event);

            JObject messageProperties = (JObject)message["properties"];
            messageProperties.Should().ContainKey("$insert_id").WhoseValue.Value<string>().Should().Be(insertId);
            messageProperties.Should().ContainKey("distinct_id").WhoseValue.Value<string>().Should().Be(distinctId);
        }

        private (string token, string distinctId, string insertId, string @event) GenerateInputs()
        {
            var randomizer = new Randomizer();
            return
            (
                SecretsProvider.MixpanelProjectToken,
                randomizer.AlphaNumeric(10),
                randomizer.AlphaNumeric(20),
                randomizer.Words()
            );
        }
    }
}