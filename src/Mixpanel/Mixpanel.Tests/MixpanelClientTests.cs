using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
#if !NET35
using System.Threading.Tasks;
#endif
using Mixpanel.Core;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelClientTests : MixpanelTestsBase
    {
        private MixpanelClient _client;

        private class HttpPostEntry
        {
            public string Endpoint { get; set; }
            public string Data { get; set; }

            public HttpPostEntry(string endpoint, string data)
            {
                Endpoint = endpoint;
                Data = data;
            }
        }
        private List<HttpPostEntry> _httpPostEntries;

        public string TrackUrl
        {
            get { return string.Format(MixpanelClient.UrlFormat, MixpanelClient.EndpointTrack); }
        }

        public string EngageUrl
        {
            get { return string.Format(MixpanelClient.UrlFormat, MixpanelClient.EndpointEngage); }
        }

        [SetUp]
        public void SetUp()
        {
            _httpPostEntries = new List<HttpPostEntry>();
            _client = new MixpanelClient(Token,
                new MixpanelConfig
                {
                    HttpPostFn = (endpoint, data) =>
                    {
                        _httpPostEntries.Add(new HttpPostEntry(endpoint, data));
                        return true;
                    },
#if !(NET40 || NET35)
                    AsyncHttpPostFn = (endpoint, data) =>
                    {
                        _httpPostEntries.Add(new HttpPostEntry(endpoint, data));
                        return Task.Run(() => true);
                    }
#endif
                });
            _client.UtcNow = () => DateTime.UtcNow;
            _client.SetSuperProperties(new object());
        }

        #region Track

        [Test]
        public void Track_AnonymousObject_CorrectDataSent()
        {
            _client.Track(Event, DistinctId, GetTrackObject());
            CheckTrack();
        }

        [Test]
        public void Track_AnonymousObjectWithDistinctId_CorrectDataSent()
        {
            _client.Track(Event, GetTrackObject(includeDistinctId: true));
            CheckTrack();
        }

        [Test]
        public void Track_Dictionary_CorrectDataSent()
        {
            _client.Track(Event, DistinctId, GetTrackDictionary());
            CheckTrack();
        }

        [Test]
        public void Track_DictionaryWithDistinctId_CorrectDataSent()
        {
            _client.Track(Event, GetTrackDictionary(includeDistinctId: true));
            CheckTrack();
        }

#if !(NET35 || NET40)
        [Test]
        public async void TrackAsync_AnonymousObject_CorrectDataSent()
        {
            await _client.TrackAsync(Event, DistinctId, GetTrackObject());
            CheckTrack();
        }

        [Test]
        public async void TrackAsync_AnonymousObjectWithDistinctId_CorrectDataSent()
        {
            await _client.TrackAsync(Event, GetTrackObject(includeDistinctId: true));
            CheckTrack();
        }


        [Test]
        public async void TrackAsync_Dictionary_CorrectDataSent()
        {
            await _client.TrackAsync(Event, DistinctId, GetTrackDictionary());
            CheckTrack();
        }

        [Test]
        public async void TrackAsync_DictionaryWithDistinctId_CorrectDataSent()
        {
            await _client.TrackAsync(Event, GetTrackDictionary(includeDistinctId: true));
            CheckTrack();
        }
#endif

        [Test]
        public void GetTrackMessage_Object_CorrectMessageReturned()
        {
            MixpanelMessage msg = _client.GetTrackMessage(Event, DistinctId, GetTrackObject());
            CheckTrackMessage(msg);
        }

        [Test]
        public void GetTrackMessage_ObjectWithDistinctId_CorrectMessageReturned()
        {
            MixpanelMessage msg = _client.GetTrackMessage(Event, GetTrackObject(includeDistinctId: true));
            CheckTrackMessage(msg);
        }

        [Test]
        public void GetTrackMessage_Dictionary_CorrectMessageReturned()
        {
            MixpanelMessage msg = _client.GetTrackMessage(Event, DistinctId, GetTrackDictionary());
            CheckTrackMessage(msg);
        }

        [Test]
        public void GetTrackMessage_DictionaryWithDistinctId_CorrectMessageReturned()
        {
            MixpanelMessage msg = _client.GetTrackMessage(Event, GetTrackDictionary(includeDistinctId: true));
            CheckTrackMessage(msg);
        }

        [Test]
        public void TrackTest_AnonymousObject_CorrectValuesReturned()
        {
            var msg = _client.TrackTest(Event, DistinctId, GetTrackObject());
            CheckTrackDictionary(msg.Data);
        }

        [Test]
        public void TrackTest_AnonymousObjectWithDistinctId_CorrectValuesReturned()
        {
            var msg = _client.TrackTest(Event, GetTrackObject(includeDistinctId: true));
            CheckTrackDictionary(msg.Data);
        }

        [Test]
        public void TrackTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.TrackTest(Event, DistinctId, GetTrackDictionary());
            CheckTrackDictionary(msg.Data);
        }

        [Test]
        public void TrackTest_DictionaryWithDistinctId_CorrectValuesReturned()
        {
            var msg = _client.TrackTest(Event, GetTrackDictionary(includeDistinctId: true));
            CheckTrackDictionary(msg.Data);
        }

        private object GetTrackObject(bool includeDistinctId = false)
        {
            if (includeDistinctId)
            {
                return new
                {
                    Ip,
                    Time,
                    DistinctId,
                    StringProperty = StringPropertyValue,
                    DecimalProperty = DecimalPropertyValue
                };
            }

            return new
            {
                Ip,
                Time,
                StringProperty = StringPropertyValue,
                DecimalProperty = DecimalPropertyValue
            };
        }

        private Dictionary<string, object> GetTrackDictionary(bool includeDistinctId = false)
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

        private void CheckTrack()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(TrackUrl));

            JObject msg = ParseMessageData(_httpPostEntries.Single().Data);
            CheckTrackJsonMessage(msg);
        }

        private void CheckTrackJsonMessage(JObject msg)
        {
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg[MixpanelProperty.TrackEvent].Value<string>(), Is.EqualTo(Event));
            var props = (JObject)msg[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(6));
            Assert.That(props[MixpanelProperty.TrackToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(props[MixpanelProperty.TrackIp].Value<string>(), Is.EqualTo(Ip));
            Assert.That(props[MixpanelProperty.TrackTime].Value<long>(), Is.EqualTo(TimeUnix));
            Assert.That(props[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private static void CheckTrackDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(2));
            Assert.That(dic[MixpanelProperty.TrackEvent], Is.EqualTo(Event));
            Assert.That(dic[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)dic[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(6));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(DistinctId));
            Assert.That(props[MixpanelProperty.TrackIp], Is.EqualTo(Ip));
            Assert.That(props[MixpanelProperty.TrackTime], Is.EqualTo(TimeUnix));
            Assert.That(props[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        private static void CheckTrackMessage(MixpanelMessage msg)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.Track));
            CheckTrackDictionary(msg.Data);
        }

        #endregion Track

        #region Alias

        [Test]
        public void Alias_ValidValues_CorrectDataSent()
        {
            _client.Alias(DistinctId, Alias);
            CheckAlias();
        }

#if !(NET40 || NET35)
        [Test]
        public async void AliasAsync_ValidValues_CorrectDataSent()
        {
            await _client.AliasAsync(DistinctId, Alias);
            CheckAlias();
        }
#endif

        [Test]
        public void GetAliasMessage_ValidValues_CorrectMessageReturned()
        {
            var msg = _client.GetAliasMessage(DistinctId, Alias);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.Alias));
            CheckAliasDictionary(msg.Data);
        }

        [Test]
        public void AliasTest_ValidValues_CorrectValuesReturned()
        {
            var msg = _client.AliasTest(DistinctId, Alias);
            CheckAliasDictionary(msg.Data);
        }

        private void CheckAlias()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(TrackUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg[MixpanelProperty.TrackEvent].Value<string>(), Is.EqualTo(MixpanelProperty.TrackCreateAlias));
            var props = (JObject)msg[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(props[MixpanelProperty.TrackAlias].Value<string>(), Is.EqualTo(Alias));
        }

        private void CheckAliasDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(2));
            Assert.That(dic[MixpanelProperty.TrackEvent], Is.EqualTo(MixpanelProperty.TrackCreateAlias));
            Assert.That(dic[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)dic[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(DistinctId));
            Assert.That(props[MixpanelProperty.TrackAlias], Is.EqualTo(Alias));
        }

        #endregion Alias

        #region PeopleSet

        [Test]
        public void PeopleSet_Dictionary_CorrectDataSent()
        {
            _client.PeopleSet(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSet();
        }

        [Test]
        public void PeopleSet_DictionaryWithDistinctId_CorrectDataSent()
        {
            _client.PeopleSet(GetPeopleSetDictionary(includeDistinctId: true));
            CheckPeopleSet();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleSetAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleSetAsync(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSet();
        }

        [Test]
        public async void PeopleSetAsync_DictionaryWithDistinctId_CorrectDataSent()
        {
            await _client.PeopleSetAsync(GetPeopleSetDictionary(includeDistinctId: true));
            CheckPeopleSet();
        }
#endif

        [Test]
        public void GetPeopleSetMessage_Dictionary_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleSetMessage(DistinctId, GetPeopleSetDictionary());
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleSet));
            CheckPeopleSetDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleSetMessage_DictionaryWithDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleSetMessage(GetPeopleSetDictionary(includeDistinctId: true));
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleSet));
            CheckPeopleSetDictionary(msg.Data);
        }

        [Test]
        public void PeopleSetTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetTest(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSetDictionary(msg.Data);
        }

        [Test]
        public void PeopleSetTest_DictionaryWithDistinctId_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetTest(GetPeopleSetDictionary(includeDistinctId: true));
            CheckPeopleSetDictionary(msg.Data);
        }

        private Dictionary<string, object> GetPeopleSetDictionary(bool includeDistinctId = false)
        {
            var dic = new Dictionary<string, object>
            {
                {MixpanelProperty.Ip, Ip},
                {MixpanelProperty.Time, Time},
                {MixpanelProperty.IgnoreTime, IgnoreTime},
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

        private void CheckPeopleSet()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            CheckPeopleSetJsonMessage(msg);
        }

        private void CheckPeopleSetJsonMessage(JObject msg)
        {
            Assert.That(msg.Count, Is.EqualTo(6));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(msg[MixpanelProperty.PeopleIp].Value<string>(), Is.EqualTo(Ip));
            Assert.That(msg[MixpanelProperty.PeopleTime].Value<long>(), Is.EqualTo(TimeUnix));
            Assert.That(msg[MixpanelProperty.PeopleIgnoreTime].Value<bool>(), Is.EqualTo(IgnoreTime));
            var set = (JObject) msg[MixpanelProperty.PeopleSet];
            Assert.That(set.Count, Is.EqualTo(8));
            Assert.That(set[MixpanelProperty.PeopleFirstName].Value<string>(), Is.EqualTo(FirstName));
            Assert.That(set[MixpanelProperty.PeopleLastName].Value<string>(), Is.EqualTo(LastName));
            Assert.That(set[MixpanelProperty.PeopleName].Value<string>(), Is.EqualTo(Name));
            Assert.That(ToDateTimeString(set[MixpanelProperty.PeopleCreated]), Is.EqualTo(CreatedFormat));
            Assert.That(set[MixpanelProperty.PeopleEmail].Value<string>(), Is.EqualTo(Email));
            Assert.That(set[MixpanelProperty.PeoplePhone].Value<string>(), Is.EqualTo(Phone));
            Assert.That(set[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
            Assert.That(set[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private void CheckPeopleSetDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(6));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleIp], Is.EqualTo(Ip));
            Assert.That(dic[MixpanelProperty.PeopleTime], Is.EqualTo(TimeUnix));
            Assert.That(dic[MixpanelProperty.PeopleIgnoreTime], Is.EqualTo(IgnoreTime));
            Assert.That(dic[MixpanelProperty.PeopleSet], Is.TypeOf<Dictionary<string, object>>());
            var set = (Dictionary<string, object>)dic[MixpanelProperty.PeopleSet];
            Assert.That(set.Count, Is.EqualTo(8));
            Assert.That(set[MixpanelProperty.PeopleFirstName], Is.EqualTo(FirstName));
            Assert.That(set[MixpanelProperty.PeopleLastName], Is.EqualTo(LastName));
            Assert.That(set[MixpanelProperty.PeopleName], Is.EqualTo(Name));
            Assert.That(set[MixpanelProperty.PeopleCreated], Is.EqualTo(CreatedFormat));
            Assert.That(set[MixpanelProperty.PeopleEmail], Is.EqualTo(Email));
            Assert.That(set[MixpanelProperty.PeoplePhone], Is.EqualTo(Phone));
            Assert.That(set[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(set[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        #endregion PeopleSet

        #region PeopleSetOnce

        [Test]
        public void PeopleSetOnce_Dictionary_CorrectDataSent()
        {
            _client.PeopleSetOnce(DistinctId, GetPeopleSetOnceDictionary());
            CheckPeopleSetOnce();
        }

        [Test]
        public void PeopleSetOnce_DictionaryWithDistinctId_CorrectDataSent()
        {
            _client.PeopleSetOnce(GetPeopleSetOnceDictionary(includeDistinctId: true));
            CheckPeopleSetOnce();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleSetOnceAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleSetOnceAsync(DistinctId, GetPeopleSetOnceDictionary());
            CheckPeopleSetOnce();
        }

        [Test]
        public async void PeopleSetOnceAsync_DictionaryWithDistinctId_CorrectDataSent()
        {
            await _client.PeopleSetOnceAsync(GetPeopleSetOnceDictionary(includeDistinctId: true));
            CheckPeopleSetOnce();
        }
#endif

        [Test]
        public void GetPeopleSetOnceMessage_Dictionary_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleSetOnceMessage(DistinctId, GetPeopleSetOnceDictionary());
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleSetOnce));
            CheckPeopleSetOnceDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleSetOnceMessage_DictionaryWithDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleSetOnceMessage(GetPeopleSetOnceDictionary(includeDistinctId: true));
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleSetOnce));
            CheckPeopleSetOnceDictionary(msg.Data);
        }

        [Test]
        public void PeopleSetOnceTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetOnceTest(DistinctId, GetPeopleSetOnceDictionary());
            CheckPeopleSetOnceDictionary(msg.Data);
        }

        [Test]
        public void PeopleSetOnceTest_DictionaryWithDistinctId_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetOnceTest(GetPeopleSetOnceDictionary(includeDistinctId: true));
            CheckPeopleSetOnceDictionary(msg.Data);
        }

        private Dictionary<string, object> GetPeopleSetOnceDictionary(bool includeDistinctId = false)
        {
            var dic = new Dictionary<string, object>
            {
                {MixpanelProperty.IgnoreTime, IgnoreTime},
                {StringPropertyName, StringPropertyValue},
                {DecimalPropertyName, DecimalPropertyValue}
            };
            IncludeDistinctIdIfNeeded(includeDistinctId, dic);

            return dic;
        }

        private void CheckPeopleSetOnce()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(4));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(msg[MixpanelProperty.PeopleIgnoreTime].Value<bool>(), Is.EqualTo(IgnoreTime));
            var setOnce = (JObject)msg[MixpanelProperty.PeopleSetOnce];
            Assert.That(setOnce.Count, Is.EqualTo(2));
            Assert.That(setOnce[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
            Assert.That(setOnce[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private void CheckPeopleSetOnceDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(4));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleIgnoreTime], Is.EqualTo(IgnoreTime));
            Assert.That(dic[MixpanelProperty.PeopleSetOnce], Is.TypeOf<Dictionary<string, object>>());
            var setOnce = (Dictionary<string, object>)dic[MixpanelProperty.PeopleSetOnce];
            Assert.That(setOnce.Count, Is.EqualTo(2));
            Assert.That(setOnce[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(setOnce[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        #endregion PeopleSetOnce

        #region PeopleAdd

        [Test]
        public void PeopleAdd_NumericInput_CorrectDataSent()
        {
            _client.PeopleAdd(DistinctId, GetPeopleAddDictionary());
            CheckPeopleAdd();
        }

        [Test]
        public void PeopleAdd_NumericInputWithDistinctId_CorrectDataSent()
        {
            _client.PeopleAdd(GetPeopleAddDictionary(includeDistinctId: true));
            CheckPeopleAdd();
        }

        [Test]
        public void PeopleAdd_MixedInput_CorrectDataSent()
        {
            _client.PeopleAdd(DistinctId, GetPeopleAddDictionary(includeNonNumericValues: true));
            CheckPeopleAdd();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleAddAsync_NumericInput_CorrectDataSent()
        {
            await _client.PeopleAddAsync(DistinctId, GetPeopleAddDictionary());
            CheckPeopleAdd();
        }

        [Test]
        public async void PeopleAddAsync_NumericInputWithDistinctId_CorrectDataSent()
        {
            await _client.PeopleAddAsync(GetPeopleAddDictionary(includeDistinctId: true));
            CheckPeopleAdd();
        }

        [Test]
        public async void PeopleAddAsync_MixedInput_CorrectDataSent()
        {
            await _client.PeopleAddAsync(DistinctId, GetPeopleAddDictionary(includeNonNumericValues: true));
            CheckPeopleAdd();
        }
#endif

        [Test]
        public void GetPeopleAddMessage_Dictionary_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleAddMessage(DistinctId, GetPeopleAddDictionary());
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleAdd));
            CheckPeopleAddDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleAddMessage_DictionaryWithDistinctIdAndNonNumericValues_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleAddMessage(
                GetPeopleAddDictionary(includeDistinctId: true, includeNonNumericValues: true));
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleAdd));
            CheckPeopleAddDictionary(msg.Data);
        }

        [Test]
        public void PeopleAddTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleAddTest(DistinctId, GetPeopleAddDictionary());
            CheckPeopleAddDictionary(msg.Data);
        }

        [Test]
        public void PeopleAddTest_DictionaryWithDistinctIdAndNonNumericValues_CorrectValuesReturned()
        {
            var msg = _client.PeopleAddTest(
                GetPeopleAddDictionary(includeDistinctId: true, includeNonNumericValues: true));
            CheckPeopleAddDictionary(msg.Data);
        }

        private Dictionary<string, object> GetPeopleAddDictionary(
            bool includeDistinctId = false, bool includeNonNumericValues = false)
        {
            var dic = new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {IntPropertyName, IntPropertyValue},
                {DoublePropertyName, DoublePropertyValue}
            };

            IncludeDistinctIdIfNeeded(includeDistinctId, dic);

            if (includeNonNumericValues)
            {
                dic[MixpanelProperty.Created] = Time;
                dic[StringPropertyName] = StringPropertyValue;
            }

            return dic;
        }

        private void CheckPeopleAdd()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            var add = (JObject)msg[MixpanelProperty.PeopleAdd];
            Assert.That(add.Count, Is.EqualTo(3));
            Assert.That(add[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
            Assert.That(add[IntPropertyName].Value<int>(), Is.EqualTo(IntPropertyValue));
            Assert.That(add[DoublePropertyName].Value<double>(), Is.EqualTo(DoublePropertyValue));
        }

        private void CheckPeopleAddDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleAdd], Is.TypeOf<Dictionary<string, object>>());
            var add = (Dictionary<string, object>)dic[MixpanelProperty.PeopleAdd];
            Assert.That(add.Count, Is.EqualTo(3));
            Assert.That(add[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
            Assert.That(add[IntPropertyName], Is.EqualTo(IntPropertyValue));
            Assert.That(add[DoublePropertyName], Is.EqualTo(DoublePropertyValue));
        }

        #endregion PeopleAdd

        #region PeopleAppend

        [Test]
        public void PeopleAppend_Dictionary_CorrectDataSent()
        {
            _client.PeopleAppend(DistinctId, GetPeopleAppendDictionary());
            CheckPeopleAppend();
        }

        [Test]
        public void PeopleAppend_DictionaryWithDistinctId_CorrectDataSent()
        {
            _client.PeopleAppend(GetPeopleAppendDictionary(includeDistinctId: true));
            CheckPeopleAppend();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleAppendAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleAppendAsync(DistinctId, GetPeopleAppendDictionary());
            CheckPeopleAppend();
        }

        [Test]
        public async void PeopleAppendAsync_DictionaryWithDistinctId_CorrectDataSent()
        {
            await _client.PeopleAppendAsync(GetPeopleAppendDictionary(includeDistinctId: true));
            CheckPeopleAppend();
        }
#endif

        [Test]
        public void GetPeopleAppendMessage_Dictionary_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleAppendMessage(DistinctId, GetPeopleAppendDictionary());
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleAppend));
            CheckPeopleAppendDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleAppendMessage_DictionaryWithDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleAppendMessage(GetPeopleAppendDictionary(includeDistinctId: true));
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleAppend));
            CheckPeopleAppendDictionary(msg.Data);
        }

        [Test]
        public void PeopleAppendTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleAppendTest(DistinctId, GetPeopleAppendDictionary());
            CheckPeopleAppendDictionary(msg.Data);
        }

        [Test]
        public void PeopleAppendTest_DictionaryWithDistinctId_CorrectValuesReturned()
        {
            var msg = _client.PeopleAppendTest(GetPeopleAppendDictionary(includeDistinctId: true));
            CheckPeopleAppendDictionary(msg.Data);
        }

        private Dictionary<string, object> GetPeopleAppendDictionary(bool includeDistinctId = false)
        {
            var dic = new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue}
            };

            IncludeDistinctIdIfNeeded(includeDistinctId, dic);

            return dic;
        }

        private void CheckPeopleAppend()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            var append = (JObject)msg[MixpanelProperty.PeopleAppend];
            Assert.That(append.Count, Is.EqualTo(2));
            Assert.That(append[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
            Assert.That(append[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
        }

        private void CheckPeopleAppendDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleAppend], Is.TypeOf<Dictionary<string, object>>());
            var append = (Dictionary<string, object>)dic[MixpanelProperty.PeopleAppend];
            Assert.That(append.Count, Is.EqualTo(2));
            Assert.That(append[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
            Assert.That(append[StringPropertyName], Is.EqualTo(StringPropertyValue));
        }

        #endregion

        #region PeopleUnion

        [Test]
        public void PeopleUnion_Dictionary_CorrectDataSent()
        {
            _client.PeopleUnion(DistinctId, GetPeopleUnionDictionary());
            CheckPeopleUnion();
        }

        [Test]
        public void PeopleUnion_DictionaryWithDistinctId_CorrectDataSent()
        {
            _client.PeopleUnion(GetPeopleUnionDictionary(includeDistinctId: true));
            CheckPeopleUnion();
        }

        [Test]
        public void PeopleUnion_DictionaryWithDistinctIdAndInvalidData_CorrectDataSent()
        {
            _client.PeopleUnion(GetPeopleUnionDictionary(includeDistinctId: true, includeInvalidData: true));
            CheckPeopleUnion();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleUnionAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleUnionAsync(DistinctId, GetPeopleUnionDictionary());
            CheckPeopleUnion();
        }

        [Test]
        public async void PeopleUnionAsync_DictionaryWithDistinctId_CorrectDataSent()
        {
            await _client.PeopleUnionAsync(GetPeopleUnionDictionary(includeDistinctId: true));
            CheckPeopleUnion();
        }

        [Test]
        public async void PeopleUnionAsync_DictionaryWithDistinctIdAndInvalidData_CorrectDataSent()
        {
            await _client.PeopleUnionAsync(
                GetPeopleUnionDictionary(includeDistinctId: true, includeInvalidData: true));
            CheckPeopleUnion();
        }
#endif

        [Test]
        public void GetPeopleUnionMessage_Dictionary_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleUnionMessage(DistinctId, GetPeopleUnionDictionary());
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnion));
            CheckPeopleUnionDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleUnionMessage_DictionaryWithDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleUnionMessage(GetPeopleUnionDictionary(includeDistinctId: true));
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnion));
            CheckPeopleUnionDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleUnionMessage_DictionaryWithDistinctIdAndInvalidData_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleUnionMessage(
                GetPeopleUnionDictionary(includeDistinctId: true, includeInvalidData: true));
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnion));
            CheckPeopleUnionDictionary(msg.Data);
        }

        [Test]
        public void PeopleUnionTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnionTest(DistinctId, GetPeopleUnionDictionary());
            CheckPeopleUnionDictionary(msg.Data);
        }

        [Test]
        public void PeopleUnionTest_DictionaryWithDistinctId_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnionTest(GetPeopleUnionDictionary(includeDistinctId: true));
            CheckPeopleUnionDictionary(msg.Data);
        }

        [Test]
        public void PeopleUnionTest_DictionaryWithDistinctIdAndInvalidData_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnionTest(
                GetPeopleUnionDictionary(includeDistinctId: true, includeInvalidData: true));
            CheckPeopleUnionDictionary(msg.Data);
        }

        private IDictionary<string, object> GetPeopleUnionDictionary(
            bool includeDistinctId = false, bool includeInvalidData = false)
        {
            var dic = new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyArray},
                {StringPropertyName, StringPropertyArray}
            };

            IncludeDistinctIdIfNeeded(includeDistinctId, dic);

            if (includeInvalidData)
            {
                dic.Add(IntPropertyName, IntPropertyValue);
                dic.Add(DoublePropertyName, DoublePropertyValue);
            }
            return dic;
        }

        private void CheckPeopleUnion()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            var union = (JObject)msg[MixpanelProperty.PeopleUnion];
            Assert.That(union.Count, Is.EqualTo(2));
            Assert.That(
                union[DecimalPropertyName].Value<JArray>().ToObject<decimal[]>(),
                Is.EquivalentTo(DecimalPropertyArray));
            Assert.That(
                union[StringPropertyName].Value<JArray>().ToObject<string[]>(),
                Is.EquivalentTo(StringPropertyArray));
        }

        private static void CheckPeopleUnionDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleUnion], Is.TypeOf<Dictionary<string, object>>());
            var union = (Dictionary<string, object>)dic[MixpanelProperty.PeopleUnion];
            Assert.That(union.Count, Is.EqualTo(2));
            Assert.That(union[DecimalPropertyName], Is.EquivalentTo(DecimalPropertyArray));
            Assert.That(union[StringPropertyName], Is.EquivalentTo(StringPropertyArray));
        }

        #endregion PeopleUnion

        #region PeopleUnset

        [Test]
        public void PeopleUnset_ValidData_CorrectDataSent()
        {
            _client.PeopleUnset(DistinctId, StringPropertyArray);
            CheckPeopleUnset();
        }

        [Test]
        public void PeopleUnset_ValidDataWithDistinctIdFromSuperProperties_CorrectDataSent()
        {
            _client.SetSuperProperties(new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId}
            });

            _client.PeopleUnset(StringPropertyArray);
            CheckPeopleUnset();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleUnsetAsync_ValidData_CorrectDataSent()
        {
            await _client.PeopleUnsetAsync(DistinctId, StringPropertyArray);
            CheckPeopleUnset();
        }

        [Test]
        public async void PeopleUnsetAsync_ValidDataWithDistinctIdFromSuperProperties_CorrectDataSent()
        {
            _client.SetSuperProperties(new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId}
            });

            await _client.PeopleUnsetAsync(StringPropertyArray);
            CheckPeopleUnset();
        }
