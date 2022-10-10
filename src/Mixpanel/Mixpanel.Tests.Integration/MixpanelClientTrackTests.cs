using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using NUnit.Framework;

namespace Mixpanel.Tests.Integration
{
    public class MixpanelClientTrackTests
    {
        [Test]
        public async Task TrackAsync_EventOnly_MessageSent()
        {
            // Arrange
            var (token, _, _, @event) = GenerateInputs();
            var client = new MixpanelClient(token);

            // Act
            var result = await client.TrackAsync(@event, new {});
 
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task TrackAsync_SimpleMessage_CorrectDataSavedInMixpanel()
        {
            // Arrange
            var (token, _, insertId, @event) = GenerateInputs();
            var client = new MixpanelClient(token);

            // Act
            await client.TrackAsync(@event, new Dictionary<string, object>
            {
                { "$insert_id", insertId },
                //{ MixpanelProperty.DistinctId, "john.doe@gmail.com" },
                //{ "app_version", "1.1.34.694.gac68a2b3" },
                //{ "song_id", "0wwPcA6wtMf6HUMpIRdeP7" },
            });

            // Assert
            var json = await MixpanelApi.GetRecentEvent(SecretsManager.MixpanelProjectId, insertId);
        }

        private (string token, string distinctId, string insertId, string @event) GenerateInputs()
        {
            var randomizer = new Randomizer();
            return
            (
                SecretsManager.MixpanelProjectToken,
                randomizer.AlphaNumeric(10),
                randomizer.AlphaNumeric(20),
                randomizer.Words()
            );
        }
    }
}