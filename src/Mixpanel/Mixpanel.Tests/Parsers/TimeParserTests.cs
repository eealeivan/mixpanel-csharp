using System;
using Mixpanel.Parsers;
using NUnit.Framework;

namespace Mixpanel.Tests.Parsers
{
    [TestFixture]
    public class TimeParserTests : MixpanelTestsBase
    {
        [Test]
        public void Given_ParsingUnixTime_When_Long_Then_Success()
        {
            AssertParseUnixSuccess(TimeUnix, TimeUnix);
        }

        [Test]
        public void Given_ParsingUnixTime_When_DateTime_Then_Success()
        {
            AssertParseUnixSuccess(Time, TimeUnix);
        }

        [Test]
        public void Given_ParsingUnixTime_When_DateTimeOffset_Then_Success()
        {
            AssertParseUnixSuccess(TimeOffset, TimeUnix);
        }

        [Test]
        public void Given_ParsingUnixTime_When_Null_Then_Fail()
        {
            DateTime? nullTime = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            AssertParseUnixFail(nullTime);
        }

        [Test]
        public void Given_ParsingMixpanelFormatTime_When_DateTime_Then_Success()
        {
            AssertParseMixpanelFormatSuccess(Time, TimeFormat);
        }

        [Test]
        public void Given_ParsingMixpanelFormatTime_When_DateTimeOffset_Then_Success()
        {
            AssertParseMixpanelFormatSuccess(TimeOffset, TimeFormat);
        }

        [Test]
        public void Given_ParsingMixpanelFormatTime_When_CorrectFormatString_Then_Success()
        {
            AssertParseMixpanelFormatSuccess(TimeFormat, TimeFormat);
        }

        [Test]
        public void Given_ParsingMixpanelFormatTime_When_Null_Then_Fail()
        {
            DateTime? nullTime = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            AssertParseMixpanelFormatFail(nullTime);
        }

        [Test]
        public void Given_ParsingMixpanelFormatTime_When_IncorrectFormatString_Then_Fail()
        {
            AssertParseMixpanelFormatFail("20-JUN-1990 08:03:00");
        }

        private void AssertParseUnixSuccess(object timeToParse, long expectedTime)
        {
            ValueParseResult parseResult = TimeParser.ParseUnix(timeToParse);
            Assert.That(parseResult.Success, Is.True);
            Assert.That(parseResult.Value, Is.EqualTo(expectedTime));
        }

        private void AssertParseUnixFail(object timeToParse)
        {
            ValueParseResult parseResult = TimeParser.ParseUnix(timeToParse);
            Assert.That(parseResult.Success, Is.False);
            Assert.That(parseResult.Value, Is.Null);
            Assert.That(parseResult.ErrorDetails, Is.Not.Empty);
        }

        private void AssertParseMixpanelFormatSuccess(object timeToParse, string expectedTime)
        {
            ValueParseResult parseResult = TimeParser.ParseMixpanelFormat(timeToParse);
            Assert.That(parseResult.Success, Is.True);
            Assert.That(parseResult.Value, Is.EqualTo(expectedTime));
        }

        private void AssertParseMixpanelFormatFail(object timeToParse)
        {
            ValueParseResult parseResult = TimeParser.ParseMixpanelFormat(timeToParse);
            Assert.That(parseResult.Success, Is.False);
            Assert.That(parseResult.Value, Is.Null);
            Assert.That(parseResult.ErrorDetails, Is.Not.Empty);
        }
    }
}