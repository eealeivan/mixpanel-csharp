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
    //     "$remove": { "Items purchased": "socks" }
    // }

    [TestFixture]
    public class MixpanelClientPeopleRemoveTests : MixpanelClientPeopleTestsBase
    {

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromProps_Then_CorrectDataSent()
        {
            await Client.PeopleRemoveAsync(GetProperties(includeDistinctId: true));
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.PeopleRemoveAsync(GetProperties(includeDistinctId: false));
            AssertSentData(SuperDistinctId);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.PeopleRemoveAsync(DistinctId, GetProperties());
            AssertSentData(DistinctId);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleRemoveMessage(GetProperties(includeDistinctId: true));
            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleRemoveMessage(GetProperties());
            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleRemoveMessage(DistinctId, GetProperties());
            AssertMessage(message, DistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleRemoveTest(GetProperties(includeDistinctId: true));
            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleRemoveTest(GetProperties());
            AssertDictionary(message.Data, SuperDistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleRemoveTest(DistinctId, GetProperties());
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
                {StringPropertyName, StringPropertyValue},
                {DecimalPropertyName, DecimalPropertyValue}
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

            var remove = (JObject)msg["$remove"];
            Assert.That(remove.Count, Is.EqualTo(2));
            Assert.That(remove[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
            Assert.That(remove[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleRemove));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            AssertDictionaryMessageProperties(dic, expectedDistinctId);

            Assert.That(dic["$remove"], Is.TypeOf<Dictionary<string, object>>());
            var remove = (Dictionary<string, object>)dic["$remove"];
            Assert.That(remove.Count, Is.EqualTo(2));
            Assert.That(remove[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(remove[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }
    }
}