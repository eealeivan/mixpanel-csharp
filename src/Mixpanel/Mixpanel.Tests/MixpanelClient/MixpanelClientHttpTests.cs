using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private List<Func<Task>> mixpanelMethods;

        [SetUp]
        public void SetUp()
        {
            MixpanelConfig.Global.Reset();

            urls = new List<string>();
            client = new Mixpanel.MixpanelClient(Token, GetConfig());

            mixpanelMethods = new List<Func<Task>>
            {
                () => client.TrackAsync(Event, DistinctId, DictionaryWithStringProperty),
                () => client.AliasAsync(DistinctId, Alias),
                () => client.PeopleSetAsync(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleSetOnceAsync(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleAddAsync(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleAppendAsync(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleUnionAsync(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleRemoveAsync(DistinctId, DictionaryWithStringProperty),
                () => client.PeopleUnsetAsync(DistinctId, StringPropertyArray),
                () => client.PeopleDeleteAsync(DistinctId),
                () => client.PeopleTrackChargeAsync(DistinctId, DecimalPropertyValue),
                () => client.SendAsync(new[]
                {
                    new MixpanelMessage
                    {
                        Kind = MessageKind.PeopleSet,
                        Data = DictionaryWithStringProperty
                    }
                })
            };
        }

        private MixpanelConfig GetConfig()
        {
            return new MixpanelConfig
            {
                AsyncHttpPostFn = (endpoint, data, cancellationToken) =>
                {
                    urls.Add(endpoint);
                    return Task.FromResult(true);
                }
            };
        }

        [Test]
        public async Task Given_CallingAllMethods_When_DefaultConfig_Then_UrlsAreValid()
        {
            await CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(IsUrlValid(url)));
        }

        [Test]
        public async Task Given_CallingAllMethods_When_DefaultConfig_Then_NoIpParam()
        {
            await CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Is.Not.Contains(IpParam)));
        }

        [Test]
        public async Task Given_CallingAllMethods_When_ConfigUseRequestIp_Then_IpParam()
        {
            MixpanelConfig.Global.IpAddressHandling = MixpanelIpAddressHandling.UseRequestIp;

            await CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.Contain(Ip1Param)));
        }

        [Test]
        public async Task Given_CallingAllMethods_When_ConfigIgnoreRequestIp_Then_IpParam()
        {
            MixpanelConfig.Global.IpAddressHandling = MixpanelIpAddressHandling.IgnoreRequestIp;

            await CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.Contain(Ip0Param)));
        }

        [Test]
        public async Task Given_CallingAllMethods_When_DefaultConfig_Then_UseDefaultHost()
        {
            await CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.StartWith("https://api.mixpanel.com")));
        }

        [Test]
        public async Task Given_CallingAllMethods_When_ConfigUseDefaultDataResidency_Then_UseDefaultHost()
        {
            MixpanelConfig.Global.DataResidencyHandling = MixpanelDataResidencyHandling.Default;

            await CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.StartWith("https://api.mixpanel.com")));
        }

        [Test]
        public async Task Given_CallingAllMethods_When_ConfigUseEUDataResidency_Then_UseEUHost()
        {
            MixpanelConfig.Global.DataResidencyHandling = MixpanelDataResidencyHandling.EU;

            await CallAllMixpanelMethods();
            AssertAllUrls(url => Assert.That(url, Does.StartWith("https://api-eu.mixpanel.com")));
        }

        private async Task CallAllMixpanelMethods()
        {
            foreach (Func<Task> mixpanelMethod in mixpanelMethods)
            {
                await mixpanelMethod();
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
            return
                Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttps;
        }
    }
}