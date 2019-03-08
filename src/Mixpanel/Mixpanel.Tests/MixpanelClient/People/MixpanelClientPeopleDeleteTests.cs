using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$delete": ""
    // }

    [TestFixture]
    public class MixpanelClientPeopleDeleteTests : MixpanelClientPeopleTestsBase
    {
        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_SendSync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            Client.PeopleDelete(DistinctId);
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_SendSync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            Client.PeopleDelete();
            AssertSentData(SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.PeopleDeleteAsync(DistinctId);
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.PeopleDeleteAsync();
            AssertSentData(SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleDeleteMessage(DistinctId);
            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleDeleteMessage();
            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleDeleteTest(DistinctId);
            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleDeleteTest();
            AssertDictionary(message.Data, SuperDistinctId);
        }

        private void AssertSentData(object expectedDistinctId)
        {
            var (endpoint, data) = HttpPostEntries.Single();

            Assert.That(endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(data);
            AssertJson(msg, expectedDistinctId);
        }

        private void AssertJson(JObject msg, object expectedDistinctId)
        {
            AssertJsonMessageProperties(msg, expectedDistinctId);

            var delete = (JValue)msg["$delete"];
            Assert.That(delete.Value, Is.EqualTo(String.Empty));
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleDelete));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            AssertDictionaryMessageProperties(dic, expectedDistinctId);

            Assert.That(dic["$delete"], Is.TypeOf<string>());
            Assert.That(dic["$delete"], Is.EqualTo(String.Empty));
        }
    }
}