using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mixpanel.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelClientTests : MixpanelTestsBase
    {
        private MixpanelClient _client;
        private string _endpoint, _data;

        public string TrackUrl
        {
            get
            {
                return string.Format(MixpanelClient.UrlFormat, MixpanelClient.EndpointTrack);
            }
        }
        
        public string EngageUrl
        {
            get
            {
                return string.Format(MixpanelClient.UrlFormat, MixpanelClient.EndpointEngage);
            }
        }

        [SetUp]
        public void SetUp()
        {
            _endpoint = null;
            _data = null;
            _client = new MixpanelClient(Token,
                new MixpanelConfig
                {
                    HttpPostFn = (endpoint, data) =>
                    {
                        _endpoint = endpoint;
                        _data = data;
                        return true;
                    },
                    AsyncHttpPostFn = (endpoint, data) =>
                    {
                        _endpoint = endpoint;
                        _data = data;
                        return Task.Run(() => true);
                    }
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
        public async void TrackAsync_AnonymousObject_CorrectDataSent()
        {
            await _client.TrackAsync(Event, DistinctId, GetTrackObject());
            CheckTrack();
        }

        [Test]
        public void TrackTest_AnonymousObject_CorrectValuesReturned()
        {
            var msg = _client.TrackTest(Event, DistinctId, GetTrackObject());
            CheckTrackTest(msg);
        }

        private object GetTrackObject()
        {
            return new
            {
                Ip,
                Time,
                StringProperty = StringPropertyValue,
                DecimalProperty = DecimalPropertyValue
            };
        }

        private void CheckTrack()
        {
            Assert.That(_endpoint, Is.EqualTo(TrackUrl));

            var msg = ParseMessageData(_data);
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

        private static void CheckTrackTest(MixpanelMessageTest msg)
        {
            Assert.That(msg.Data.Count, Is.EqualTo(2));
            Assert.That(msg.Data[MixpanelProperty.TrackEvent], Is.EqualTo(Event));
            Assert.That(msg.Data[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>) msg.Data[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(6));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(DistinctId));
            Assert.That(props[MixpanelProperty.TrackIp], Is.EqualTo(Ip));
            Assert.That(props[MixpanelProperty.TrackTime], Is.EqualTo(TimeUnix));
            Assert.That(props[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(props[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        #endregion Track

        #region Alias

        [Test]
        public void Alias_ValidValues_CorrectDataSent()
        {
            _client.Alias(DistinctId, Alias);
            CheckAlias();
        } 
        
        [Test]
        public async void AliasAsync_ValidValues_CorrectDataSent()
        {
            await _client.AliasAsync(DistinctId, Alias);
            CheckAlias();
        }
        
        [Test]
        public void AliasTest_ValidValues_CorrectValuesReturned()
        {
            var msg = _client.AliasTest(DistinctId, Alias);
            CheckAliasTest(msg);
        }

        private void CheckAlias()
        {
            Assert.That(_endpoint, Is.EqualTo(TrackUrl));

            var msg = ParseMessageData(_data);
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg[MixpanelProperty.TrackEvent].Value<string>(), Is.EqualTo(MixpanelProperty.TrackCreateAlias));
            var props = (JObject)msg[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId].Value<string>(), Is.EqualTo(DistinctId));
            Assert.That(props[MixpanelProperty.TrackAlias].Value<string>(), Is.EqualTo(Alias));
        } 
        
        private void CheckAliasTest(MixpanelMessageTest msg)
        {
            Assert.That(msg.Data.Count, Is.EqualTo(2));
            Assert.That(msg.Data[MixpanelProperty.TrackEvent], Is.EqualTo(MixpanelProperty.TrackCreateAlias));
            Assert.That(msg.Data[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)msg.Data[MixpanelProperty.TrackProperties];
            Assert.That(props.Count, Is.EqualTo(3));
            Assert.That(props[MixpanelProperty.TrackToken], Is.EqualTo(Token));
            Assert.That(props[MixpanelProperty.TrackDistinctId], Is.EqualTo(DistinctId));
            Assert.That(props[MixpanelProperty.TrackAlias], Is.EqualTo(Alias));
        }

        #endregion Alias

        #region PeopleSet

        [Test]
        public void PeopleSet_AnonymousObject_CorrectDataSent()
        {
            _client.PeopleSet(DistinctId, GetPeopleSetObject());
            CheckPeopleSet();
        }
        
        [Test]
        public async void PeopleSetAsync_AnonymousObject_CorrectDataSent()
        {
            await _client.PeopleSetAsync(DistinctId, GetPeopleSetObject());
            CheckPeopleSet();
        } 
        
        [Test]
        public void PeopleSet_Dictionary_CorrectDataSent()
        {
            _client.PeopleSet(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSet();
        }
        
        [Test]
        public async void PeopleSetAsync_Dictionary_CorrectDataSent()
        {
            await _client.PeopleSetAsync(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSet();
        }
        
        [Test]
        public void PeopleSetTest_AnonymousObject_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetTest(DistinctId, GetPeopleSetObject());
            CheckPeopleSetTest(msg);
        }

        [Test]
        public void PeopleSetTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetTest(DistinctId, GetPeopleSetDictionary());
            CheckPeopleSetTest(msg);
        }

        private object GetPeopleSetObject()
        {
            return new
            {
                Ip,
                Time,
                IgnoreTime,
                FirstName,
                LastName,
                Name,
                Created,
                Email,
                Phone,
                StringProperty = StringPropertyValue,
                DecimalProperty = DecimalPropertyValue
            };
        }

        private Dictionary<string, object> GetPeopleSetDictionary()
        {
            return new Dictionary<string, object>
            {
                { MixpanelProperty.Ip, Ip },
                { MixpanelProperty.Time, Time },
                { MixpanelProperty.IgnoreTime, IgnoreTime },
                { MixpanelProperty.FirstName, FirstName },
                { MixpanelProperty.LastName, LastName },
                { MixpanelProperty.Name, Name },
                { MixpanelProperty.Created, Created },
                { MixpanelProperty.Email, Email },
                { MixpanelProperty.Phone, Phone },
                { StringPropertyName, StringPropertyValue },
                { DecimalPropertyName, DecimalPropertyValue }
            };
        }

        private void CheckPeopleSet()
        {
            Assert.That(_endpoint, Is.EqualTo(EngageUrl));

            var msg = ParseMessageData(_data);
            Assert.That(msg.Count, Is.EqualTo(6));
            Assert.That(msg[MixpanelProperty.PeopleToken].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg[MixpanelProperty.PeopleDistinctId].Value<string>(), Is.EqualTo(DistinctId));
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
        
        private void CheckPeopleSetTest(MixpanelMessageTest msg)
        {
            Assert.That(msg.Data.Count, Is.EqualTo(6));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleIp], Is.EqualTo(Ip));
            Assert.That(msg.Data[MixpanelProperty.PeopleTime], Is.EqualTo(TimeUnix));
            Assert.That(msg.Data[MixpanelProperty.PeopleIgnoreTime], Is.EqualTo(IgnoreTime));
            Assert.That(msg.Data[MixpanelProperty.PeopleSet], Is.TypeOf<Dictionary<string, object>>());
            var set = (Dictionary<string, object>)msg.Data[MixpanelProperty.PeopleSet];
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
        public void PeopleSetOnceTest_AnonymousObject_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetOnceTest(DistinctId, new
            {
                IgnoreTime,
                StringProperty = StringPropertyValue,
                DecimalProperty = DecimalPropertyValue
            });

            CheckPeopleSetOnce(msg);
        }

        [Test]
        public void PeopleSetOnceTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetOnceTest(DistinctId, new Dictionary<string, object>
            {
                { MixpanelProperty.IgnoreTime, IgnoreTime },
                { StringPropertyName, StringPropertyValue },
                { DecimalPropertyName, DecimalPropertyValue }
            });

            CheckPeopleSetOnce(msg);
        }

        private void CheckPeopleSetOnce(MixpanelMessageTest msg)
        {
            Assert.That(msg.Data.Count, Is.EqualTo(4));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleIgnoreTime], Is.EqualTo(IgnoreTime));
            Assert.That(msg.Data[MixpanelProperty.PeopleSetOnce], Is.TypeOf<Dictionary<string, object>>());
            var setOnce = (Dictionary<string, object>)msg.Data[MixpanelProperty.PeopleSetOnce];
            Assert.That(setOnce.Count, Is.EqualTo(2));
            Assert.That(setOnce[StringPropertyName], Is.EqualTo(StringPropertyValue));
            Assert.That(setOnce[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
        }

        #endregion PeopleSetOnce

        #region PeopleAdd

        [Test]
        public void PeopleAddTest_OnlyNumericInput_CorrectValuesReturned()
        {
            var msg = _client.PeopleAddTest(DistinctId, new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {IntPropertyName, IntPropertyValue},
                {DoublePropertyName, DoublePropertyValue}
            });

            CheckPeopleAdd(msg);
        }

        [Test]
        public void PeopleAddTest_MixedInput_CorrectValuesReturned()
        {
            var msg = _client.PeopleAddTest(DistinctId, new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {IntPropertyName, IntPropertyValue},
                {DoublePropertyName, DoublePropertyValue},
                {MixpanelProperty.Created, Time},
                {StringPropertyName, StringPropertyValue}
            });

            CheckPeopleAdd(msg);
        }

        private void CheckPeopleAdd(MixpanelMessageTest msg)
        {
            Assert.That(msg.Data.Count, Is.EqualTo(3));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleAdd], Is.TypeOf<Dictionary<string, object>>());
            var add = (Dictionary<string, object>)msg.Data[MixpanelProperty.PeopleAdd];
            Assert.That(add.Count, Is.EqualTo(3));
            Assert.That(add[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
            Assert.That(add[IntPropertyName], Is.EqualTo(IntPropertyValue));
            Assert.That(add[DoublePropertyName], Is.EqualTo(DoublePropertyValue));
        }

        #endregion PeopleAdd

        #region PeopleAppend

        [Test]
        public void PeopleAppendTest_ValidInput_CorrectValuesReturned()
        {
            var msg = _client.PeopleAppendTest(DistinctId, new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyValue},
                {StringPropertyName, StringPropertyValue}
            });

            Assert.That(msg.Data.Count, Is.EqualTo(3));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleAppend], Is.TypeOf<Dictionary<string, object>>());
            var append = (Dictionary<string, object>)msg.Data[MixpanelProperty.PeopleAppend];
            Assert.That(append.Count, Is.EqualTo(2));
            Assert.That(append[DecimalPropertyName], Is.EqualTo(DecimalPropertyValue));
            Assert.That(append[StringPropertyName], Is.EqualTo(StringPropertyValue));
        }

        #endregion

        #region PeopleUnion

        [Test]
        public void PeopleUnionTest_ListsInput_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnionTest(DistinctId, new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyArray},
                {StringPropertyName, StringPropertyArray}
            });

            CheckPeopleUnion(msg);
        }

        [Test]
        public void PeopleUnionTest_MixedInput_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnionTest(DistinctId, new Dictionary<string, object>
            {
                {DecimalPropertyName, DecimalPropertyArray},
                {StringPropertyName, StringPropertyArray},
                {IntPropertyName, IntPropertyValue},
                {DoublePropertyName, DoublePropertyValue},
            });

            CheckPeopleUnion(msg);
        }

        private static void CheckPeopleUnion(MixpanelMessageTest msg)
        {
            Assert.That(msg.Data.Count, Is.EqualTo(3));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleUnion], Is.TypeOf<Dictionary<string, object>>());
            var append = (Dictionary<string, object>)msg.Data[MixpanelProperty.PeopleUnion];
            Assert.That(append.Count, Is.EqualTo(2));
            Assert.That(append[DecimalPropertyName], Is.EquivalentTo(DecimalPropertyArray));
            Assert.That(append[StringPropertyName], Is.EquivalentTo(StringPropertyArray));
        }

        #endregion PeopleUnion

        #region PeopleUnset

        [Test]
        public void PeopleUnsetTest_CorrectValuesReturned()
        {
            var msg = _client.PeopleUnsetTest(DistinctId, StringPropertyArray);

            Assert.That(msg.Data.Count, Is.EqualTo(3));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleUnset], Is.TypeOf<List<object>>());
            var unset = (List<object>)msg.Data[MixpanelProperty.PeopleUnset];
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
        public void PeopleDeleteTest_CorrectValuesReturned()
        {
            var res = _client.PeopleDeleteTest(DistinctId);

            Assert.That(res.Data.Count, Is.EqualTo(3));
            Assert.That(res.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(res.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(res.Data[MixpanelProperty.PeopleDelete], Is.Empty);
        }

        #endregion PeopleDelete

        #region PeopleTrackCharge

        [Test]
        public void PeopleTrackChargeTest_NoTime_CorrectValuesReturned()
        {
            _client.UtcNow = () => Time;
            var msg = _client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue);

            CheckPeopleTrackCharge(msg);
        }

        [Test]
        public void PeopleTrackChargeTest_WithTime_CorrectValuesReturned()
        {
            var msg = _client.PeopleTrackChargeTest(DistinctId, DecimalPropertyValue, Time);

            CheckPeopleTrackCharge(msg);
        }

        private void CheckPeopleTrackCharge(MixpanelMessageTest msg)
        {
            Assert.That(msg.Data.Count, Is.EqualTo(3));
            Assert.That(msg.Data[MixpanelProperty.PeopleToken], Is.EqualTo(Token));
            Assert.That(msg.Data[MixpanelProperty.PeopleDistinctId], Is.EqualTo(DistinctId));
            Assert.That(msg.Data[MixpanelProperty.PeopleAppend], Is.TypeOf<Dictionary<string, object>>());
            var append = (Dictionary<string, object>)msg.Data[MixpanelProperty.PeopleAppend];
            Assert.That(append.Count, Is.EqualTo(1));
            Assert.That(append[MixpanelProperty.PeopleTransactions], Is.TypeOf<Dictionary<string, object>>());
            var transactions = (Dictionary<string, object>)append[MixpanelProperty.PeopleTransactions];
            Assert.That(transactions.Count, Is.EqualTo(2));
            Assert.That(transactions[MixpanelProperty.PeopleTime], Is.EqualTo(TimeFormat));
            Assert.That(transactions[MixpanelProperty.PeopleAmount], Is.EqualTo(DecimalPropertyValue));
        }

        #endregion PeopleTrackCharge

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
            var props = (Dictionary<string, object>) msg.Data[MixpanelProperty.TrackProperties];
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

        public JObject ParseMessageData(string data)
        {
            // Remove "data="
            var base64 = data.Remove(0, 5);
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var msg = (JObject)JsonConvert.DeserializeObject(json);
            return msg;
        }

        public string ToDateTimeString(JToken jToken)
        {
            return jToken.Value<DateTime>().ToString(ValueParser.MixpanelDateFormat);
        }
    }
}