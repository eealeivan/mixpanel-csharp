using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient.Track
{
    [TestFixture]
    public class MixpanelClientAliasTests : MixpanelClientTrackTestsBase
    {
        [Test]
        [TrackSuperProps(TrackSuperPropsDetails.DistinctId)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.AliasAsync(Alias);
            AssertSentData(SuperDistinctId);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.AliasAsync(DistinctId, Alias);
            AssertSentData(DistinctId);
        }

        [Test]
        [TrackSuperProps(TrackSuperPropsDetails.DistinctId)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetAliasMessage(Alias);
            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetAliasMessage(DistinctId, Alias);
            AssertMessage(message, DistinctId);
        }

        [Test]
        [TrackSuperProps(TrackSuperPropsDetails.DistinctId)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.AliasTest(Alias);
            AssertDictionary(message.Data, SuperDistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.AliasTest(DistinctId, Alias);
            AssertDictionary(message.Data, DistinctId);
        }

        private void AssertSentData(object expectedDistinctId)
        {
            var (endpoint, data) = HttpPostEntries.Single();

            Assert.That(endpoint, Is.EqualTo(TrackUrl));

            var msg = ParseMessageData(data);
            AssertJson(msg, expectedDistinctId);
        }

        private void AssertJson(JObject msg, object expectedDistinctId)
        {
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg["event"].Value<string>(), Is.EqualTo("$create_alias"));

            var props = (JObject)msg["properties"];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props["token"].Value<string>(), Is.EqualTo(Token));
            Assert.That(props["distinct_id"].Value<string>(), Is.EqualTo(expectedDistinctId));
            Assert.That(props["alias"].Value<string>(), Is.EqualTo(Alias));
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.Alias));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            Assert.That(dic.Count, Is.EqualTo(2));
            Assert.That(dic["event"], Is.EqualTo("$create_alias"));
            Assert.That(dic["properties"], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)dic["properties"];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props["token"], Is.EqualTo(Token));
            Assert.That(props["distinct_id"], Is.EqualTo(expectedDistinctId));
            Assert.That(props["alias"], Is.EqualTo(Alias));
        }
    }
}