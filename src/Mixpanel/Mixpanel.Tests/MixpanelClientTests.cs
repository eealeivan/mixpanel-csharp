using System;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelClientTests
    {
        private MixpanelClient _client;
        private string _endpoint, _data;

        private readonly string _event = "test";
        private readonly object _props = new {Prop1 = "haha", Prop2 = 2.5M};
        private readonly string _distinctId = "456";
        private readonly string _ip = "111.111.111.111";
        private readonly DateTime _now = new DateTime(2013, 11, 30, 0, 0, 0, DateTimeKind.Utc);

        private readonly string _expectedJson = @"{""event"":""test"",""properties"":{""token"":""1234"",""distinct_id"":""456"",""ip"":""111.111.111.111"",""time"":1385769600,""Prop1"":""haha"",""Prop2"":2.5}}";
        private readonly string _expectedEncodedJson = "eyJldmVudCI6InRlc3QiLCJwcm9wZXJ0aWVzIjp7InRva2VuIjoiMTIzNCIsImRpc3RpbmN0X2lkIjoiNDU2IiwiaXAiOiIxMTEuMTExLjExMS4xMTEiLCJ0aW1lIjoxMzg1NzY5NjAwLCJQcm9wMSI6ImhhaGEiLCJQcm9wMiI6Mi41fX0=";

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

            Assert.That(_endpoint, Is.EqualTo("track"));
            Assert.That(_data, Is.EqualTo(_expectedEncodedJson));
        }
    }
}