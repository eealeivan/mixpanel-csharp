using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
#if !JSON
using Newtonsoft.Json;
#endif

namespace Mixpanel.Tests.MixpanelClient
{
    [TestFixture]
    public class BatchMessageWrapperTests : MixpanelTestsBase
    {
        private Mixpanel.MixpanelClient mixpanelClient;

        [SetUp]
        public void SetUp()
        {
            mixpanelClient = new Mixpanel.MixpanelClient(
                Token,
                new MixpanelConfig
                {
#if !JSON
                       SerializeJsonFn = obj => JsonConvert.SerializeObject(obj)
#endif
                });
        }

        [Test]
        public void When_AllTypeOfMessages_Then_DividedCorrectly()
        {
            var messages = new[]
            {
                // Track
                mixpanelClient.GetTrackMessage(Event, DistinctId, DictionaryWithStringProperty),
                mixpanelClient.GetAliasMessage(DistinctId, Alias),

                // Engage
                mixpanelClient.GetPeopleSetMessage(DistinctId, DictionaryWithStringProperty),
                mixpanelClient.GetPeopleSetOnceMessage(DistinctId, DictionaryWithStringProperty),
                mixpanelClient.GetPeopleAddMessage(DistinctId, DictionaryWithStringProperty),
                mixpanelClient.GetPeopleAppendMessage(DistinctId, DictionaryWithStringProperty),
                mixpanelClient.GetPeopleUnionMessage(DistinctId, DictionaryWithStringProperty),
                mixpanelClient.GetPeopleRemoveMessage(DistinctId, DictionaryWithStringProperty),
                mixpanelClient.GetPeopleUnsetMessage(DistinctId, StringPropertyArray),
                mixpanelClient.GetPeopleDeleteMessage(DistinctId),
                mixpanelClient.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue)
            };

            var batchMessage = new BatchMessageWrapper(messages);

            // Check track
            Assert.That(batchMessage.TrackMessages.Count, Is.EqualTo(1));
            List<MixpanelMessage> trackMessages = batchMessage.TrackMessages.Single();
            Assert.That(trackMessages.Count, Is.EqualTo(2));

            MixpanelMessage trackMessage = trackMessages[0];
            Assert.That(trackMessage.Data["event"], Is.EqualTo(Event));

            MixpanelMessage aliasMessage = trackMessages[1];
            Assert.That(aliasMessage.Data["event"], Is.EqualTo("$create_alias"));

            // Check engage
            Assert.That(batchMessage.EngageMessages.Count, Is.EqualTo(1));
            List<MixpanelMessage> engageMessages = batchMessage.EngageMessages.Single();
            Assert.That(engageMessages.Count, Is.EqualTo(9));
        }

        [Test]
        public void When_OnlyTrackMessages_Then_DividedCorrectly()
        {
            var messages = new List<MixpanelMessage>();
            for (int i = 0; i < 10; i++)
            {
                messages.Add(
                    mixpanelClient.GetTrackMessage(Event, DistinctId, DictionaryWithStringProperty));
            }

            var batchMessage = new BatchMessageWrapper(messages);

            // Check track
            Assert.That(batchMessage.TrackMessages.Count, Is.EqualTo(1));
            Assert.That(batchMessage.TrackMessages[0].Count, Is.EqualTo(10));

            // Check engage
            Assert.That(batchMessage.EngageMessages, Is.Null);
        }

        [Test]
        public void When_OnlyEngageMessages_Then_DividedCorrectly()
        {
            var messages = new List<MixpanelMessage>();
            for (int i = 0; i < 10; i++)
            {
                messages.Add(
                    mixpanelClient.GetPeopleSetMessage(DistinctId, DictionaryWithStringProperty));
            }

            var batchMessage = new BatchMessageWrapper(messages);

            // Check track
            Assert.That(batchMessage.TrackMessages, Is.Null);

            // Check engage
            Assert.That(batchMessage.EngageMessages.Count, Is.EqualTo(1));
            Assert.That(batchMessage.EngageMessages[0].Count, Is.EqualTo(10));
        }

        [TestCase(1)]
        [TestCase(49)]
        [TestCase(50)]
        [TestCase(51)]
        [TestCase(99)]
        [TestCase(100)]
        [TestCase(101)]
        public void When_ManyMessages_Then_MessagesSplitted(int messagesCount)
        {
            var messages = new List<MixpanelMessage>();
            for (int i = 0; i < messagesCount; i++)
            {
                messages.Add(
                    mixpanelClient.GetTrackMessage(Event, DistinctId, DictionaryWithStringProperty));
                messages.Add(
                    mixpanelClient.GetPeopleSetMessage(DistinctId, DictionaryWithStringProperty));
            }

            var batchMessage = new BatchMessageWrapper(messages);
            var splits = GetSplits(messagesCount, BatchMessageWrapper.MaxBatchSize);

            // Check track
            Assert.That(batchMessage.TrackMessages.Count, Is.EqualTo(splits.Count));
            for (int i = 0; i < splits.Count; i++)
            {
                Assert.That(batchMessage.TrackMessages[i].Count, Is.EqualTo(splits[i]));
            }

            // Check engage
            Assert.That(batchMessage.EngageMessages.Count, Is.EqualTo(splits.Count));
            for (int i = 0; i < splits.Count; i++)
            {
                Assert.That(batchMessage.EngageMessages[i].Count, Is.EqualTo(splits[i]));
            }
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
    }
}