using Mixpanel.MessageProperties;
using NUnit.Framework;

namespace Mixpanel.Tests.MessageProperties
{
    [TestFixture]
    public class PropertyNameFormatterTests
    {
        [SetUp]
        public void SetUp()
        {
            MixpanelConfig.Global.Reset();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void When_NullOrWhiteSpace_Then_PropertyNameNotChanged(string propertyName)
        {
            AssertFormattedProperty(propertyName, propertyName);
        }

        #region Default

        [Test]
        public void When_DefaultConfig_Then_PropertyNameNotChanged()
        {
            AssertDefault();
        }

        [Test]
        public void When_NoneConfig_Then_PropertyNameNotChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            AssertDefault();

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceCase;
            AssertDefault(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None });
        }

        private void AssertDefault(MixpanelConfig config = null)
        {
            AssertFormattedProperties(
                config,
                ("SomeCoolProperty", "SomeCoolProperty"),
                ("someCoolProperty", "someCoolProperty"),
                ("PropP", "PropP"),
                ("UIControl", "UIControl"));
        }

        #endregion Default

        #region SentenceCase

        [Test]
        public void When_SentenceCaseConfig_Then_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceCase;
            AssertSentenceCase();

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            AssertSentenceCase(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.SentenceCase });
        }

        private void AssertSentenceCase(MixpanelConfig config = null)
        {
            AssertFormattedProperties(
                config,
                ("SomeCoolProperty", "Some cool property"),
                ("someCoolProperty", "Some cool property"),
                ("prop", "Prop"),
                ("PropP", "Prop p"),
                ("UIControl", "UI control"),
                ("nextUIControl", "Next ui control"),
                ("LevelId", "Level id"),
                ("LevelID", "Level id"));
        }

        #endregion SentenceCase

        #region TitleCase

        [Test]
        public void When_TitleCaseConfig_Then_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase;
            AssertTitleCase();

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            AssertTitleCase(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.TitleCase });
        }

        private void AssertTitleCase(MixpanelConfig config = null)
        {
            AssertFormattedProperties(
                config,
                ("SomeCoolProperty", "Some Cool Property"),
                ("someCoolProperty", "Some Cool Property"),
                ("prop", "Prop"),
                ("PropP", "Prop P"),
                ("UIControl", "UI Control"),
                ("nextUIControl", "Next UI Control"),
                ("LevelId", "Level Id"),
                ("LevelID", "Level ID"));
        }

        #endregion TitleCase

        #region LowerCase

        [Test]
        public void When_LowerCaseConfig_Then_PropertyNameChanged()
        {
            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.LowerCase;
            AssertLowerCase();

            MixpanelConfig.Global.MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
            AssertLowerCase(
                new MixpanelConfig { MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.LowerCase });
        }

        private void AssertLowerCase(MixpanelConfig config = null)
        {
            AssertFormattedProperties(
                config,
                ("SomeCoolProperty", "some cool property"),
                ("someCoolProperty", "some cool property"),
                ("prop", "prop"),
                ("PropP", "prop p"),
                ("UIControl", "ui control"),
                ("nextUIControl", "next ui control"),
                ("LevelId", "level id"),
                ("LevelID", "level id"));
        }

        #endregion

        private void AssertFormattedProperties(
            MixpanelConfig config = null,
            params (string original, string expected)[] props)
        {
            foreach (var (original, expected) in props)
            {
                AssertFormattedProperty(original, expected, config);
            }
        }

        private void AssertFormattedProperty(
            string originalPropertyName,
            string expectedPropertyName,
            MixpanelConfig config = null,
            PropertyNameSource propertyNameSource = PropertyNameSource.Default,
            PropertyOrigin propertyOrigin = PropertyOrigin.RawProperty)
        {
            Assert.That(
                PropertyNameFormatter.Format(
                    new ObjectProperty(
                        originalPropertyName,
                        propertyNameSource,
                        propertyOrigin,
                        null),
                    config),
                Is.EqualTo(expectedPropertyName));
        }
    }
}