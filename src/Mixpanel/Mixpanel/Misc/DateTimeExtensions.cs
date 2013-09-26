using System;

namespace Mixpanel.Misc
{
    internal static class DateTimeExtensions
    {
        public const long UnixEpoch = 621355968000000000L;

        public static long ToUnixTime(this DateTime dateTime)
        {
            return (dateTime.ToStableUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerSecond;
        }

        public static DateTime ToStableUniversalTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime == DateTime.MinValue)
                return new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.ToUniversalTime();
        }
    }
}
