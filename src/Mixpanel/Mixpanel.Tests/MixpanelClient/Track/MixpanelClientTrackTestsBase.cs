using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Mixpanel.Tests.MixpanelClient.Track
{
    [TestFixture]
    public abstract class MixpanelClientTrackTestsBase : MixpanelClientTestsBase
    {
        protected TrackSuperPropsDetails? SuperPropsDetails { get; private set; }

        [SetUp]
        public void MixpanelClientPeopleSetUp()
        {
            var properties = TestContext.CurrentContext.Test.Properties;
            if (properties != null && properties.ContainsKey(TrackSuperPropsAttribute.Name))
            {
                SuperPropsDetails = (TrackSuperPropsDetails)properties[TrackSuperPropsAttribute.Name].Single();
                Client = new Mixpanel.MixpanelClient(Token, GetConfig(), GetSuperProperties());
            }
        }

        protected void AssertJsonSpecialProperties(
            JObject msg,
            object expectedDistinctId)
        {
            Assert.That(msg.Count, Is.EqualTo(2));
            Assert.That(msg["event"].Value<string>(), Is.EqualTo(Event));

            var props = (JObject)msg["properties"];
            Assert.That(props["token"].Value<string>(), Is.EqualTo(Token));
            if (expectedDistinctId != null)
            {
                Assert.That(props["distinct_id"].Value<string>(), Is.EqualTo(expectedDistinctId));
            }
            Assert.That(props["ip"].Value<string>(), Is.EqualTo(Ip));
            Assert.That(props["time"].Value<long>(), Is.EqualTo(TimeUnix));
        }

        protected void AssertDictionarySpecialProperties(
            IDictionary<string, object> dic,
            object expectedDistinctId)
        {
            Assert.That(dic.Count, Is.EqualTo(2));
            Assert.That(dic["event"], Is.EqualTo(Event));

            Assert.That(dic["properties"], Is.TypeOf<Dictionary<string, object>>());
            var props = (Dictionary<string, object>)dic["properties"];
            Assert.That(props["token"], Is.EqualTo(Token));
            if (expectedDistinctId != null)
            {
                Assert.That(props["distinct_id"], Is.EqualTo(expectedDistinctId));
            }
            Assert.That(props["ip"], Is.EqualTo(Ip));
            Assert.That(props["time"], Is.EqualTo(TimeUnix));
        }

        private IDictionary<string, object> GetSuperProperties()
        {
            var dic = new Dictionary<string, object>();

            if (SuperPropsDetails?.HasFlag(TrackSuperPropsDetails.DistinctId) ?? false)
            {
                dic.Add(MixpanelProperty.DistinctId, SuperDistinctId);
            }

            if (SuperPropsDetails?.HasFlag(TrackSuperPropsDetails.SpecialProperties) ?? false)
            {
            }

            if (SuperPropsDetails?.HasFlag(TrackSuperPropsDetails.UserProperties) ?? false)
            {
                dic.Add(DecimalSuperPropertyName, DecimalSuperPropertyValue);
                dic.Add(StringSuperPropertyName, StringSuperPropertyValue);
            }

            return dic;
        }
    }
}