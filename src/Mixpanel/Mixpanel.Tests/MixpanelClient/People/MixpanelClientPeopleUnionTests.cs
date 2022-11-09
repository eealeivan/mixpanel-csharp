using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

// ReSharper disable RedundantArgumentDefaultValue

namespace Mixpanel.Tests.Unit.MixpanelClient.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$union": { "Items purchased": ["socks", "shirts"] }
    // }

    [TestFixture]
    public class MixpanelClientPeopleUnionTests : MixpanelClientPeopleTestsBase
    {
        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromProps_Then_CorrectDataSent()
        {
            await Client.PeopleUnionAsync(GetProperties(includeDistinctId: true));
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.PeopleUnionAsync(GetProperties(includeDistinctId: false));
            AssertSentData(SuperDistinctId);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.PeopleUnionAsync(DistinctId, GetProperties());
            AssertSentData(DistinctId);
        }

        [Test]
        public void PeopleUnionAsync_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var superProps = new { DistinctId = distinctId };
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance, superProps);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleUnionAsync(new { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void PeopleUnionAsyncWithDistinctId_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleUnionAsync(distinctId, new { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleUnionMessage(GetProperties(includeDistinctId: true));
            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleUnionMessage(GetProperties());
            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleUnionMessage(DistinctId, GetProperties());
            AssertMessage(message, DistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleUnionTest(GetProperties(includeDistinctId: true));
            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleUnionTest(GetProperties());
            AssertDictionary(message.Data, SuperDistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleUnionTest(DistinctId, GetProperties());
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
                {DecimalPropertyName, DecimalPropertyArray},
                {StringPropertyName, StringPropertyArray}
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

            var union = (JObject)msg["$union"];
            Assert.That(union.Count, Is.EqualTo(2));
            Assert.That(
                union[DecimalPropertyName].Value<JArray>().ToObject<decimal[]>(), 
                Is.EquivalentTo(DecimalPropertyArray));
            Assert.That(
                union[StringPropertyName].Value<JArray>().ToObject<string[]>(), 
                Is.EquivalentTo(StringPropertyArray));
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnion));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            AssertDictionaryMessageProperties(dic, expectedDistinctId);

            Assert.That(dic["$union"], Is.TypeOf<Dictionary<string, object>>());
            var union = (Dictionary<string, object>)dic["$union"];
            Assert.That(union.Count, Is.EqualTo(2));
            Assert.That(union[DecimalPropertyName], Is.EquivalentTo(DecimalPropertyArray));
            Assert.That(union[StringPropertyName], Is.EquivalentTo(StringPropertyArray));
        }

        private (string token, string distinctId, HttpMockMixpanelConfig<JObject> httpMockMixpanelConfig) GenerateInputs()
        {
            var randomizer = new Randomizer();
            return
            (
                randomizer.AlphaNumeric(32),
                randomizer.AlphaNumeric(10),
                new HttpMockMixpanelConfig<JObject>()
            );
        }
    }
}