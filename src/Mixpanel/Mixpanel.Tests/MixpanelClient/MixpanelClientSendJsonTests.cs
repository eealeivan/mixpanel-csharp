using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient
{
    [TestFixture]
    public class MixpanelClientSendJsonTests : MixpanelClientTestsBase
    {
        [Test]
        public async Task When_SendAsync_Then_CorrectDataSent()
        {
            bool result = await Client.SendJsonAsync(MixpanelMessageEndpoint.Track, CreateJsonMessage());

            Assert.That(result, Is.True);
            Assert.That(HttpPostEntries.Single().Endpoint, Is.EqualTo(TrackUrl));
            CheckSendJsonMessage();
        }

        private string CreateJsonMessage()
        {
            var message = new
            {
                @event = Event,
                properties = new
                {
                    token = Token,
                    distinct_id = DistinctId,
                    StringProperty = StringPropertyValue
                }
            };
            string messageJson = JsonConvert.SerializeObject(message);
            return messageJson;
        }

        private void CheckSendJsonMessage()
        {
            var msg = ParseMessageData(HttpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg["event"].Value<string>(), Is.EqualTo(Event));
            var props = (JObject)msg["properties"];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props["token"].Value<string>(), Is.EqualTo(Token));
            Assert.That(props["distinct_id"].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(props[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
        }
    }
}