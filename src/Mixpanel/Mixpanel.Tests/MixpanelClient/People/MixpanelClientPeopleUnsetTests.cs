using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.Unit.MixpanelClient.People
{
    // Message example:
    // {
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793",
    //     "$unset": [ "Days Overdue" ]
    // }

    [TestFixture]
    public class MixpanelClientPeopleUnsetTests : MixpanelClientPeopleTestsBase
    {
        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.PeopleUnsetAsync(StringPropertyArray);
            AssertSentData(SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.PeopleUnsetAsync(DistinctId, StringPropertyArray);
            AssertSentData(DistinctId);
        }

        [Test]
        public void PeopleUnsetAsync_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var superProps = new { DistinctId = distinctId };
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance, superProps);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleUnsetAsync(new string[] { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void PeopleUnsetAsyncWithDistinctId_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, httpMockMixpanelConfig) = GenerateInputs();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.PeopleUnsetAsync(distinctId, new string[] { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleUnsetMessage(StringPropertyArray);
            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetPeopleUnsetMessage(DistinctId, StringPropertyArray);
            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleUnsetTest(StringPropertyArray);
            AssertDictionary(message.Data, SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.PeopleUnsetTest(DistinctId, StringPropertyArray);
            AssertDictionary(message.Data, DistinctId);
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

            var unset = (JArray)msg["$unset"];
            Assert.That(unset.Count, Is.EqualTo(StringPropertyArray.Length));

            for (int i = 0; i < StringPropertyArray.Length; i++)
            {
                Assert.That(unset[i].Value<string>(), Is.EqualTo(StringPropertyArray[i]));
            }
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnset));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            AssertDictionaryMessageProperties(dic, expectedDistinctId);

            Assert.That(dic["$unset"], Is.AssignableTo<IEnumerable<string>>());
            var unset = ((IEnumerable<string>)dic["$unset"]).ToArray();
            Assert.That(unset.Count, Is.EqualTo(StringPropertyArray.Length));
            for (int i = 0; i < StringPropertyArray.Length; i++)
            {
                Assert.That(unset[i], Is.EqualTo(StringPropertyArray[i]));
            }
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