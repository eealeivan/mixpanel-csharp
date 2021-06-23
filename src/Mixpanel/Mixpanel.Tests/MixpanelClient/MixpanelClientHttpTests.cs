using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient
{
    [TestFixture]
    public class MixpanelClientHttpTests : MixpanelTestsBase
    {
        private const string IpParam = "ip";
        private const string Ip1Param = "ip=1";
        private const string Ip0Param = "ip=0";

        private List<string> urls;
        private Mixpanel.MixpanelClient client;
        private List<Action> mixpanelMethods;

        [SetUp]
        public void SetUp()
        {
            MixpanelConfig.Global.Reset();

            urls = new List<string>();
            client = new Mixpanel.MixpanelClient(Token, GetConfig());

            mixpanelMethods = new List<Action>
            {
                () => client.Track(Event, DistinctId, DictionaryWithStringProperty),
                () => client.Alias(DistinctId, Alias),
                () => client.PeopleSet(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleSetOnce(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleAdd(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleAppend(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleUnion(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleRemove(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleUnset(DistinctId, StringPropertyArray),
                () => client.PeopleDelete(DistinctId),
                () => client.PeopleTrackCharge(DistinctId, DecimalPropertyValue),
                () => client.Send(new MixpanelMessage
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
                    urls.Add(endpoint);
                    return true;
                }
            };
        }

        [Test]
        public void Given_CallingAllMethods_When_DefaultConfig_Then_UrlsAreValid()
        {
            CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(IsUrlValid(url)));
        }

        [Test]
        public void Given_CallingAllMethods_When_DefaultConfig_Then_NoIpParam()
        {
            CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Is.Not.Contains(IpParam)));
        }

        [Test]
        public void Given_CallingAllMethods_When_ConfigUseRequestIp_Then_IpParam()
        {
            MixpanelConfig.Global.IpAddressHandling = MixpanelIpAddressHandling.UseRequestIp;

            CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.Contain(Ip1Param)));
        }

        [Test]
        public void Given_CallingAllMethods_When_ConfigIgnoreRequestIp_Then_IpParam()
        {
            MixpanelConfig.Global.IpAddressHandling = MixpanelIpAddressHandling.IgnoreRequestIp;

            CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.Contain(Ip0Param)));
        }

        [Test]
        public void Given_CallingAllMethods_When_DefaultConfig_Then_UseDefaultHost()
        {
            CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.StartWith("https://api.mixpanel.com")));
        }

        [Test]
        public void Given_CallingAllMethods_When_ConfigUseDefaultDataResidency_Then_UseDefaultHost()
        {
            MixpanelConfig.Global.DataResidencyHandling = MixpanelDataResidencyHandling.Default;

            CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.StartWith("https://api.mixpanel.com")));
        }

        [Test]
        public void Given_CallingAllMethods_When_ConfigUseEUDataResidency_Then_UseEUHost()
        {
            MixpanelConfig.Global.DataResidencyHandling = MixpanelDataResidencyHandling.EU;

            CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.StartWith("https://api-eu.mixpanel.com")));
        }

        private void CallAllMixpanelMethods()
        {
            foreach (Action mixpanelMethod in mixpanelMethods)
            {
                mixpanelMethod();
            }

            Assert.That(urls.Count, Is.EqualTo(mixpanelMethods.Count));
        }

        private void AssertAllUrls(Action<string> assertFn)
        {
            foreach (string url in urls)
            {
                assertFn(url);
            }
        }

        private bool IsUrlValid(string url)
        {
#if NETCOREAPP11
            return
                Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && uriResult.Scheme == "https";
#else
            return
                Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttps;
#endif
        }
    }
}