#endif

        [Test]
        public void GetPeopleUnsetMessage_ValidData_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleUnsetMessage(DistinctId, StringPropertyArray);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnset));
            CheckPeopleUnsetDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleUnsetMessage_ValidDataWithDistinctIdFromSuperProperties_CorrectMessageReturned()
        {
            _client.SetSuperProperties(new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId}
            });

            var msg = _client.GetPeopleUnsetMessage(StringPropertyArray);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnset));
            CheckPeopleUnsetDictionary(msg.Data);
        }

        [Test]
        public void PeopleUnsetTest_ValidData_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnsetTest(DistinctId, StringPropertyArray);
            CheckPeopleUnsetDictionary(msg.Data);
        }

        [Test]
        public void PeopleUnsetTest_ValidDataWithDistinctIdFromSuperProperties_CorrectValuesReturned()
        {
            _client.SetSuperProperties(new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId}
            });

            var msg = _client.PeopleUnsetTest(StringPropertyArray);
            CheckPeopleUnsetDictionary(msg.Data);
        }

        private void CheckPeopleUnset()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            var unset = msg[MixpanelProperty.PeopleUnset].Value<JArray>().ToObject<string[]>();
            Assert.That(unset.Length, Is.EqualTo(StringPropertyArray.Length));
            for (int i = 0; i < StringPropertyArray.Length; i++)
            {
                Assert.That(unset[i], Is.TypeOf<string>());
                Assert.That(unset[i], Is.EqualTo(StringPropertyArray[i]));
            }
        }

        private static void CheckPeopleUnsetDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleUnset], Is.TypeOf<List<object>>());
            var unset = (List<object>)dic[MixpanelProperty.PeopleUnset];
            Assert.That(unset.Count, Is.EqualTo(StringPropertyArray.Length));
            for (int i = 0; i < StringPropertyArray.Length; i++)
            {
                Assert.That(unset[i], Is.TypeOf<string>());
                Assert.That(unset[i], Is.EqualTo(StringPropertyArray[i]));
            }
        }

        #endregion PeopleUnset

        #region PeopleDelete

        [Test]
        public void PeopleDelete_ValidData_CorrectDataSent()
        {
            var res = _client.PeopleDelete(DistinctId);

            Assert.That(res, Is.EqualTo(true));
            CheckPeopleDelete();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleDeleteAsync_ValidData_CorrectDataSent()
        {
            var res = await _client.PeopleDeleteAsync(DistinctId);

            Assert.That(res, Is.EqualTo(true));
            CheckPeopleDelete();
        }
#endif

        [Test]
        public void GetPeopleDeleteMessage_ValidData_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleDeleteMessage(DistinctId);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleDelete));
            CheckPeopleDeleteDictionary(msg.Data);
        }

        [Test]
        public void PeopleDeleteTest_ValidData_CorrectValuesReturned()
        {
            var res = _client.PeopleDeleteTest(DistinctId);
            CheckPeopleDeleteDictionary(res.Data);
        }

        private void CheckPeopleDelete()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(msg[MixpanelProperty.PeopleDelete].Value<string>(), Is.Empty);
        }

        private static void CheckPeopleDeleteDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleDelete], Is.Empty);
        }

        #endregion PeopleDelete

        #region PeopleTrackCharge

        [Test]
        public void PeopleTrackCharge_NoTime_CorrectDataSent()
        {
            _client.UtcNow = () => Time;
            var res = _client.PeopleTrackCharge(DistinctId, DecimalPropertyValue);

            Assert.That(res, Is.EqualTo(true));
            CheckPeopleTrackCharge();
        }

        [Test]
        public void PeopleTrackCharge_WithTime_CorrectDataSent()
        {
            var res = _client.PeopleTrackCharge(DistinctId, DecimalPropertyValue, Time);

            Assert.That(res, Is.EqualTo(true));
            CheckPeopleTrackCharge();
        }

