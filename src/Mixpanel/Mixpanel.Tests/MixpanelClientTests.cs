using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelClientTests : MixpanelTestsBase
    {
        private MixpanelClient _client;
        private string _endpoint, _data;


        //private readonly object _props = new { Prop1 = "haha", Prop2 = 2.5M, Ip = _ip, Time = _now };

        //private const string ExpectedTrackJson = @"{""event"":""test"",""properties"":{""token"":""1234"",""distinct_id"":""456"",""ip"":""111.111.111.111"",""time"":1385769600,""Prop1"":""haha"",""Prop2"":2.5}}";
        //private const string ExpectedTrackBase64 = "eyJldmVudCI6InRlc3QiLCJwcm9wZXJ0aWVzIjp7InRva2VuIjoiMTIzNCIsImRpc3RpbmN0X2lkIjoiNDU2IiwiaXAiOiIxMTEuMTExLjExMS4xMTEiLCJ0aW1lIjoxMzg1NzY5NjAwLCJQcm9wMSI6ImhhaGEiLCJQcm9wMiI6Mi41fX0=";
        //private const string ExpectedTrackFormData = "data=" + ExpectedTrackBase64;

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
                    }
                });
            _client.UtcNow = () => DateTime.UtcNow;
            _client.SetSuperProperties(new object());
        }

        //[Test]
        //public void Track_SendSimpleObject_Sent()
        //{
        //    _client.Track(_event, DistinctId, _props);

        //    Assert.That(_endpoint, Is.EqualTo("http://api.mixpanel.com/track"));
        //    Assert.That(_data, Is.EqualTo(ExpectedTrackFormData));
        //}

        #region Track

        [Test]
        public void TrackTest_AnonymousObject_CorrectValuesReturned()
        {
            var msg = _client.TrackTest(Event, DistinctId, new
            {
                Ip,
                Time,
                StringProperty = StringPropertyValue,
                DecimalProperty = DecimalPropertyValue
            });

            Assert.That(msg.Data.Count, Is.EqualTo(2));
            Assert.That(msg.Data[MixpanelProperty.TrackEvent], Is.EqualTo(Event));
            Assert.That(msg.Data[MixpanelProperty.TrackProperties], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)msg.Data[MixpanelProperty.TrackProperties];
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
        public void AliasTest_ValidValues_CorrectValuesReturned()
        {
            var msg = _client.AliasTest(DistinctId, Alias);
            CheckAlias(msg);
        }

        private void CheckAlias(MixpanelMessageTest msg)
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
        public void PeopleSetTest_AnonymousObject_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetTest(DistinctId, new
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
            });

            CheckPeopleSet(msg);
        }

        [Test]
        public void PeopleSetTest_Dictionary_CorrectValuesReturned()
        {
            var msg = _client.PeopleSetTest(DistinctId, new Dictionary<string, object>
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
            });

            CheckPeopleSet(msg);
        }

        private void CheckPeopleSet(MixpanelMessageTest msg)
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

        public class MyClass
        {
            public string PropTest { get; set; }

            [MixpanelName("Mega Date")]
            public DateTime SuperMegaDate { get; set; }

            public List<string> List { get; set; }
        }

        //[Test]
        //public void Realtest()
        //{
        //    var config = new MixpanelConfig
        //    {
        //        MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.LowerCase
        //    };
        //    //var props = new
        //    //{
        //    //    PropTest = "haha",
        //    //    PropSuperTest = 2.5M,
        //    //    MegaDate = DateTime.UtcNow,
        //    //    DistinctId = "890"
        //    //};
        //    var props = new MyClass
        //    {
        //        PropTest = "huhu",
        //        SuperMegaDate = DateTime.UtcNow,
        //        List = new List<string> { "one", "two", "three" }
        //    };
        //    var res = new MixpanelClient("16acd719b243fb6aef1ded661b0ae657", config)
        //        .Track(_event, props, _distinctId, null, DateTime.UtcNow);

        //    Assert.That(res, Is.True);
        //}
    }
}