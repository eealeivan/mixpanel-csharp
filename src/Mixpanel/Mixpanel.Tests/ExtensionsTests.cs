using System;
using Mixpanel.Misc;
using NUnit.Framework;

namespace Mixpanel.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void DateTimeExtensions_converts_date_time_to_unix_time()
        {
            // UTC time
            var utcDateTime = new DateTime(2013, 9, 26, 22, 21, 11, DateTimeKind.Utc);
            Assert.AreEqual(1380234071L, utcDateTime.ToUnixTime());

            // Local time
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);
            Assert.AreEqual(1380234071L, localDateTime.ToUnixTime());

            // Min value
            Assert.AreEqual(-62135596800L, DateTime.MinValue.ToUnixTime());
        }
    }
}
