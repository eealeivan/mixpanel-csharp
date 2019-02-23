using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient
{
    [TestFixture]
    public class MixpanelClientSendTests : MixpanelClientTestsBase
    {
        [TestCase(5, 7)]
        [TestCase(49, 51)]
        [TestCase(50, 50)]
        [TestCase(51, 49)]
        [TestCase(51, 49)]
        [TestCase(0, 5)]
        [TestCase(0, 75)]
        [TestCase(5, 0)]
        [TestCase(75, 0)]
        [TestCase(125, 200)]
        public void Given_SendSync_When_Enumerable_Then_CorrectDataSent(
            int trackMessagesCount, int engageMessagesCount)
        {
            SendResult res = Client.Send(GetSendMessages(trackMessagesCount, engageMessagesCount));

            Assert.That(res.Success, Is.EqualTo(true));
            CheckSend(trackMessagesCount, engageMessagesCount);
        }

        [Test]
        public void Given_SendSync_When_Params_Then_CorrectDataSent()
        {
            var messages = GetSendMessages(2, 2);
            SendResult res = Client.Send(messages[0], messages[1], messages[2], messages[3]);

            Assert.That(res.Success, Is.EqualTo(true));
            CheckSend(2, 2);
        }

        [TestCase(5, 7)]
        [TestCase(49, 51)]
        [TestCase(50, 50)]
        [TestCase(51, 49)]
        [TestCase(51, 49)]
        [TestCase(0, 5)]
        [TestCase(0, 75)]
        [TestCase(5, 0)]
        [TestCase(75, 0)]
        [TestCase(125, 200)]
        public async Task Given_SendAsync_When_Enumerable_Then_CorrectDataSent(
            int trackMessagesCount, int engageMessagesCount)
        {
            SendResult res = await Client.SendAsync(GetSendMessages(trackMessagesCount, engageMessagesCount));

            Assert.That(res.Success, Is.EqualTo(true));
            CheckSend(trackMessagesCount, engageMessagesCount);
        }

        [Test]
        public async Task Given_SendAsync_When_Params_Then_CorrectDataSent()
        {
            var messages = GetSendMessages(2, 2);
            SendResult res = await Client.SendAsync(messages[0], messages[1], messages[2], messages[3]);

            Assert.That(res.Success, Is.EqualTo(true));
            CheckSend(2, 2);
        }

        [Test]
        public void Given_SendTest_When_CorrectMessages_Then_CorrectDataReturned()
        {
            ReadOnlyCollection<MixpanelBatchMessageTest> mixpanelBatchMessageTests = 
                Client.SendTest(GetSendMessages(10, 10));

            Assert.That(mixpanelBatchMessageTests.Count, Is.EqualTo(2));
            foreach (var mixpanelBatchMessageTest in mixpanelBatchMessageTests)
            {
                Assert.That(mixpanelBatchMessageTest.Data.Count, Is.EqualTo(10));
                Assert.That(mixpanelBatchMessageTest.Json, Is.Not.Null);
                Assert.That(mixpanelBatchMessageTest.Base64, Is.Not.Null);
                Assert.That(mixpanelBatchMessageTest.Exception, Is.Null);
            }
        }

        private IList<MixpanelMessage> GetSendMessages(
            int trackMessagesCount = 0, int engageMessagesCount = 0)
        {
            var messages = new List<MixpanelMessage>(trackMessagesCount + engageMessagesCount);
            for (int i = 0; i < trackMessagesCount; i++)
            {
                messages.Add(
                    Client.GetTrackMessage(Event, GetTrackDictionary()));
            }

            for (int i = 0; i < engageMessagesCount; i++)
            {
                messages.Add(
                    Client.GetPeopleSetMessage(GetPeopleSetDictionary()));
            }

            return messages;
        }

        private object GetTrackDictionary()
        {
            var dic = new Dictionary<string, object>
            {
                {MixpanelProperty.Ip, Ip},
                {StringPropertyName, StringPropertyValue}
            };
            return dic;
        }

        private void AssertTrackJson(JObject msg)
        {
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg["event"].Value<string>(), Is.EqualTo(Event));

            var props = (JObject)msg["properties"];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props["token"].Value<string>(), Is.EqualTo(Token));
            Assert.That(props["ip"].Value<string>(), Is.EqualTo(Ip));
            Assert.That(props[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
        }

        private object GetPeopleSetDictionary()
        {
            var dic = new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId },
                {MixpanelProperty.Ip, Ip},
                {StringPropertyName, StringPropertyValue}
            };
            return dic;
        }

        private void AssertPeopleSetJson(JObject msg)
        {
            Assert.That(msg.Count, Is.EqualTo(4));
            Assert.That(msg["$token"].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg["$distinct_id"].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(msg["$ip"].Value<string>(), Is.EqualTo(Ip));

            var set = (JObject)msg["$set"];
            Assert.That(set.Count, Is.EqualTo(1));
            Assert.That(set[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
        }

        private void CheckSend(int trackMessagesCount, int engageMessagesCount)
        {
            // Track
            var trackSplits = GetSplits(trackMessagesCount, BatchMessageWrapper.MaxBatchSize);
            var trackHttpPostEntries = HttpPostEntries.Where(x => x.Endpoint == TrackUrl).ToList();
            Assert.That(trackHttpPostEntries.Count, Is.EqualTo(trackSplits.Count));
            for (int i = 0; i < trackHttpPostEntries.Count; i++)
            {
                JArray msg = ParseBatchMessageData(trackHttpPostEntries[i].Data);
                Assert.That(msg.Count, Is.EqualTo(trackSplits[i]));
                foreach (JObject msgPart in msg.Cast<JObject>())
                {
                    AssertTrackJson(msgPart);
                }
            }

            // Engage
            var engageSplits = GetSplits(engageMessagesCount, BatchMessageWrapper.MaxBatchSize);
            var engageHttpPostEntries = HttpPostEntries.Where(x => x.Endpoint == EngageUrl).ToList();
            Assert.That(engageHttpPostEntries.Count, Is.EqualTo(engageSplits.Count));
            for (int i = 0; i < engageHttpPostEntries.Count; i++)
            {
                JArray msg = ParseBatchMessageData(engageHttpPostEntries[i].Data);
                Assert.That(msg.Count, Is.EqualTo(engageSplits[i]));
                foreach (JObject msgPart in msg.Cast<JObject>())
                {
                    AssertPeopleSetJson(msgPart);
                }
            }

            Assert.That(HttpPostEntries.Count, Is.EqualTo(trackSplits.Count + engageSplits.Count));
        }

        private List<int> GetSplits(int number, int batchSize)
        {
            if (number <= 0)
            {
                return new List<int>(0);
            }

            var list = new List<int> { batchSize };
            while (true)
            {
                if (list.Sum() >= number)
                {
                    break;
                }
                list.Add(batchSize);
            }
            list[list.Count - 1] = batchSize - (list.Sum() - number);
            return list;
        }

        private JArray ParseBatchMessageData(string data)
        {
            // Can't use JArray.Parse because it's not possible to disable DateTime parsing
            using (var stringReader = new StringReader(GetJsonFromData(data)))
            {
                using (JsonReader jsonReader = new JsonTextReader(stringReader))
                {
                    jsonReader.DateParseHandling = DateParseHandling.None;
                    JArray msg = JArray.Load(jsonReader);
                    return msg;
                }
            }
        }
    }
}