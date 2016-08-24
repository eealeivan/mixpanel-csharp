using System;
using System.Collections.Generic;
using NUnit.Framework;
#if (PORTABLE || PORTABLE40)
using Newtonsoft.Json;
#endif

namespace Mixpanel.Tests
{
    [TestFixture]
    public class MixpanelClientHttpTests : MixpanelTestsBase
    {
        private const string IpParam = "ip";
        private const string Ip1Param = "ip=1";
        private const string Ip0Param = "ip=0";

        private List<string> _urls;
        private MixpanelClient _client;
        private List<Action> _mixpanelMethods;

        [SetUp]
        public void SetUp()
        {
            MixpanelConfig.Global.Reset();

            _urls = new List<string>();
            _client = new MixpanelClient(Token, GetConfig());

            _mixpanelMethods = new List<Action>
            {
                () => _client.Track(Event, DistinctId, DictionaryWithStringProperty),
                () => _client.Alias(DistinctId, Alias),
                () => _client.PeopleSet(DistinctId, DictionaryWithStringProperty),
                () => _client.PeopleSetOnce(DistinctId, DictionaryWithStringProperty),
                () => _client.PeopleAdd(DistinctId, DictionaryWithStringProperty),
                () => _client.PeopleAppend(DistinctId, DictionaryWithStringProperty),
                () => _client.PeopleUnion(DistinctId, DictionaryWithStringProperty),
                () => _client.PeopleUnset(DistinctId, StringPropertyArray),
                () => _client.PeopleDelete(DistinctId),
                () => _client.PeopleTrackCharge(DistinctId, DecimalPropertyValue),
                () => _client.Send(new MixpanelMessage
                {
                    Kind = MessageKind.PeopleSet, 
                    Data = DictionaryWithStringProperty
                }),
            };
        }

        private MixpanelConfig GetConfig()
        {
            return new MixpanelConfig
            {
                HttpPostFn = (endpoint, data) =>
                {
                    _urls.Add(endpoint);
                    return true;
                },
#if (PORTABLE || PORTABLE40)
                SerializeJsonFn = obj => JsonConvert.SerializeObject(obj)
#endif
            };
        }

        private void CallAllMixpnaleMethods()
        {
            foreach (Action mixpanelMethod in _mixpanelMethods)
            {
                mixpanelMethod();
            }

            Assert.That(_urls.Count, Is.EqualTo(_mixpanelMethods.Count));
        }

        [Test]
        public void AllMethods_UrlsAreValid()
        {
            CallAllMixpnaleMethods();
            AssertAllUrls(url => Assert.That(IsUrlValid(url)));
        }

        [Test]
        public void IpAddressHandling_None_NoRequestStringParamAdded()
        {
            CallAllMixpnaleMethods();
            AssertAllUrls(url => Assert.That(url, Is.Not.Contains(IpParam)));
        }

        [Test]
        public void IpAddressHandling_UseRequestIp_RequestStringParamAdded()
        {
            MixpanelConfig.Global.IpAddressHandling = MixpanelIpAddressHandling.UseRequestIp;

            CallAllMixpnaleMethods();
            AssertAllUrls(url => Assert.That(url, Is.StringContaining(Ip1Param)));
        }

        [Test]
        public void IpAddressHandling_IgnoreRequestIp_RequestStringParamAdded()
        {
            MixpanelConfig.Global.IpAddressHandling = MixpanelIpAddressHandling.IgnoreRequestIp;

            CallAllMixpnaleMethods();
            AssertAllUrls(url => Assert.That(url, Is.StringContaining(Ip0Param)));
        }

        private void AssertAllUrls(Action<string> assertFn)
        {
            foreach (string url in _urls)
            {
                assertFn(url);
            }
        }

        private bool IsUrlValid(string url)
        {
            Uri uriResult;
            return
                Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}