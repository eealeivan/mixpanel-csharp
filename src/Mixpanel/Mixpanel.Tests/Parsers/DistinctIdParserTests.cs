using System;
using Mixpanel.Parsers;
using NUnit.Framework;

namespace Mixpanel.Tests.Parsers
{
    [TestFixture]
    public class DistinctIdParserTests : MixpanelTestsBase
    {
        [Test]
        public void When_String_Then_Success()
        {
            AssertSuccess(DistinctId, DistinctId);
        }

        [Test]
        public void When_Int_Then_Success()
        {
            AssertSuccess(DistinctIdInt, DistinctIdInt.ToString());
        }

        [Test]
        public void When_Guid_Then_Success()
        {
            var guidDistinctId = Guid.NewGuid();
            AssertSuccess(guidDistinctId, guidDistinctId.ToString());
        }

        [Test]
        public void When_Null_Then_Fail()
        {
            string nullDistinctId = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            AssertFail(nullDistinctId);
        }

        private void AssertSuccess(object distinctIdToParse, string expectedDistinctId)
        {
            ValueParseResult parseResult = DistinctIdParser.Parse(distinctIdToParse);
            Assert.That(parseResult.Success, Is.True);
            Assert.That(parseResult.Value, Is.EqualTo(expectedDistinctId));
        }

        private void AssertFail(object distinctIdToParse)
        {
            ValueParseResult parseResult = DistinctIdParser.Parse(distinctIdToParse);
            Assert.That(parseResult.Success, Is.False);
            Assert.That(parseResult.Value, Is.Null);
            Assert.That(parseResult.ErrorDetails, Is.Not.Empty);
        }
    }
}