#if !(NET40 || NET35)
        [Test]
        public async void PeopleTrackChargeAsync_NoTime_CorrectDataSent()
        {
            _client.UtcNow = () => Time;
            var res = await _client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue);

            Assert.That(res, Is.EqualTo(true));
            CheckPeopleTrackCharge();
        }

        [Test]
        public async void PeopleTrackChargeAsync_WithTime_CorrectDataSent()
        {
            var res = await _client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue, Time);

            Assert.That(res, Is.EqualTo(true));
            CheckPeopleTrackCharge();
        }
#endif

        [Test]
        public void PeopleTrackChargeTest_NoTime_CorrectMessageReturned()
        {
            _client.UtcNow = () => Time;
            var msg = _client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue);

            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleTrackCharge));
            CheckPeopleTrackChargeDictionary(msg.Data);
        }

        [Test]
        public void PeopleTrackChargeTest_WithTime_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue, Time);

            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleTrackCharge));
            CheckPeopleTrackChargeDictionary(msg.Data);
        }

        [Test]
        public void PeopleTrackChargeTest_NoTime_CorrectValuesReturned()
        {
            _client.UtcNow = () => Time;
            var msg = _client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue);

            CheckPeopleTrackChargeDictionary(msg.Data);
        }

        [Test]
        public void PeopleTrackChargeTest_WithTime_CorrectValuesReturned()
        {
            var msg = _client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue, Time);

            CheckPeopleTrackChargeDictionary(msg.Data);
        }

        private void CheckPeopleTrackCharge()
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            var append = (JObject)msg[MixpanelProperty.PeopleAppend];
            Assert.That(append.Count, Is.EqualTo(1));
            var transactions = (JObject)append[MixpanelProperty.PeopleTransactions];
            Assert.That(transactions.Count, Is.EqualTo(2));
            Assert.That(transactions[MixpanelProperty.PeopleTime].Value<string>(), Is.EqualTo(TimeFormat));
            Assert.That(transactions[MixpanelProperty.PeopleAmount].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private void CheckPeopleTrackChargeDictionary(IDictionary<string, object> dic)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(dic[MixpanelProperty.PeopleAppend], Is.TypeOf<Dictionary<string, object>>());
            var append = (Dictionary<string, object>)dic[MixpanelProperty.PeopleAppend];
            Assert.That(append.Count, Is.EqualTo(1));
            Assert.That(append[MixpanelProperty.PeopleTransactions], Is.TypeOf<Dictionary<string, object>>());
            var transactions = (Dictionary<string, object>)append[MixpanelProperty.PeopleTransactions];
            Assert.That(transactions.Count, Is.EqualTo(2));
            Assert.That(transactions[MixpanelProperty.PeopleTime], Is.EqualTo(TimeFormat));
            Assert.That(transactions[MixpanelProperty.PeopleAmount], Is.EqualTo(DecimalPropertyValue));
        }

        #endregion PeopleTrackCharge

        #region Send

        [TestCase(5, 7)]
        [TestCase(49, 51)]
        [TestCase(50, 50)]
        [TestCase(51, 49)]
        [TestCase(51, 49)]
        [TestCase(0, 5)]
        [TestCase(0, 75)]
        [TestCase(5, 0)]
        [TestCase(75, 0)]
        [TestCase(125, 200)]
        public void Send_DifferentVariantsAsEnumerable_CorrectDataSent(
            int trackMessagesCount, int engageMessagesCount)
        {
            bool res = _client.Send(GetSendMessages(trackMessagesCount, engageMessagesCount));

            Assert.That(res, Is.EqualTo(true));
            CheckSend(trackMessagesCount, engageMessagesCount);
        }

        [Test]
        public void Send_AsParams_CorrectDataSent()
        {
            var messages = GetSendMessages(2, 2);
            bool res = _client.Send(messages[0], messages[1], messages[2], messages[3]);

            Assert.That(res, Is.EqualTo(true));
            CheckSend(2, 2);
        }

