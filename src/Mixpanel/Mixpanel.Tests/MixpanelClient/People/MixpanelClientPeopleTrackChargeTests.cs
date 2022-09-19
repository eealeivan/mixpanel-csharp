using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient.People
{
    // Message example:
    // {
    //     "$append": {
    //         "$transactions": {
    //             "$time": "2013-01-03T09:00:00",
    //             "$amount": 25.34
    //         }
    //     },
    //     "$token": "36ada5b10da39a1347559321baf13063",
    //     "$distinct_id": "13793"
    // }

    // TODO: All special properties, user super props
    [TestFixture]
    public class MixpanelClientPeopleTrackChargeTests : MixpanelClientPeopleTestsBase
    {
        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            bool res = await Client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            bool res = await Client.PeopleTrackChargeAsync(DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            AssertSentData(SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_DistinctIdFromParamsAndSuperProps_Then_CorrectDataSent()
        {
            bool res = await Client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_TimeFromParams_Then_CorrectDataSent()
        {
            bool res = await Client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            AssertSentData(DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public async Task Given_SendAsync_When_NoTime_Then_CorrectDataSent()
        {
            Client.UtcNow = () => Time;
            bool res = await Client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue);

            Assert.That(res, Is.True);
            AssertSentData(DistinctId);
        }

        [TestCase(DistinctIdType.Argument, true)]
        [TestCase(DistinctIdType.Argument, false)]
        [TestCase(DistinctIdType.SuperProps, true)]
        [TestCase(DistinctIdType.SuperProps, false)]
        public void PeopleTrackChargeAsyncAllVariations_CancellationRequested_RequestCancelled(
            DistinctIdType distinctIdType, bool setTime)
        {
            // Arrange
            var (token, distinctId, amount, time, httpMockMixpanelConfig) = GenerateInputs();
            var superProps = distinctIdType == DistinctIdType.SuperProps ? new { DistinctId = distinctId } : null;
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance, superProps);

            // Act
            Func<Task<bool>> function;
            switch (distinctIdType)
            {
                case DistinctIdType.Argument when setTime:
                    function = async () => await client.PeopleTrackChargeAsync(distinctId, amount, time, cancellationTokenSource.Token);
                    break;
                case DistinctIdType.Argument: // setTime is false
                    function = async () => await client.PeopleTrackChargeAsync(distinctId, amount, cancellationTokenSource.Token);
                    break;
                case DistinctIdType.SuperProps when setTime:
                    function = async () => await client.PeopleTrackChargeAsync(amount, time, cancellationTokenSource.Token);
                    break;
                case DistinctIdType.SuperProps: // setTime is false
                    function = async () => await client.PeopleTrackChargeAsync(amount, cancellationTokenSource.Token);
                    break;
                default:
                    throw new Exception();
            }
            
            var task = Task.Factory.StartNew(function);
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            MixpanelMessage message = Client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue, Time);

            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            MixpanelMessage message = Client.GetPeopleTrackChargeMessage(DecimalPropertyValue, Time);

            AssertMessage(message, SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_DistinctIdFromParamsAndSuperProps_Then_CorrectDataSent()
        {
            MixpanelMessage message = Client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue, Time);

            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_TimeFromParams_Then_CorrectDataSent()
        {
            MixpanelMessage message = Client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue, Time);

            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetMessage_When_NoTime_Then_CorrectDataSent()
        {
            Client.UtcNow = () => Time;
            MixpanelMessage message = Client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue);

            AssertMessage(message, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            MixpanelMessageTest message = Client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue, Time);

            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            MixpanelMessageTest message = Client.PeopleTrackChargeTest(DecimalPropertyValue, Time);

            AssertDictionary(message.Data, SuperDistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.DistinctId | PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_DistinctIdFromParamsAndSuperProps_Then_CorrectDataSent()
        {
            MixpanelMessageTest message = Client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue, Time);

            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_TimeFromParams_Then_CorrectDataSent()
        {
            MixpanelMessageTest message = Client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue, Time);

            AssertDictionary(message.Data, DistinctId);
        }

        [Test]
        [PeopleSuperProps(PeopleSuperPropsDetails.MessageSpecialProperties)]
        public void Given_GetTestMessage_When_NoTime_Then_CorrectDataSent()
        {
            Client.UtcNow = () => Time;
            MixpanelMessageTest message = Client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue);

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

            var append = (JObject)msg["$append"];
            Assert.That(append.Count, Is.EqualTo(1));
            var transactions = (JObject)append["$transactions"];
            Assert.That(transactions.Count, Is.EqualTo(2));
            Assert.That(transactions["$time"].Value<string>(), Is.EqualTo(TimeFormat));
            Assert.That(transactions["$amount"].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleTrackCharge));
            AssertDictionary(msg.Data, expectedDistinctId);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId)
        {
            AssertDictionaryMessageProperties(dic, expectedDistinctId);

            var append = (Dictionary<string, object>)dic["$append"];
            Assert.That(append.Count, Is.EqualTo(1));
            Assert.That(append["$transactions"], Is.TypeOf<Dictionary<string, object>>());
            var transactions = (Dictionary<string, object>)append["$transactions"];
            Assert.That(transactions.Count, Is.EqualTo(2));
            Assert.That(transactions["$time"], Is.EqualTo(TimeFormat));
            Assert.That(transactions["$amount"], Is.EqualTo(DecimalPropertyValue));
        }

        private (string token, string distinctId, decimal amount, DateTime time, HttpMockMixpanelConfig<JObject> httpMockMixpanelConfig) GenerateInputs()
        {
            var randomizer = new Randomizer();
            var date = new Faker().Date;
            return
            (
                randomizer.AlphaNumeric(32),
                randomizer.AlphaNumeric(10),
                randomizer.Decimal(),
                date.Recent(),
                new HttpMockMixpanelConfig<JObject>()
            );
        }
    }
}