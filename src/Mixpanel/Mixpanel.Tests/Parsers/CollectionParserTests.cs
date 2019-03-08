using System;
using System.Collections;
using Mixpanel.Parsers;
using NUnit.Framework;

namespace Mixpanel.Tests.Parsers
{
    [TestFixture]
    public class CollectionParserTests : MixpanelTestsBase
    {
        [Test]
        public void Given_CollectionOfNumbers_When_OnlyNumbersAllowed_Then_AllNumbersParsed()
        {
            AssertSuccess(
                NumberParser.Parse,
                new object[] { 1, 5M, 2.3D },
                new object[] { 1, 5M, 2.3D });
        }

        [Test]
        public void Given_CollectionOfMixedTypes_When_OnlyNumbersAllowed_Then_OnlyNumbersParsed()
        {
            AssertSuccess(
                NumberParser.Parse,
                new object[] { 1, 5M, 2.3D, StringPropertyValue, Created, Duration },
                new object[] { 1, 5M, 2.3D });
        }

        [Test]
        public void Given_CollectionOfMixedTypes_When_MixedTypesAllowed_Then_AllElementsParsed()
        {
            AssertSuccess(
                rawValue => GenericPropertyParser.Parse(rawValue, allowCollections: false),
                new object[] { 1, 5M, 2.3D, StringPropertyValue, Created, Duration },
                new object[] { 1, 5M, 2.3D, StringPropertyValue, CreatedFormat, Duration });
        }

        [Test]
        public void Given_NonCollectionType_When_MixedTypesAllowed_Then_FailReturned()
        {
            AssertFail(
                rawValue => GenericPropertyParser.Parse(rawValue, allowCollections: false), 
                Created);
            AssertFail(
                rawValue => GenericPropertyParser.Parse(rawValue, allowCollections: false), 
                StringPropertyValue);
        }

        private void AssertSuccess(
            Func<object, ValueParseResult> itemParseFn,
            object collectionToParse,
            IEnumerable expectedCollection)
        {
            ValueParseResult parseResult = CollectionParser.Parse(collectionToParse, itemParseFn);
            Assert.That(parseResult.Success, Is.True);
            Assert.That(parseResult.Value, Is.EquivalentTo(expectedCollection));
        }

        private void AssertFail(
            Func<object, ValueParseResult> itemParseFn,
            object collectionToParse)
        {
            ValueParseResult parseResult = CollectionParser.Parse(collectionToParse, itemParseFn);
            Assert.That(parseResult.Success, Is.False);
            Assert.That(parseResult.Value, Is.Null);
            Assert.That(parseResult.ErrorDetails, Is.Not.Empty);
        }
    }
}