#if !(NET40 || NET35)
        [TestCase(5, 7)]
        [TestCase(49, 51)]
        [TestCase(50, 50)]
        [TestCase(51, 49)]
        [TestCase(51, 49)]
        [TestCase(0, 5)]
        [TestCase(0, 75)]
        [TestCase(5, 0)]
        [TestCase(75, 0)]
        [TestCase(125, 200)]
        public async void SendAsync_DifferentVariantsAsEnumerable_CorrectDataSent(
            int trackMessagesCount, int engageMessagesCount)
        {
            bool res = await _client.SendAsync(GetSendMessages(trackMessagesCount, engageMessagesCount));

            Assert.That(res, Is.EqualTo(true));
            CheckSend(trackMessagesCount, engageMessagesCount);
        }

        [Test]
        public async void SendAsync_AsParams_CorrectDataSent()
        {
            var messages = GetSendMessages(2, 2);
            bool res = await _client.SendAsync(messages[0], messages[1], messages[2], messages[3]);

            Assert.That(res, Is.EqualTo(true));
            CheckSend(2, 2);
        }
#endif

        private IList<MixpanelMessage> GetSendMessages(
            int trackMessagesCount = 0, int engageMessagesCount = 0)
        {
            var messages = new List<MixpanelMessage>(trackMessagesCount + engageMessagesCount);
            for (int i = 0; i < trackMessagesCount; i++)
            {
                messages.Add(
                    _client.GetTrackMessage(Event, DistinctId, GetTrackDictionary()));
            }

            for (int i = 0; i < engageMessagesCount; i++)
            {
                messages.Add(
                    _client.GetPeopleSetMessage(DistinctId, GetPeopleSetDictionary()));
            }

            return messages;
        }

        private void CheckSend(int trackMessagesCount, int engageMessagesCount)
        {
            // Track
            var trackSplits = GetSplits(trackMessagesCount, BatchMessageWrapper.MaxBatchSize);
            var trackHttpPostEntries = _httpPostEntries.Where(x => x.Endpoint == TrackUrl).ToList();
            Assert.That(trackHttpPostEntries.Count, Is.EqualTo(trackSplits.Count));
            for (int i = 0; i < trackHttpPostEntries.Count; i++)
            {
                JArray msg = ParseBatchMessageData(trackHttpPostEntries[i].Data);
                Assert.That(msg.Count, Is.EqualTo(trackSplits[i]));
                foreach (JObject msgPart in msg.Cast<JObject>())
                {
                    CheckTrackJsonMessage(msgPart);
                }
            }
            
            // Engage
            var engageSplits = GetSplits(engageMessagesCount, BatchMessageWrapper.MaxBatchSize);
            var engageHttpPostEntries = _httpPostEntries.Where(x => x.Endpoint == EngageUrl).ToList();
            Assert.That(engageHttpPostEntries.Count, Is.EqualTo(engageSplits.Count));
            for (int i = 0; i < engageHttpPostEntries.Count; i++)
            {
                JArray msg = ParseBatchMessageData(engageHttpPostEntries[i].Data);
                Assert.That(msg.Count, Is.EqualTo(engageSplits[i]));
                foreach (JObject msgPart in msg.Cast<JObject>())
                {
                    CheckPeopleSetJsonMessage(msgPart);
                }
            }

            Assert.That(_httpPostEntries.Count, Is.EqualTo(trackSplits.Count + engageSplits.Count));
        }

        #endregion Send

        #region SuperProperties

        [Test]
        public void SuperProperties_ValidValues_AddedToMessage()
        {
            _client.SetSuperProperties(new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId},
                {DoublePropertyName, DoublePropertyValue},
                {StringPropertyName, StringPropertyValue}
            });

            var msg = _client.TrackTest(Event, new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue}
            });

            Assert.That(msg.Data.Count, Is.EqualTo(2));
            Assert.That(msg.Data[MixpanelProperty.TrackEvent], Is.EqualTo(Event));
            Assert.That(msg.Data[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)msg.Data[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(5));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(DistinctId));
            Assert.That(props[DoublePropertyName], Is.EqualTo(DoublePropertyValue));
            Assert.That(props[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        [Test]
        public void SuperProperties_InvalidValues_Ignored()
        {
            _client.SetSuperProperties(new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId},
                {InvalidPropertyName, InvalidPropertyValue},
                {InvalidPropertyName2, InvalidPropertyValue2}
            });

            var msg = _client.TrackTest(Event, new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue}
            });

            Assert.That(msg.Data.Count, Is.EqualTo(2));
            Assert.That(msg.Data[MixpanelProperty.TrackEvent], Is.EqualTo(Event));
            Assert.That(msg.Data[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)msg.Data[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(DistinctId));
            Assert.That(props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        [Test]
        public void SuperProperties_FromClientConstructor_AddedToMessage()
        {
            var client = new MixpanelClient(Token, null, new Dictionary<string, object>
            {
                {MixpanelProperty.DistinctId, DistinctId},
                {DoublePropertyName, DoublePropertyValue},
                {StringPropertyName, StringPropertyValue}
            });

            var msg = client.PeopleSetTest(new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue}
            });

            Assert.That(msg.Data.Count, Is.EqualTo(3));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleSet], Is.TypeOf<Dictionary<string, object>>());
            var set = (Dictionary<string, object>)msg.Data[MixpanelProperty.PeopleSet];
            Assert.That(set.Count, Is.EqualTo(3));
            Assert.That(set[DoublePropertyName], Is.EqualTo(DoublePropertyValue));
            Assert.That(set[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(set[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        #endregion SuperProperties

        private JObject ParseMessageData(string data)
        {
            // Can't use JObject.Parse because it's not possible to disable DateTime parsing
            using (var stringReader = new StringReader(GetJsonFromData(data)))
            {
                using (JsonReader jsonReader = new JsonTextReader(stringReader))
                {
                    jsonReader.DateParseHandling = DateParseHandling.None;
                    JObject msg = JObject.Load(jsonReader);
                    return msg;
                }
            }
        }

        private JArray ParseBatchMessageData(string data)
        {
            // Can't use JArray.Parse because it's not possible to disable DateTime parsing
            using (var stringReader = new StringReader(GetJsonFromData(data)))
            {
                using (JsonReader jsonReader = new JsonTextReader(stringReader))
                {
                    jsonReader.DateParseHandling = DateParseHandling.None;
                    JArray msg = JArray.Load(jsonReader);
                    return msg;
                }
            }
        }

        private string GetJsonFromData(string data)
        {
            // Remove "data=" to get raw BASE64
            var base64 = data.Remove(0, 5);

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            return json;
        }

        private string ToDateTimeString(JToken jToken)
        {
            return jToken.Value<DateTime>().ToString(ValueParser.MixpanelDateFormat);
        }

        private static void IncludeDistinctIdIfNeeded(bool includeDistinctId, Dictionary<string, object> dic)
        {
            if (includeDistinctId)
            {
                dic.Add(MixpanelProperty.DistinctId, DistinctId);
            }
        }
    }
}