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
            MixpanelGlobalConfig.Reset();
            _defaultFormatter = new PropertyNameFormatter();
        }

        [Test]
        public void Format_DefaultConfig_PropertyNameNotChanged()
        {
            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));

            var formatter = new PropertyNameFormatter(new MixpanelConfig());
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));
        }

        [Test]
        public void Format_NoneConfig_PropertyNameNotChanged()
        {
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.None;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));

            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { PropertyNameFormat = PropertyNameFormat.None });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));
        }

        [Test]
        public void Format_SentenceTitleCaseConfig_PropertyNameChanged()
        {
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.SentenceTitleCase;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(_defaultFormatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(_defaultFormatter.Format("PropP"), Is.EqualTo("Prop P"));

            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { PropertyNameFormat = PropertyNameFormat.SentenceTitleCase });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("Prop P"));
        }

        [Test]
        public void Format_SentenseCapitilizedConfig_PropertyNameChanged()
        {
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.SentenseCapitilized;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(_defaultFormatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(_defaultFormatter.Format("PropP"), Is.EqualTo("Prop p"));

            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { PropertyNameFormat = PropertyNameFormat.SentenseCapitilized });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("Prop p"));

        }

        [Test]
        public void Format_SentenceLowerCaseConfig_PropertyNameChanged()
        {
            MixpanelGlobalConfig.PropertyNameFormat = PropertyNameFormat.SentenceLowerCase;

            Assert.That(_defaultFormatter.Format("SomeCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(_defaultFormatter.Format("someCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(_defaultFormatter.Format("prop"), Is.EqualTo("prop"));
            Assert.That(_defaultFormatter.Format("PropP"), Is.EqualTo("prop p"));

            var formatter = new PropertyNameFormatter(
                new MixpanelConfig { PropertyNameFormat = PropertyNameFormat.SentenceLowerCase });
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("prop p"));
        }
    }
}