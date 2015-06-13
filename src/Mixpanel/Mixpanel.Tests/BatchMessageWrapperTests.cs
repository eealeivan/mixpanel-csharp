using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class BatchMessageWrapperTests : MixpanelTestsBase
    {
        private MixpanelClient _mc;

        [SetUp]
        public void SetUp()
        {
            _mc = new MixpanelClient(Token);
        }

        [Test]
        public void AllMessages_DevidedCorrectly()
        {
            var messages = new[]
            {
                // Track
                _mc.GetTrackMessage(Event, DistinctId, DictionaryWithStringProperty),
                _mc.GetAliasMessage(DistinctId, Alias),

                // Engage
                _mc.GetPeopleSetMessage(DistinctId, DictionaryWithStringProperty),
                _mc.GetPeopleSetOnceMessage(DistinctId, DictionaryWithStringProperty),
                _mc.GetPeopleAddMessage(DistinctId, DictionaryWithStringProperty),
                _mc.GetPeopleAppendMessage(DistinctId, DictionaryWithStringProperty),
                _mc.GetPeopleUnionMessage(DistinctId, DictionaryWithStringProperty),
                _mc.GetPeopleUnsetMessage(DistinctId, StringPropertyArray),
                _mc.GetPeopleDeleteMessage(DistinctId),
                _mc.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue)
            };

            var batchMessage = new BatchMessageWrapper(messages);

            // Check track
            Assert.That(batchMessage.TrackMessages.Count, Is.EqualTo(1));
            var trackMessages = batchMessage.TrackMessages.Single();
            Assert.That(trackMessages.Count, Is.EqualTo(2));

            var trackMessage = trackMessages[0];
            Assert.That(trackMessage[MixpanelProperty.TrackEvent], Is.EqualTo(Event));

            var aliasMessage = trackMessages[1];
            Assert.That(aliasMessage[MixpanelProperty.TrackEvent], Is.EqualTo(MixpanelProperty.TrackCreateAlias));

            // Check engage
            Assert.That(batchMessage.EngageMessages.Count, Is.EqualTo(1));
            var engageMessages = batchMessage.EngageMessages.Single();
            Assert.That(engageMessages.Count, Is.EqualTo(8));
        }

        [Test]
        public void OnlyTrackMessages_DevidedCorrectly()
        {
            var messages = new List<MixpanelMessage>();
            for (int i = 0; i < 10; i++)
            {
                messages.Add(
                    _mc.GetTrackMessage(Event, DistinctId, DictionaryWithStringProperty));
            }

            var batchMessage = new BatchMessageWrapper(messages);

            // Check track
            Assert.That(batchMessage.TrackMessages.Count, Is.EqualTo(1));
            Assert.That(batchMessage.TrackMessages[0].Count, Is.EqualTo(10));

            // Check engage
            Assert.That(batchMessage.EngageMessages, Is.Null);
        }

        [Test]
        public void OnlyEngageMessages_DevidedCorrectly()
        {
            var messages = new List<MixpanelMessage>();
            for (int i = 0; i < 10; i++)
            {
                messages.Add(
                    _mc.GetPeopleSetMessage(DistinctId, DictionaryWithStringProperty));
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
        public void BothTypesBoundaryNumbers_DevidedCorrectly(int messagesCount)
        {
            var messages = new List<MixpanelMessage>();
            for (int i = 0; i < messagesCount; i++)
            {
                messages.Add(
                    _mc.GetTrackMessage(Event, DistinctId, DictionaryWithStringProperty));
                messages.Add(
                    _mc.GetPeopleSetMessage(DistinctId, DictionaryWithStringProperty));
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
    }
}