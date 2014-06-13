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

        #region Default

        [Test]
        public void Format_DefaultConfig_PropertyNameNotChanged()
        {
            CheckDefault(_defaultFormatter);

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceCase;
            var formatter = new PropertyNameFormatter(new MixpanelConfig());
            CheckDefault(formatter);
        }

        [Test]
        public void Format_NoneConfig_PropertyNameNotChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            CheckDefault(_defaultFormatter);

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceCase;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig {MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None});
            CheckDefault(formatter);
        }

        private void CheckDefault(PropertyNameFormatter formatter)
        {
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("SomeCoolProperty"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("someCoolProperty"));
        }

        #endregion Default

        #region SentenceCase

        [Test]
        public void Format_SentenceCaseConfig_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceCase;
            CheckSentenceCase(_defaultFormatter);

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig {MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceCase});
            CheckSentenceCase(formatter);
        }

        private void CheckSentenceCase(PropertyNameFormatter formatter)
        {
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("Some cool property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("Prop p"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("Prop p"));
            Assert.That(formatter.Format("Some Cool Property"), Is.EqualTo("Some cool property"));
        }

        #endregion SentenceCase

        #region TitleCase

        [Test]
        public void Format_TitleCaseConfig_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase;
            CheckTitleCase(_defaultFormatter);

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig {MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase});
            CheckTitleCase(formatter);
        }

        private void CheckTitleCase(PropertyNameFormatter formatter)
        {
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("Some Cool Property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("Prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("Prop P"));
            Assert.That(formatter.Format("Some Cool Property"), Is.EqualTo("Some Cool Property"));
        }

        #endregion TitleCase

        #region LowerCase

        [Test]
        public void Format_LowerCaseConfig_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.LowerCase;
            CheckLowerCase(_defaultFormatter);

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            var formatter = new PropertyNameFormatter(
                new MixpanelConfig {MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.LowerCase});
            CheckLowerCase(formatter);
        }

        private void CheckLowerCase(PropertyNameFormatter formatter)
        {
            Assert.That(formatter.Format("SomeCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(formatter.Format("someCoolProperty"), Is.EqualTo("some cool property"));
            Assert.That(formatter.Format("prop"), Is.EqualTo("prop"));
            Assert.That(formatter.Format("PropP"), Is.EqualTo("prop p"));
            Assert.That(formatter.Format("Some Cool Property"), Is.EqualTo("some cool property"));
        }

        #endregion
    }
}