using System;
using Mixpanel.Misc;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void DateTimeToUnixTime_Works()
        {
            // UTC time
            var utcDateTime = new DateTime(2013, 9, 26, 22, 21, 11, DateTimeKind.Utc);
            Assert.That(utcDateTime.ToUnixTime(), Is.EqualTo(1380234071L));

#if !NETSTANDARD16
            // Local time
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);
            Assert.That(localDateTime.ToUnixTime(), Is.EqualTo(1380234071L));
#endif

            // Min value
            Assert.That(DateTime.MinValue.ToUnixTime(), Is.EqualTo(-62135596800L));
        }
    }
}
