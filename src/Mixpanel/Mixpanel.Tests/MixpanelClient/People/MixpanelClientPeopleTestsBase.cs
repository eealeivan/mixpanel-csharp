using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Mixpanel.Tests.MixpanelClient.People
{
    [TestFixture]
    public abstract class MixpanelClientPeopleTestsBase : MixpanelClientTestsBase
    {
        protected PeopleSuperPropsDetails? SuperPropsDetails { get; private set; }

        [SetUp]
        public void MixpanelClientPeopleSetUp()
        {
            var properties = TestContext.CurrentContext.Test.Properties;
            if (properties != null && properties.ContainsKey(PeopleSuperPropsAttribute.Name))
            {
                SuperPropsDetails = (PeopleSuperPropsDetails)properties[PeopleSuperPropsAttribute.Name].Single();
                Client = new Mixpanel.MixpanelClient(Token, GetConfig(), GetSuperProperties());
            }
        }

        protected void AssertJsonMessageProperties(
            JObject msg, 
            object expectedDistinctId)
        {
            Assert.That(msg.Count, Is.EqualTo(7)); // +1 for operation
            Assert.That(msg["$token"].Value<string>(), Is.EqualTo(Token));
            Assert.That(msg["$distinct_id"].Value<string>(), Is.EqualTo(expectedDistinctId));
            Assert.That(msg["$ip"].Value<string>(), Is.EqualTo(Ip));
            Assert.That(msg["$time"].Value<long>(), Is.EqualTo(TimeUnix));
            Assert.That(msg["$ignore_time"].Value<bool>(), Is.EqualTo(IgnoreTime));
            Assert.That(msg["$ignore_alias"].Value<bool>(), Is.EqualTo(IgnoreAlias));
        }

        protected void AssertDictionaryMessageProperties(
            IDictionary<string, object> dic, 
            object expectedDistinctId)
        {
            Assert.That(dic.Count, Is.EqualTo(7)); // +1 for operation
            Assert.That(dic["$token"], Is.EqualTo(Token));
            Assert.That(dic["$distinct_id"], Is.EqualTo(expectedDistinctId));
            Assert.That(dic["$ip"], Is.EqualTo(Ip));
            Assert.That(dic["$time"], Is.EqualTo(TimeUnix));
            Assert.That(dic["$ignore_time"], Is.EqualTo(IgnoreTime));
            Assert.That(dic["$ignore_alias"], Is.EqualTo(IgnoreAlias));
        }

        private IDictionary<string, object> GetSuperProperties()
        {
            var dic = new Dictionary<string, object>();

            if (SuperPropsDetails?.HasFlag(PeopleSuperPropsDetails.DistinctId) ?? false)
            {
                dic.Add(MixpanelProperty.DistinctId, SuperDistinctId);
            }

            if (SuperPropsDetails?.HasFlag(PeopleSuperPropsDetails.MessageSpecialProperties) ?? false)
            {
                dic.Add(MixpanelProperty.Ip, Ip);
                dic.Add(MixpanelProperty.Time, Time);
                dic.Add(MixpanelProperty.IgnoreTime, IgnoreTime);
                dic.Add(MixpanelProperty.IgnoreAlias, IgnoreAlias);
            }

            if (SuperPropsDetails?.HasFlag(PeopleSuperPropsDetails.UserProperties) ?? false)
            {
                dic.Add(DecimalSuperPropertyName, DecimalSuperPropertyValue);
                dic.Add(StringSuperPropertyName, StringSuperPropertyValue);
            }

            return dic;
        }
    }
}