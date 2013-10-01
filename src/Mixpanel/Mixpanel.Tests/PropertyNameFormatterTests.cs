using Mixpanel.Builders;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class PropertyNameFormatterTests
    {
        [Test]
        public void PropertyNameFormatter_doesnt_change_property_with_default_config()
        {
            MixpanelGlobalConfig.Reset();

            var formatter1 = new PropertyNameFormatter();
            Assert.AreEqual("SomeCoolProperty", formatter1.Format("SomeCoolProperty"));
            Assert.AreEqual("someCoolProperty", formatter1.Format("someCoolProperty"));

            var formatter2 = new PropertyNameFormatter(new MixpanelConfig());
            Assert.AreEqual("SomeCoolProperty", formatter2.Format("SomeCoolProperty"));
            Assert.AreEqual("someCoolProperty", formatter2.Format("someCoolProperty"));
        }

        [Test]
        public void PropertyNameFormatter_doesnt_change_property_if_none()
        {
            MixpanelGlobalConfig.Reset();
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.None;

            var formatter1 = new PropertyNameFormatter();
            Assert.AreEqual("SomeCoolProperty", formatter1.Format("SomeCoolProperty"));
            Assert.AreEqual("someCoolProperty", formatter1.Format("someCoolProperty"));

            var formatter2 = new PropertyNameFormatter(
                new MixpanelConfig {PropertyNameFormat = PropertyNameFormat.None});
            Assert.AreEqual("SomeCoolProperty", formatter2.Format("SomeCoolProperty"));
            Assert.AreEqual("someCoolProperty", formatter2.Format("someCoolProperty"));
        }

        [Test]
        public void PropertyNameFormatter_formats_property_to_sentence_title_case()
        {
            MixpanelGlobalConfig.Reset();
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.SentenceTitleCase;

            var formatter1 = new PropertyNameFormatter();
            Assert.AreEqual("Some Cool Property", formatter1.Format("SomeCoolProperty"));
            Assert.AreEqual("Some Cool Property", formatter1.Format("someCoolProperty"));
            Assert.AreEqual("Prop", formatter1.Format("prop"));
            Assert.AreEqual("Prop P", formatter1.Format("PropP"));

            var formatter2 = new PropertyNameFormatter(
                new MixpanelConfig { PropertyNameFormat = PropertyNameFormat.SentenceTitleCase });
            Assert.AreEqual("Some Cool Property", formatter2.Format("SomeCoolProperty"));
            Assert.AreEqual("Some Cool Property", formatter2.Format("someCoolProperty"));
            Assert.AreEqual("Prop", formatter2.Format("prop"));
            Assert.AreEqual("Prop P", formatter2.Format("PropP"));
        }

        [Test]
        public void PropertyNameFormatter_formats_property_to_sentence_capitalized()
        {
            MixpanelGlobalConfig.Reset();
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.SentenseCapitilized;

            var formatter1 = new PropertyNameFormatter();
            Assert.AreEqual("Some cool property", formatter1.Format("SomeCoolProperty"));
            Assert.AreEqual("Some cool property", formatter1.Format("someCoolProperty"));
            Assert.AreEqual("Prop", formatter1.Format("prop"));
            Assert.AreEqual("Prop p", formatter1.Format("PropP"));

            var formatter2 = new PropertyNameFormatter(
                new MixpanelConfig { PropertyNameFormat = PropertyNameFormat.SentenseCapitilized });
            Assert.AreEqual("Some cool property", formatter2.Format("SomeCoolProperty"));
            Assert.AreEqual("Some cool property", formatter2.Format("someCoolProperty"));
            Assert.AreEqual("Prop", formatter2.Format("prop"));
            Assert.AreEqual("Prop p", formatter2.Format("PropP"));
        }

        [Test]
        public void PropertyNameFormatter_formats_property_to_sentence_lower_case()
        {
            MixpanelGlobalConfig.Reset();
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.SentenceLowerCase;

            var formatter1 = new PropertyNameFormatter();
            Assert.AreEqual("some cool property", formatter1.Format("SomeCoolProperty"));
            Assert.AreEqual("some cool property", formatter1.Format("someCoolProperty"));
            Assert.AreEqual("prop", formatter1.Format("prop"));
            Assert.AreEqual("prop p", formatter1.Format("PropP"));

            var formatter2 = new PropertyNameFormatter(
                new MixpanelConfig { PropertyNameFormat = PropertyNameFormat.SentenceLowerCase });
            Assert.AreEqual("some cool property", formatter2.Format("SomeCoolProperty"));
            Assert.AreEqual("some cool property", formatter2.Format("someCoolProperty"));
            Assert.AreEqual("prop", formatter2.Format("prop"));
            Assert.AreEqual("prop p", formatter2.Format("PropP"));
        }
    }
}