using Mixpanel.Exceptions;
using NUnit.Framework;

namespace Mixpanel.Tests.MixpanelClient
{
    [TestFixture]
    public class MixpanelClientJsonSerializerTests : MixpanelTestsBase
    {
#if !JSON

        [Test]
        public void Track_JsonSerializerFnNotSet_ThrowsException()
        {
            var client = new Mixpanel.MixpanelClient(Token);

            Assert.That(
                () => { client.Track(Event, DistinctId, new {}); },
                Throws.TypeOf<MixpanelConfigurationException>());
        }
#endif
    }
}