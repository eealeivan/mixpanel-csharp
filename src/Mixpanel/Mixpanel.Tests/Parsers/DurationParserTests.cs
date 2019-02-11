using Mixpanel.Parsers;
using NUnit.Framework;

namespace Mixpanel.Tests.Parsers
{
    [TestFixture]
    public class DurationParserTests : MixpanelTestsBase
    {
        [Test]
        public void When_TimeSpan_Then_Success()
        {
            AssertSuccess(Duration, DurationSeconds);
        }

        [Test]
        public void When_Double_Then_Success()
        {
            AssertSuccess(DurationSeconds, DurationSeconds);
        }

        [Test]
        public void When_Null_Then_Fail()
        {
            string nullDuration = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            AssertFail(nullDuration);
        }

        private void AssertSuccess(object durationToParse, object expectedDuration)
        {
            ValueParseResult parseResult = DurationParser.Parse(durationToParse);
            Assert.That(parseResult.Success, Is.True);
            Assert.That(parseResult.Value, Is.EqualTo(expectedDuration));
        }

        private void AssertFail(object durationToParse)
        {
            ValueParseResult parseResult = DurationParser.Parse(durationToParse);
            Assert.That(parseResult.Success, Is.False);
            Assert.That(parseResult.Value, Is.Null);
            Assert.That(parseResult.ErrorDetails, Is.Not.Empty);
        }
    }
}