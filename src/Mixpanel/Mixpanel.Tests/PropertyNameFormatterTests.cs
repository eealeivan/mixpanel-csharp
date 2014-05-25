using Mixpanel.Core;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class PropertyNameFormatterTests
    {
        private PropertyNameFormatter _defaultFormatter;

        [SetUp]
        public void SetUp()
        {
            MixpanelConfig.Global.Reset();
            _defaultFormatter = new PropertyNameFormatter();
        }

        [Test]
        public void Format_DefaultConfig_PropertyNameNotChanged()
        {
            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceTitleCase;
            var formatter = new PropertyNameFormatter(new MixpanelConfig());
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));
        }

        [Test]
        public void Format_NoneConfig_PropertyNameNotChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceTitleCase;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));
        }

        [Test]
        public void Format_SentenceTitleCaseConfig_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceTitleCase;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(_defaultFormatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(_defaultFormatter.Format("PropP"), Is.EqualTo("Prop P"));

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceTitleCase });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("Prop P"));
        }

        [Test]
        public void Format_SentenseCapitilizedConfig_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenseCapitilized;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(_defaultFormatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(_defaultFormatter.Format("PropP"), Is.EqualTo("Prop p"));

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenseCapitilized });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("Prop p"));

        }

        [Test]
        public void Format_SentenceLowerCaseConfig_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceLowerCase;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(_defaultFormatter.Format("prop"), Is.EqualTo("prop"));
            Assert.That(_defaultFormatter.Format("PropP"), Is.EqualTo("prop p"));

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceLowerCase });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("prop p"));
        }
    }
}