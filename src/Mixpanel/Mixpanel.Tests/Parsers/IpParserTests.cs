using System.Linq;
using System.Net;
using Mixpanel.Parsers;
using NUnit.Framework;

namespace Mixpanel.Tests.Parsers
{
    [TestFixture]
    public class IpParserTests : MixpanelTestsBase
    {
        [Test]
        public void Given_StringInput_When_ValidIp_Then_Success()
        {
            AssertSuccess(Ip, Ip);
        }

        [Test]
        public void Given_StringInput_When_InvalidIp_Then_Fail()
        {
            AssertFail("023.44.33.22");
        }

#if !NETCOREAPP11

        [Test]
        public void Given_IPAddressInput_When_ValidIp_Then_Success()
        {
            byte[] ipBytes = Ip.Split('.').Select(byte.Parse).ToArray();
            var ipAddress = new IPAddress(ipBytes);

            AssertSuccess(ipAddress, Ip);
        }

#endif

        private void AssertSuccess(object ipToParse, string expectedIp)
        {
            ValueParseResult parseResult = IpParser.Parse(ipToParse);
            Assert.That(parseResult.Success, Is.True);
            Assert.That(parseResult.Value, Is.EqualTo(expectedIp));
        }

        private void AssertFail(object ipToParse)
        {
            ValueParseResult parseResult = IpParser.Parse(ipToParse);
            Assert.That(parseResult.Success, Is.False);
            Assert.That(parseResult.Value, Is.Null);
            Assert.That(parseResult.ErrorDetails, Is.Not.Empty);
        }
    }
}