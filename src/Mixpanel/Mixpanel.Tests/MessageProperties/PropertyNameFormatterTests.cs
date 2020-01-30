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
                ("someCoolProperty", "someCoolProperty"));
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
                ("Some Cool Property", "Some cool property"));
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
                ("Some Cool Property", "Some Cool Property"),
                ("DistinctID", "Distinct ID"),
                ("DistinctId", "Distinct ID"));
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
                ("Some Cool Property", "some cool property"));
        }

        #endregion

        private void AssertFormattedProperties(
            MixpanelConfig config = null,
            params (string original, string expected)[] props)
        {
            foreach ((string original, string expected) in props)
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