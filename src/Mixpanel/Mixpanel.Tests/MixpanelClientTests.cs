using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Mixpanel.Exceptions;
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
            MixpanelConfig.Global.Reset();

            _httpPostEntries = new List<HttpPostEntry>();

            string testName = TestContext.CurrentContext.Test.Name;
            bool setSuperPropertiesWithDistinctId = testName.Contains("SuperPropsDistinctId");
            bool setSuperProperties = testName.Contains("SuperProps");

            if (setSuperPropertiesWithDistinctId)
            {
                _client = new MixpanelClient(
                    Token, GetConfig(), GetSuperPropertiesDictionary(includeDistinctId: true));
            }
            else if (setSuperProperties)
            {
                _client = new MixpanelClient(
                    Token, GetConfig(), GetSuperPropertiesDictionary());
            }
            else
            {
                _client = new MixpanelClient(Token, GetConfig());
            }

            _client.UtcNow = () => DateTime.UtcNow;
        }

        private MixpanelConfig GetConfig()
        {
            return new MixpanelConfig
            {
                HttpPostFn = (endpoint, data) =>
                {
                    _httpPostEntries.Add(new HttpPostEntry(endpoint, data));
                    return true;
                },
#if ASYNC
                AsyncHttpPostFn = (endpoint, data) =>
                {
                    _httpPostEntries.Add(new HttpPostEntry(endpoint, data));
                    return Task.Run(() => true);
                },
#endif
#if !JSON
                SerializeJsonFn = obj => JsonConvert.SerializeObject(obj)
#endif
            };
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
        public void Track_AnonymousObjectAndSuperProps_CorrectDataSent()
        {
            // All super properties should be included in message
            _client.Track(Event, DistinctId, GetTrackObject());
            CheckTrack(CheckOptions.DistinctIdSet | CheckOptions.SuperPropsSet);
        }

        [Test]
        public void Track_AnonymousObjectAndSuperPropsDistinctId_CorrectDataSent()
        {
            // All super properties should be included in message
            _client.Track(Event, DistinctId, GetTrackObject());
            CheckTrack(
                CheckOptions.DistinctIdSet | CheckOptions.SuperPropsSet | CheckOptions.SuperPropsDistinctIdSet);
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

        [Test]
        public void Track_DictionaryAndSuperProps_CorrectDataSent()
        {
            // All super properties should be included in message
            _client.Track(Event, DistinctId, GetTrackDictionary());
            CheckTrack(CheckOptions.SuperPropsSet);
        }

        [Test]
        public void Track_DictionaryAndSuperPropsDistinctId_CorrectDataSent()
        {
            // All super properties should be included in message
            _client.Track(Event, DistinctId, GetTrackDictionary());
            CheckTrack(
                CheckOptions.DistinctIdSet | CheckOptions.SuperPropsSet | CheckOptions.SuperPropsDistinctIdSet);
        }

#if !(NET35 || NET40)
        [Test]
        public async Task TrackAsync_AnonymousObject_CorrectDataSent()
        {
            await _client.TrackAsync(Event, DistinctId, GetTrackObject());
            CheckTrack();
        }

        [Test]
        public async Task TrackAsync_AnonymousObjectWithDistinctId_CorrectDataSent()
        {
            await _client.TrackAsync(Event, GetTrackObject(includeDistinctId: true));
            CheckTrack();
        }


        [Test]
        public async Task TrackAsync_Dictionary_CorrectDataSent()
        {
            await _client.TrackAsync(Event, DistinctId, GetTrackDictionary());
            CheckTrack();
        }

        [Test]
        public async Task TrackAsync_DictionaryWithDistinctId_CorrectDataSent()
        {
            await _client.TrackAsync(Event, GetTrackDictionary(includeDistinctId: true));
            CheckTrack();
        }

        [Test]
        public async Task TrackAsync_DictionaryAndSuperProps_CorrectDataSent()
        {
            await _client.TrackAsync(Event, DistinctId, GetTrackDictionary());
            CheckTrack(CheckOptions.SuperPropsSet);
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
        public void GetTrackMessage_DictionaryAndSuperProps_CorrectMessageReturned()
        {
            MixpanelMessage msg = _client.GetTrackMessage(Event, DistinctId, GetTrackDictionary());
            CheckTrackMessage(msg, CheckOptions.SuperPropsSet);
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

        [Test]
        public void TrackTest_DictionaryAndSuperProps_CorrectValuesReturned()
        {
            var msg = _client.TrackTest(Event, DistinctId, GetTrackDictionary());
            CheckTrackDictionary(msg.Data, CheckOptions.SuperPropsSet);
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

        private void CheckTrack(CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(TrackUrl));

            JObject msg = ParseMessageData(_httpPostEntries.Single().Data);
            CheckTrackJsonMessage(msg, checkOptions);
        }

        private void CheckTrackJsonMessage(JObject msg, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg[MixpanelProperty.TrackEvent].Value<string>(), Is.EqualTo(Event));
            var props = (JObject)msg[MixpanelProperty.TrackProperties];

            Assert.That(props.Count, Is.EqualTo(GetPropsCount(checkOptions, normalCount: 6, superPropsCount: 8)));
            Assert.That(props[MixpanelProperty.TrackToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId].Value<string>(), Is.EqualTo(GetDistinctId(checkOptions)));
            Assert.That(props[MixpanelProperty.TrackIp].Value<string>(), Is.EqualTo(Ip));
            Assert.That(props[MixpanelProperty.TrackTime].Value<long>(), Is.EqualTo(TimeUnix));
            Assert.That(props[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));

            if (checkOptions.HasFlag(CheckOptions.SuperPropsSet))
            {
                Assert.That(props[DecimalSuperPropertyName].Value<decimal>(), Is.EqualTo(DecimalSuperPropertyValue));
                Assert.That(props[StringSuperPropertyName].Value<string>(), Is.EqualTo(StringSuperPropertyValue));
            }
        }

        private void CheckTrackDictionary(IDictionary<string, object> dic, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(dic.Count, Is.EqualTo(2));
            Assert.That(dic[MixpanelProperty.TrackEvent], Is.EqualTo(Event));
            Assert.That(dic[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)dic[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(GetPropsCount(checkOptions, normalCount: 6, superPropsCount: 8)));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(GetDistinctId(checkOptions)));
            Assert.That(props[MixpanelProperty.TrackIp], Is.EqualTo(Ip));
            Assert.That(props[MixpanelProperty.TrackTime], Is.EqualTo(TimeUnix));
            Assert.That(props[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));

            if (checkOptions.HasFlag(CheckOptions.SuperPropsSet))
            {
                Assert.That(props[DecimalSuperPropertyName], Is.EqualTo(DecimalSuperPropertyValue));
                Assert.That(props[StringSuperPropertyName], Is.EqualTo(StringSuperPropertyValue));
            }
        }

        private void CheckTrackMessage(MixpanelMessage msg, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.Track));
            CheckTrackDictionary(msg.Data, checkOptions);
        }

        #endregion Track

        #region Alias

        [Test]
        public void Alias_SuperPropsDistinctId_CorrectDataSent()
        {
            _client.Alias(Alias);
            CheckAlias(CheckOptions.SuperPropsSet | CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void Alias_DistinctIdNoSet_NoDataSent()
        {
            EnsureNotSent(() => _client.Alias(Alias));
        }

        [Test]
        public void Alias_ParamDistinctId_CorrectDataSent()
        {
            _client.Alias(DistinctId, Alias);
            CheckAlias(CheckOptions.DistinctIdSet);
        }

        [Test]
        public void Alias_ParamDistinctIdAndSuperProps_CorrectDataSent()
        {
            // Super properties should be ignored
            _client.Alias(DistinctId, Alias);
            CheckAlias(CheckOptions.SuperPropsSet | CheckOptions.DistinctIdSet);
        }

        [Test]
        public void Alias_ParamDistinctIdAndSuperPropsDistinctId_CorrectDataSent()
        {
            // DistinctId from super props should be overwritten with params value
            _client.Alias(DistinctId, Alias);
            CheckAlias(CheckOptions.DistinctIdSet | CheckOptions.SuperPropsDistinctIdSet);
        }

#if ASYNC
        [Test]
        public async Task AliasAsync_SuperPropsDistinctId_CorrectDataSent()
        {
            await _client.AliasAsync(Alias);
            CheckAlias(CheckOptions.SuperPropsSet | CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void AliasAsync_DistinctIdNoSet_NoDataSent()
        {
            EnsureNotSent(async () => await _client.AliasAsync(Alias));
        }

        [Test]
        public async Task AliasAsync_ParamDistinctId_CorrectDataSent()
        {
            await _client.AliasAsync(DistinctId, Alias);
            CheckAlias();
        }

        [Test]
        public async Task AliasAsync_ParamDistinctIdAndSuperProps_CorrectDataSent()
        {
            // Super properties should be ignored
            await _client.AliasAsync(DistinctId, Alias);
            CheckAlias(CheckOptions.SuperPropsSet | CheckOptions.DistinctIdSet);
        }
#endif

        [Test]
        public void GetAliasMessage_ParamDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetAliasMessage(DistinctId, Alias);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.Alias));
            CheckAliasDictionary(msg.Data);
        }

        [Test]
        public void GetAliasMessage_SuperPropsDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetAliasMessage(Alias);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.Alias));
            CheckAliasDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void GetAliasMessage_NoDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetAliasMessage(Alias);
            Assert.That(msg, Is.Null);
        }

        [Test]
        public void AliasTest_ParamDistinctId_CorrectValuesReturned()
        {
            var msg = _client.AliasTest(DistinctId, Alias);
            CheckAliasDictionary(msg.Data);
        }

        [Test]
        public void AliasTest_SuperPropsDistinctId_CorrectValuesReturned()
        {
            var msg = _client.AliasTest(Alias);
            CheckAliasDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        private void CheckAlias(CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(TrackUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg[MixpanelProperty.TrackEvent].Value<string>(), Is.EqualTo(MixpanelProperty.TrackCreateAlias));
            var props = (JObject)msg[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(
                props[MixpanelProperty.TrackDistinctId].Value<string>(),
                Is.EqualTo(GetDistinctId(checkOptions)));
            Assert.That(props[MixpanelProperty.TrackAlias].Value<string>(), Is.EqualTo(Alias));
        }

        private void CheckAliasDictionary(IDictionary<string, object> dic, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(dic.Count, Is.EqualTo(2));
            Assert.That(dic[MixpanelProperty.TrackEvent], Is.EqualTo(MixpanelProperty.TrackCreateAlias));
            Assert.That(dic[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)dic[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(GetDistinctId(checkOptions)));
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
        public void PeopleSet_IpOnly_CorrectDataSent()
        {
            _client.PeopleSet(DistinctId, new Dictionary<string, object>
            {
                {MixpanelProperty.Ip, Ip}
            });
            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg[MixpanelProperty.PeopleIp].Value<string>(), Is.EqualTo(Ip));
        }

        [Test]
        public void PeopleSet_DictionaryWithDistinctId_CorrectDataSent()
        {
            _client.PeopleSet(GetPeopleSetDictionary(includeDistinctId: true));
            CheckPeopleSet();
        }

        [Test]
        public void PeopleSet_DictionaryAndSuperProps_CorrectDataSent()
        {
            // Super properties should be ignored
            _client.PeopleSet(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSet();
        }

        [Test]
        public void PeopleSet_DictionaryAndSuperPropsDistinctId_CorrectDataSent()
        {
            // DistinctId from super props should be overwritten with value from params;
            _client.PeopleSet(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSet(
                CheckOptions.DistinctIdSet | CheckOptions.SuperPropsSet | CheckOptions.SuperPropsDistinctIdSet);
        }

#if ASYNC
        [Test]
        public async Task PeopleSetAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleSetAsync(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSet();
        }

        [Test]
        public async Task PeopleSetAsync_DictionaryWithDistinctId_CorrectDataSent()
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

        private void CheckPeopleSet(CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            CheckPeopleSetJsonMessage(msg, checkOptions);
        }

        private void CheckPeopleSetJsonMessage(JObject msg, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(msg.Count, Is.EqualTo(6));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(
                msg[MixpanelProperty.PeopleDistinctId].Value<string>(),
                Is.EqualTo(GetDistinctId(checkOptions)));
            Assert.That(msg[MixpanelProperty.PeopleIp].Value<string>(), Is.EqualTo(Ip));
            Assert.That(msg[MixpanelProperty.PeopleTime].Value<long>(), Is.EqualTo(TimeUnix));
            Assert.That(msg[MixpanelProperty.PeopleIgnoreTime].Value<bool>(), Is.EqualTo(IgnoreTime));
            var set = (JObject)msg[MixpanelProperty.PeopleSet];
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

#if ASYNC
        [Test]
        public async Task PeopleSetOnceAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleSetOnceAsync(DistinctId, GetPeopleSetOnceDictionary());
            CheckPeopleSetOnce();
        }

        [Test]
        public async Task PeopleSetOnceAsync_DictionaryWithDistinctId_CorrectDataSent()
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

#if ASYNC
        [Test]
        public async Task PeopleAddAsync_NumericInput_CorrectDataSent()
        {
            await _client.PeopleAddAsync(DistinctId, GetPeopleAddDictionary());
            CheckPeopleAdd();
        }

        [Test]
        public async Task PeopleAddAsync_NumericInputWithDistinctId_CorrectDataSent()
        {
            await _client.PeopleAddAsync(GetPeopleAddDictionary(includeDistinctId: true));
            CheckPeopleAdd();
        }

        [Test]
        public async Task PeopleAddAsync_MixedInput_CorrectDataSent()
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

#if ASYNC
        [Test]
        public async Task PeopleAppendAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleAppendAsync(DistinctId, GetPeopleAppendDictionary());
            CheckPeopleAppend();
        }

        [Test]
        public async Task PeopleAppendAsync_DictionaryWithDistinctId_CorrectDataSent()
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

#if ASYNC
        [Test]
        public async Task PeopleUnionAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleUnionAsync(DistinctId, GetPeopleUnionDictionary());
            CheckPeopleUnion();
        }

        [Test]
        public async Task PeopleUnionAsync_DictionaryWithDistinctId_CorrectDataSent()
        {
            await _client.PeopleUnionAsync(GetPeopleUnionDictionary(includeDistinctId: true));
            CheckPeopleUnion();
        }

        [Test]
        public async Task PeopleUnionAsync_DictionaryWithDistinctIdAndInvalidData_CorrectDataSent()
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
        public void PeopleUnset_SuperPropsDistinctId_CorrectDataSent()
        {
            _client.PeopleUnset(StringPropertyArray);
            CheckPeopleUnset(CheckOptions.SuperPropsDistinctIdSet);
        }

#if ASYNC
        [Test]
        public async Task PeopleUnsetAsync_ValidData_CorrectDataSent()
        {
            await _client.PeopleUnsetAsync(DistinctId, StringPropertyArray);
            CheckPeopleUnset();
        }

        [Test]
        public async Task PeopleUnsetAsync_SuperPropsDistinctId_CorrectDataSent()
        {
            await _client.PeopleUnsetAsync(StringPropertyArray);
            CheckPeopleUnset(CheckOptions.SuperPropsDistinctIdSet);
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
        public void GetPeopleUnsetMessage_SuperPropsDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleUnsetMessage(StringPropertyArray);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleUnset));
            CheckPeopleUnsetDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleUnsetTest_ValidData_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnsetTest(DistinctId, StringPropertyArray);
            CheckPeopleUnsetDictionary(msg.Data);
        }

        [Test]
        public void PeopleUnsetTest_SuperPropsDistinctId_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnsetTest(StringPropertyArray);
            CheckPeopleUnsetDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        private void CheckPeopleUnset(CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(
                msg[MixpanelProperty.PeopleDistinctId].Value<string>(),
                Is.EqualTo(GetDistinctId(checkOptions)));
            var unset = msg[MixpanelProperty.PeopleUnset].Value<JArray>().ToObject<string[]>();
            Assert.That(unset.Length, Is.EqualTo(StringPropertyArray.Length));
            for (int i = 0; i < StringPropertyArray.Length; i++)
            {
                Assert.That(unset[i], Is.TypeOf<string>());
                Assert.That(unset[i], Is.EqualTo(StringPropertyArray[i]));
            }
        }

        private void CheckPeopleUnsetDictionary(IDictionary<string, object> dic, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(dic[MixpanelProperty.PeopleDistinctId], Is.EqualTo(GetDistinctId(checkOptions)));
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
        public void PeopleDelete_ParamDistinctId_CorrectDataSent()
        {
            var res = _client.PeopleDelete(DistinctId);

            Assert.That(res, Is.True);
            CheckPeopleDelete(CheckOptions.DistinctIdSet);
        }

        [Test]
        public void PeopleDelete_SuperPropsDistinctId_CorrectDataSent()
        {
            var res = _client.PeopleDelete();

            Assert.That(res, Is.True);
            CheckPeopleDelete(CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleDelete_ParamDistinctIdAndSuperPropsDistinctId_CorrectDataSent()
        {
            var res = _client.PeopleDelete(DistinctId);

            Assert.That(res, Is.True);
            CheckPeopleDelete(CheckOptions.DistinctIdSet | CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleDelete_NoDistinctId_NoDataSent()
        {
            EnsureNotSent(() => _client.PeopleDelete());
        }

#if ASYNC
        [Test]
        public async Task PeopleDeleteAsync_ParamDistinctId_CorrectDataSent()
        {
            var res = await _client.PeopleDeleteAsync(DistinctId);

            Assert.That(res, Is.True);
            CheckPeopleDelete();
        }

        [Test]
        public async Task PeopleDeleteAsync_SuperPropsDistinctId_CorrectDataSent()
        {
            var res = await _client.PeopleDeleteAsync();

            Assert.That(res, Is.True);
            CheckPeopleDelete(CheckOptions.SuperPropsDistinctIdSet);
        }
#endif

        [Test]
        public void GetPeopleDeleteMessage_ParamDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleDeleteMessage(DistinctId);
            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleDelete));
            CheckPeopleDeleteDictionary(msg.Data);
        }

        [Test]
        public void GetPeopleDeleteMessage_SuperPropsDistinctId_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleDeleteMessage();

            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleDelete));
            CheckPeopleDeleteDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleDeleteTest_ParamDistinctId_CorrectValuesReturned()
        {
            var res = _client.PeopleDeleteTest(DistinctId);
            CheckPeopleDeleteDictionary(res.Data);
        }

        [Test]
        public void PeopleDeleteTest_SuperPropsDistinctId_CorrectValuesReturned()
        {
            var res = _client.PeopleDeleteTest();
            CheckPeopleDeleteDictionary(res.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        private void CheckPeopleDelete(CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(
                msg[MixpanelProperty.PeopleDistinctId].Value<string>(),
                Is.EqualTo(GetDistinctId(checkOptions)));
            Assert.That(msg[MixpanelProperty.PeopleDelete].Value<string>(), Is.Empty);
        }

        private void CheckPeopleDeleteDictionary(
            IDictionary<string, object> dic, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(
                dic[MixpanelProperty.PeopleDistinctId],
                Is.EqualTo(GetDistinctId(checkOptions)));
            Assert.That(dic[MixpanelProperty.PeopleDelete], Is.Empty);
        }

        #endregion PeopleDelete

        #region PeopleTrackCharge

        [Test]
        public void PeopleTrackCharge_ParamDistinctIdAndNoTime_CorrectDataSent()
        {
            _client.UtcNow = () => Time;
            var res = _client.PeopleTrackCharge(DistinctId, DecimalPropertyValue);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge();
        }

        [Test]
        public void PeopleTrackCharge_SuperPropsDistinctIdAndNoTime_CorrectDataSent()
        {
            _client.UtcNow = () => Time;
            var res = _client.PeopleTrackCharge(DecimalPropertyValue);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleTrackCharge_ParamDistinctIdAndSuperPropsDistinctIdAndNoTime_CorrectDataSent()
        {
            _client.UtcNow = () => Time;
            var res = _client.PeopleTrackCharge(DistinctId, DecimalPropertyValue);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.DistinctIdSet | CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleTrackCharge_NoDistinctIdAndNoTime_NoDataSent()
        {
            EnsureNotSent(() => _client.PeopleTrackCharge(DecimalPropertyValue));
        }

        [Test]
        public void PeopleTrackCharge_ParamDistinctIdAndTime_CorrectDataSent()
        {
            var res = _client.PeopleTrackCharge(DistinctId, DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.DistinctIdSet);
        }

        [Test]
        public void PeopleTrackCharge_SuperPropsDistinctIdAndTime_CorrectDataSent()
        {
            var res = _client.PeopleTrackCharge(DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.SuperPropsDistinctIdSet);
        }

#if ASYNC
        [Test]
        public async Task PeopleTrackChargeAsync_ParamDistinctIdNoTime_CorrectDataSent()
        {
            _client.UtcNow = () => Time;
            var res = await _client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.DistinctIdSet);
        }

        [Test]
        public async Task PeopleTrackChargeAsync_SuperPropsDistinctIdAndNoTime_CorrectDataSent()
        {
            _client.UtcNow = () => Time;
            var res = await _client.PeopleTrackChargeAsync(DecimalPropertyValue);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public async Task PeopleTrackChargeAsync_ParamDistinctIdAndWithTime_CorrectDataSent()
        {
            var res = await _client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.DistinctIdSet);
        }

        [Test]
        public async Task PeopleTrackChargeAsync_SuperPropsDistinctIdAndWithTime_CorrectDataSent()
        {
            var res = await _client.PeopleTrackChargeAsync(DecimalPropertyValue, Time);

            Assert.That(res, Is.True);
            CheckPeopleTrackCharge(CheckOptions.SuperPropsDistinctIdSet);
        }
#endif

        [Test]
        public void PeopleTrackChargeTest_ParamDistinctIdNoTime_CorrectMessageReturned()
        {
            _client.UtcNow = () => Time;
            var msg = _client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue);

            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleTrackCharge));
            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.DistinctIdSet);
        }

        [Test]
        public void PeopleTrackChargeTest_SuperPropsDistinctIdNoTime_CorrectMessageReturned()
        {
            _client.UtcNow = () => Time;
            var msg = _client.GetPeopleTrackChargeMessage(DecimalPropertyValue);

            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleTrackCharge));
            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleTrackChargeTest_ParamDistinctIdWithTime_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleTrackChargeMessage(DistinctId, DecimalPropertyValue, Time);

            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleTrackCharge));
            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.DistinctIdSet);
        }

        [Test]
        public void PeopleTrackChargeTest_SuperPropsDistinctIdWithTime_CorrectMessageReturned()
        {
            var msg = _client.GetPeopleTrackChargeMessage(DecimalPropertyValue, Time);

            Assert.That(msg.Kind, Is.EqualTo(MessageKind.PeopleTrackCharge));
            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleTrackChargeTest_ParamDistinctIdNoTime_CorrectValuesReturned()
        {
            _client.UtcNow = () => Time;
            var msg = _client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue);

            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.DistinctIdSet);
        }

        [Test]
        public void PeopleTrackChargeTest_SuperPropsDistinctIdNoTime_CorrectValuesReturned()
        {
            _client.UtcNow = () => Time;
            var msg = _client.PeopleTrackChargeTest(DecimalPropertyValue);

            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void PeopleTrackChargeTest_ParamDistinctWithTime_CorrectValuesReturned()
        {
            var msg = _client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue, Time);

            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.DistinctIdSet);
        }

        [Test]
        public void PeopleTrackChargeTest_SuperPropsDistinctIdWithTime_CorrectValuesReturned()
        {
            var msg = _client.PeopleTrackChargeTest(DecimalPropertyValue, Time);

            CheckPeopleTrackChargeDictionary(msg.Data, CheckOptions.SuperPropsDistinctIdSet);
        }

        private void CheckPeopleTrackCharge(CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(3));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(
                msg[MixpanelProperty.PeopleDistinctId].Value<string>(),
                Is.EqualTo(GetDistinctId(checkOptions)));
            var append = (JObject)msg[MixpanelProperty.PeopleAppend];
            Assert.That(append.Count, Is.EqualTo(1));
            var transactions = (JObject)append[MixpanelProperty.PeopleTransactions];
            Assert.That(transactions.Count, Is.EqualTo(2));
            Assert.That(transactions[MixpanelProperty.PeopleTime].Value<string>(), Is.EqualTo(TimeFormat));
            Assert.That(transactions[MixpanelProperty.PeopleAmount].Value<decimal>(), Is.EqualTo(DecimalPropertyValue));
        }

        private void CheckPeopleTrackChargeDictionary(
            IDictionary<string, object> dic, CheckOptions checkOptions = CheckOptions.None)
        {
            Assert.That(dic.Count, Is.EqualTo(3));
            Assert.That(dic[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(
                dic[MixpanelProperty.PeopleDistinctId],
                Is.EqualTo(GetDistinctId(checkOptions)));
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
            SendResult res = _client.Send(GetSendMessages(trackMessagesCount, engageMessagesCount));

            Assert.That(res.Success, Is.EqualTo(true));
            CheckSend(trackMessagesCount, engageMessagesCount);
        }

        [Test]
        public void Send_AsParams_CorrectDataSent()
        {
            var messages = GetSendMessages(2, 2);
            SendResult res = _client.Send(messages[0], messages[1], messages[2], messages[3]);

            Assert.That(res.Success, Is.EqualTo(true));
            CheckSend(2, 2);
        }

#if ASYNC
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
        public async Task SendAsync_DifferentVariantsAsEnumerable_CorrectDataSent(
            int trackMessagesCount, int engageMessagesCount)
        {
            SendResult res = await _client.SendAsync(GetSendMessages(trackMessagesCount, engageMessagesCount));

            Assert.That(res.Success, Is.EqualTo(true));
            CheckSend(trackMessagesCount, engageMessagesCount);
        }

        [Test]
        public async Task SendAsync_AsParams_CorrectDataSent()
        {
            var messages = GetSendMessages(2, 2);
            SendResult res = await _client.SendAsync(messages[0], messages[1], messages[2], messages[3]);

            Assert.That(res.Success, Is.EqualTo(true));
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

        #region SendJson

        [Test]
        public void SendJson_CorrectDataSent()
        {
            bool result = _client.SendJson(MixpanelMessageEndpoint.Track, CreateJsonMessage());

            Assert.That(result, Is.True);
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(TrackUrl));
            CheckSendJsonMessage();
        }

        [Test]
        public void SendJson_HttpPostThrowsException_FalseReturned()
        {
            MixpanelConfig.Global.HttpPostFn = (url, data) => { throw new Exception(); };
            var client = new MixpanelClient();

            bool result = client.SendJson(MixpanelMessageEndpoint.Track, CreateJsonMessage());
            Assert.That(result, Is.False);
        }

#if ASYNC

        [Test]
        public async Task SendJsonAsync_CorrectDataSent()
        {
            bool result = await _client.SendJsonAsync(MixpanelMessageEndpoint.Track, CreateJsonMessage());

            Assert.That(result, Is.True);
            Assert.That(_httpPostEntries.Single().Endpoint, Is.EqualTo(TrackUrl));
            CheckSendJsonMessage();
        }

        [Test]
        public async Task SendJsonAsync_HttpPostThrowsException_FalseReturned()
        {
            MixpanelConfig.Global.AsyncHttpPostFn = (url, data) => { throw new Exception(); };
            var client = new MixpanelClient();

            bool result = await client.SendJsonAsync(MixpanelMessageEndpoint.Track, CreateJsonMessage());
            Assert.That(result, Is.False);
        }

#endif

        private string CreateJsonMessage()
        {
            var message = new
            {
                @event = Event,
                properties = new
                {
                    token = Token,
                    distinct_id = DistinctId,
                    StringProperty = StringPropertyValue
                }
            };
            string messageJson = JsonConvert.SerializeObject(message);
            return messageJson;
        }

        private void CheckSendJsonMessage()
        {
            var msg = ParseMessageData(_httpPostEntries.Single().Data);
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg[MixpanelProperty.TrackEvent].Value<string>(), Is.EqualTo(Event));
            var props = (JObject)msg[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(props[StringPropertyName].Value<string>(), Is.EqualTo(StringPropertyValue));
        }

        #endregion

        #region SuperProperties

        [Test]
        public void SuperProperties_InvalidValues_Ignored()
        {
            var superProps = GetSuperPropertiesDictionary(includeDistinctId: true);
            superProps.Add(InvalidPropertyName, InvalidPropertyValue);
            superProps.Add(InvalidPropertyName2, InvalidPropertyValue2);

            var client = new MixpanelClient(Token, GetConfig(), superProps);

            var msg = client.GetTrackMessage(Event, GetTrackDictionary());
            CheckTrackMessage(msg, CheckOptions.SuperPropsSet | CheckOptions.SuperPropsDistinctIdSet);
        }

        [Test]
        public void SuperProperties_FromClientConstructor_AddedToMessage()
        {
            var client = new MixpanelClient(Token, GetConfig(), GetSuperPropertiesDictionary(includeDistinctId: true));

            MixpanelMessage msg = client.GetTrackMessage(Event, GetTrackDictionary());
            CheckTrackMessage(msg, CheckOptions.SuperPropsSet | CheckOptions.SuperPropsDistinctIdSet);
        }

        private IDictionary<string, object> GetSuperPropertiesDictionary(bool includeDistinctId = false)
        {
            var dic = new Dictionary<string, object>
            {
                {DecimalSuperPropertyName, DecimalSuperPropertyValue},
                {StringSuperPropertyName, StringSuperPropertyValue}
            };

            if (includeDistinctId)
            {
                dic.Add(MixpanelProperty.DistinctId, SuperDistinctId);
            }

            return dic;
        }

        #endregion SuperProperties

        #region NET Standard

#if !JSON

        [Test]
        public void Track_JsonSerializerFnNotSet_ThrowsException()
        {
            var config = GetConfig();
            config.SerializeJsonFn = null;
            var mc = new MixpanelClient(Token, config);

            Assert.That(
                () => { mc.Track(Event, DistinctId, GetTrackDictionary()); },
                Throws.TypeOf<MixpanelConfigurationException>());
        }
#endif


        #endregion NET Standard

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
            string base64 = data.Remove(0, 5);
            byte[] bytesString = Convert.FromBase64String(base64);

            var json = Encoding.UTF8.GetString(bytesString, 0, bytesString.Length);
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

        private int GetPropsCount(CheckOptions checkOptions, int normalCount, int superPropsCount)
        {
            if (checkOptions.HasFlag(CheckOptions.SuperPropsSet))
            {
                return 8;
            }
            return 6;
        }

        private string GetDistinctId(CheckOptions checkOptions)
        {
            bool distinctIdSetFromSuperProps =
                checkOptions.HasFlag(CheckOptions.SuperPropsDistinctIdSet) &&
                !checkOptions.HasFlag(CheckOptions.DistinctIdSet);

            if (distinctIdSetFromSuperProps)
            {
                return SuperDistinctId;
            }
            return DistinctId;
        }

#if ASYNC
        private void EnsureNotSent(Func<Task<bool>> fn)
        {
            bool res = fn().Result;
            Assert.That(res, Is.False);
            Assert.That(_httpPostEntries, Is.Empty);
        }
#endif

        private void EnsureNotSent(Func<bool> fn)
        {
            bool res = fn();
            Assert.That(res, Is.False);
            Assert.That(_httpPostEntries, Is.Empty);
        }

        [Flags]
        private enum CheckOptions
        {
            None = 0x0,
            DistinctIdSet = 0x1,
            SuperPropsSet = 0x2,
            SuperPropsDistinctIdSet = 0x4,
        }
    }
}