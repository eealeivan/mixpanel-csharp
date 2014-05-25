using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelClientTests
    {
        private MixpanelClient _client;
        private string _endpoint, _data;

        private readonly string _event = "test";
        private readonly object _props = new { Prop1 = "haha", Prop2 = 2.5M };
        private readonly string _distinctId = "456";
        private readonly string _ip = "111.111.111.111";
        private readonly DateTime _now = new DateTime(2013, 11, 30, 0, 0, 0, DateTimeKind.Utc);

        private const string ExpectedTrackJson = @"{""event"":""test"",""properties"":{""token"":""1234"",""distinct_id"":""456"",""ip"":""111.111.111.111"",""time"":1385769600,""Prop1"":""haha"",""Prop2"":2.5}}";
        private const string ExpectedTrackBase64 = "eyJldmVudCI6InRlc3QiLCJwcm9wZXJ0aWVzIjp7InRva2VuIjoiMTIzNCIsImRpc3RpbmN0X2lkIjoiNDU2IiwiaXAiOiIxMTEuMTExLjExMS4xMTEiLCJ0aW1lIjoxMzg1NzY5NjAwLCJQcm9wMSI6ImhhaGEiLCJQcm9wMiI6Mi41fX0=";
        private const string ExpectedTrackFormData = "data=" + ExpectedTrackBase64;

        [SetUp]
        public void SetUp()
        {
            _endpoint = null;
            _data = null;
            _client = new MixpanelClient("1234",
                new MixpanelConfig
                {
                    HttpPostFn = (endpoint, data) =>
                    {
                        _endpoint = endpoint;
                        _data = data;
                        return true;
                    }
                });
        }

        [Test]
        public void Track_SendSimpleObject_Sent()
        {
            _client.Track(_event, _props, _distinctId, _ip, _now);

            Assert.That(_endpoint, Is.EqualTo("http://api.mixpanel.com/track"));
            Assert.That(_data, Is.EqualTo(ExpectedTrackFormData));
        }

        [Test]
        public void TrackTest_TestSimpleObject_CorrectValuesReturned()
        {
            var res = _client.TrackTest(_event, _props, _distinctId, _ip, _now);

            Assert.That(res.Data.Count, Is.EqualTo(2));
            Assert.That(res.Data["event"], Is.EqualTo("test"));
            Assert.That(res.Data["properties"], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)res.Data["properties"];
            Assert.That(props.Count, Is.EqualTo(6));
            Assert.That(props["token"], Is.EqualTo("1234"));
            Assert.That(props["distinct_id"], Is.EqualTo("456"));
            Assert.That(props["ip"], Is.EqualTo("111.111.111.111"));
            Assert.That(props["time"], Is.EqualTo(1385769600));
            Assert.That(props["Prop1"], Is.EqualTo("haha"));
            Assert.That(props["Prop2"], Is.EqualTo(2.5M));
            Assert.That(res.Json, Is.EqualTo(ExpectedTrackJson));
            Assert.That(res.Base64, Is.EqualTo(ExpectedTrackBase64));
        }


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
        //        MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceLowerCase
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