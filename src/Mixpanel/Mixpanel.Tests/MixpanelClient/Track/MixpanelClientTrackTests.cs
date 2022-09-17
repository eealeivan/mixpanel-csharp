using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
// ReSharper disable RedundantArgumentDefaultValue

namespace Mixpanel.Tests.MixpanelClient.Track
{
    // Message example:
    // {
    //     "event": "Signed Up",
    //     "properties": {
    //         "distinct_id": "13793",
    //         "token": "e3bc4100330c35722740fb8c6f5abddc",
    //         "Referred By": "Friend"
    //     }
    // }

    [TestFixture]
    public class MixpanelClientTrackTests : MixpanelClientTrackTestsBase
    {
        [Test]
        public async Task Given_SendAsync_When_NoDistinctId_Then_CorrectDataSent()
        {
            await Client.TrackAsync(Event, GetProperties(includeDistinctId: false));
            AssertSentData(null, 3 + 2 + 0);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromProps_Then_CorrectDataSent()
        {
            await Client.TrackAsync(Event, GetProperties(includeDistinctId: true));
            AssertSentData(DistinctId, 4 + 2 + 0);
        }

        [Test]
        [TrackSuperProps(TrackSuperPropsDetails.DistinctId)]
        public async Task Given_SendAsync_When_DistinctIdFromSuperProps_Then_CorrectDataSent()
        {
            await Client.TrackAsync(Event, GetProperties());
            AssertSentData(SuperDistinctId, 4 + 2 + 0);
        }

        [Test]
        public async Task Given_SendAsync_When_DistinctIdFromParams_Then_CorrectDataSent()
        {
            await Client.TrackAsync(Event, DistinctId, GetProperties());
            AssertSentData(DistinctId, 4 + 2 + 0);
        }

        [Test]
        public void TrackAsync_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, _, @event, httpMockMixpanelConfig) = GenerateInputs();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.TrackAsync(@event, new { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void TrackAsyncWithDistinctId_CancellationRequested_RequestCancelled()
        {
            // Arrange
            var (token, distinctId, @event, httpMockMixpanelConfig) = GenerateInputs();
            var cancellationTokenSource = new CancellationTokenSource();
            var client = new Mixpanel.MixpanelClient(token, httpMockMixpanelConfig.Instance);

            // Act
            var task = Task.Factory.StartNew(
                async () => await client.TrackAsync(@event, distinctId, new { }, cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            task.Wait();

            // Assert
            httpMockMixpanelConfig.RequestCancelled.Should().BeTrue();
        }

        [Test]
        public void Given_GetMessage_When_NoDistinctId_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetTrackMessage(Event, GetProperties(includeDistinctId: false));
            AssertMessage(message, null, 3 + 2 + 0);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetTrackMessage(Event, GetProperties(includeDistinctId: true));
            AssertMessage(message, DistinctId, 4 + 2 + 0);
        }

        [Test]
        [TrackSuperProps(TrackSuperPropsDetails.DistinctId)]
        public void Given_GetMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetTrackMessage(Event, GetProperties());
            AssertMessage(message, SuperDistinctId, 4 + 2 + 0);
        }

        [Test]
        public void Given_GetMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessage message = Client.GetTrackMessage(Event, DistinctId, GetProperties());
            AssertMessage(message, DistinctId, 4 + 2 + 0);
        }

        [Test]
        public void Given_GetTestMessage_When_NoDistinctId_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.TrackTest(Event, GetProperties(includeDistinctId: false));
            AssertDictionary(message.Data, null, 3 + 2 + 0);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.TrackTest(Event, GetProperties(includeDistinctId: true));
            AssertDictionary(message.Data, DistinctId, 4 + 2 + 0);
        }

        [Test]
        [TrackSuperProps(TrackSuperPropsDetails.DistinctId)]
        public void Given_GetTestMessage_When_DistinctIdFromSuperProps_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.TrackTest(Event, GetProperties());
            AssertDictionary(message.Data, SuperDistinctId, 4 + 2 + 0);
        }

        [Test]
        public void Given_GetTestMessage_When_DistinctIdFromParams_Then_CorrectMessageReturned()
        {
            MixpanelMessageTest message = Client.TrackTest(Event, DistinctId, GetProperties());
            AssertDictionary(message.Data, DistinctId, 4 + 2 + 0);
        }

        private Dictionary<string, object> GetProperties(bool includeDistinctId = false)
        {
            var dic = new Dictionary<string, object>
            {
                {MixpanelProperty.Ip, Ip},
                {MixpanelProperty.Time, Time},
                {StringPropertyName, StringPropertyValue},
                {DecimalPropertyName, DecimalPropertyValue},
            };
            IncludeDistinctIdIfNeeded(includeDistinctId, dic);

            return dic;
        }

        private void AssertSentData(object expectedDistinctId, int propsCount)
        {
            var (endpoint, data) = HttpPostEntries.Single();

            Assert.That(endpoint, Is.EqualTo(TrackUrl));

            var msg = ParseMessageData(data);
            AssertJson(msg, expectedDistinctId, propsCount);
        }

        private void AssertJson(JObject msg, object expectedDistinctId, int propsCount)
        {
            AssertJsonSpecialProperties(msg, expectedDistinctId);

            var props = (JObject) msg["properties"];
            Assert.That(props.Count, Is.EqualTo(propsCount));
            Assert.That(props[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));

            if (SuperPropsDetails?.HasFlag(TrackSuperPropsDetails.UserProperties) ?? false)
            {
                Assert.That(
                    props[DecimalSuperPropertyName].Value<decimal>(),
                    Is.EqualTo(DecimalSuperPropertyValue));
                Assert.That(
                    props[StringSuperPropertyName].Value<string>(),
                    Is.EqualTo(StringSuperPropertyValue));
            }
        }

        private void AssertMessage(MixpanelMessage msg, object expectedDistinctId, int propsCount)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.Track));
            AssertDictionary(msg.Data, expectedDistinctId, propsCount);
        }

        private void AssertDictionary(IDictionary<string, object> dic, object expectedDistinctId, int propsCount)
        {
            AssertDictionarySpecialProperties(dic, expectedDistinctId);

            var props = (Dictionary<string, object>) dic["properties"];
            Assert.That(props.Count, Is.EqualTo(propsCount));
            Assert.That(props[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));

            if (SuperPropsDetails?.HasFlag(TrackSuperPropsDetails.UserProperties) ?? false)
            {
                Assert.That(props[DecimalSuperPropertyName], Is.EqualTo(DecimalSuperPropertyValue));
                Assert.That(props[StringSuperPropertyName], Is.EqualTo(StringSuperPropertyValue));
            }
        }

        private (string token, string distinctId, string @event, HttpMockMixpanelConfig<JObject> httpMockMixpanelConfig) GenerateInputs()
        {
            var randomizer = new Randomizer();
            return
            (
                randomizer.AlphaNumeric(32),
                randomizer.AlphaNumeric(10),
                randomizer.Words(),
                new HttpMockMixpanelConfig<JObject>(null)
            );
        }
    }
}