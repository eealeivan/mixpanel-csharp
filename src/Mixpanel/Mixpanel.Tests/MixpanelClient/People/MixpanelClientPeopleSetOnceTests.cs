using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
// ReSharper disable RedundantArgumentDefaultValue

namespace Mixpanel.Tests.MixpanelClient.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$set_once": {
    //         "First login date": "2013-04-01T13:20:00"
    //     }
    // }

    [TestFixture]
    public class MixpanelClientPeopleSetOnceTests : MixpanelClientPeopleTestsBase
    {

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromProps_Then_CorrectDataSent()
        {
            await Client.PeopleSetOnceAsync(GetProperties(includeDistinctId: true));
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.PeopleSetOnceAsync(GetProperties(includeDistinctId: false));
            AssertSentData(SuperDistinctId);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.PeopleSetOnceAsync(DistinctId, GetProperties());
            AssertSentData(DistinctId);
        }

        [Test]
        public void PeopleSetOnceAsync_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var superProps = new { DistinctId = distinctId };
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance, superProps);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleSetOnceAsync(new { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void PeopleSetOnceAsyncWithDistinctId_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleSetOnceAsync(distinctId, new { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleSetOnceMessage(GetProperties(includeDistinctId: true));
            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleSetOnceMessage(GetProperties());
            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleSetOnceMessage(DistinctId, GetProperties());
            AssertMessage(message, DistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleSetOnceTest(GetProperties(includeDistinctId: true));
            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleSetOnceTest(GetProperties());
            AssertDictionary(message.Data, SuperDistinctId);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleSetOnceTest(DistinctId, GetProperties());
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
                {MixpanelProperty.FirstName, FirstName},
                {MixpanelProperty.LastName, LastName},
                {MixpanelProperty.Name, Name},
                {MixpanelProperty.Created, Created},
                {MixpanelProperty.Email, Email},
                {MixpanelProperty.Phone, Phone},
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

            var set = (JObject)msg["$set_once"];
            Assert.That(set.Count, Is.EqualTo(8));
            Assert.That(set["$first_name"].Value<string>(), Is.EqualTo(FirstName));
            Assert.That(set["$last_name"].Value<string>(), Is.EqualTo(LastName));
            Assert.That(set["$name"].Value<string>(), Is.EqualTo(Name));
            Assert.That(set["$created"].Value<string>(), Is.EqualTo(CreatedFormat));
            Assert.That(set["$email"].Value<string>(), Is.EqualTo(Email));
            Assert.That(set["$phone"].Value<string>(), Is.EqualTo(Phone));
            Assert.That(set[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
            Assert.That(set[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleSetOnce));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            AssertDictionaryMessageProperties(dic, expectedDistinctId);

            Assert.That(dic["$set_once"], Is.TypeOf<Dictionary<string, object>>());
            var set = (Dictionary<string, object>)dic["$set_once"];
            Assert.That(set.Count, Is.EqualTo(8));
            Assert.That(set["$first_name"], Is.EqualTo(FirstName));
            Assert.That(set["$last_name"], Is.EqualTo(LastName));
            Assert.That(set["$name"], Is.EqualTo(Name));
            Assert.That(set["$created"], Is.EqualTo(CreatedFormat));
            Assert.That(set["$email"], Is.EqualTo(Email));
            Assert.That(set["$phone"], Is.EqualTo(Phone));
            Assert.That(set[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(set[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
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