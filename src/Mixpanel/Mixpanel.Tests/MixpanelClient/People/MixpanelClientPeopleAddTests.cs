using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
// ReSharper disable RedundantArgumentDefaultValue

namespace Mixpanel.Tests.MixpanelClient.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$add": { "Coins Gathered": 12 }
    // }

    [TestFixture]
    public class MixpanelClientPeopleAddTests : MixpanelClientPeopleTestsBase
    {
        [Test]
        public void Given_SendSync_When_DistinctIdFromProps_Then_CorrectDataSent()
        {
            Client.PeopleAdd(GetProperties(includeDistinctId: true));
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_SendSync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            Client.PeopleAdd(GetProperties(includeDistinctId: false));
            AssertSentData(SuperDistinctId);
        }

        [Test]
        public void Given_SendSync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            Client.PeopleAdd(DistinctId, GetProperties());
            AssertSentData(DistinctId);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromProps_Then_CorrectDataSent()
        {
            await Client.PeopleAddAsync(GetProperties(includeDistinctId: true));
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.PeopleAddAsync(GetProperties(includeDistinctId: false));
            AssertSentData(SuperDistinctId);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.PeopleAddAsync(DistinctId, GetProperties());
            AssertSentData(DistinctId);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleAddMessage(GetProperties(includeDistinctId: true));
            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleAddMessage(GetProperties());
            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleAddMessage(DistinctId, GetProperties());
            AssertMessage(message, DistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleAddTest(GetProperties(includeDistinctId: true));
            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleAddTest(GetProperties());
            AssertDictionary(message.Data, SuperDistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleAddTest(DistinctId, GetProperties());
            AssertDictionary(message.Data, DistinctId);
        }

        private Dictionary<string, object> GetProperties(bool includeDistinctId = false)
        {
            var dic = new Dictionary<string, object>
            {
                {MixpanelProperty.Ip, Ip},
                {MixpanelProperty.Time, Time},
                {MixpanelProperty.IgnoreTime, IgnoreTime},
                {MixpanelProperty.IgnoreAlias, IgnoreAlias},
                {DecimalPropertyName, DecimalPropertyValue},
                {IntPropertyName, IntPropertyValue},
                {DoublePropertyName, DoublePropertyValue},
                // Must be ignored because only numeric are valid
                {StringPropertyName, StringPropertyValue}
            };

            IncludeDistinctIdIfNeeded(includeDistinctId, dic);

            return dic;
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

            var add = (JObject)msg["$add"];
            Assert.That(add.Count, Is.EqualTo(3));
            Assert.That(add[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
            Assert.That(add[IntPropertyName].Value<decimal>(), Is.EqualTo(IntPropertyValue));
            Assert.That(add[DoublePropertyName].Value<decimal>(), Is.EqualTo(DoublePropertyValue));
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleAdd));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            AssertDictionaryMessageProperties(dic, expectedDistinctId);

            Assert.That(dic["$add"], Is.TypeOf<Dictionary<string, object>>());
            var add = (Dictionary<string, object>)dic["$add"];
            Assert.That(add.Count, Is.EqualTo(3));
            Assert.That(add[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
            Assert.That(add[IntPropertyName], Is.EqualTo(IntPropertyValue));
            Assert.That(add[DoublePropertyName], Is.EqualTo(DoublePropertyValue));
        }
    }
}