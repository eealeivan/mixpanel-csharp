using System.Collections.Generic;

namespace Mixpanel.MessageProperties
{
    internal static class PeopleSpecialProperty
    {
        public const string Token = "$token";

        public const string DistinctId = "$distinct_id";
        public const string Ip = "$ip";
        public const string Time = "$time";
        public const string IgnoreTime = "$ignore_time";
        public const string IgnoreAlias = "$ignore_alias";

        public const string FirstName = "$first_name";
        public const string LastName = "$last_name";
        public const string Name = "$name";
        public const string Email = "$email";
        public const string Phone = "$phone";
        public const string Created = "$created";

        public static readonly HashSet<string> MessageSpecialProperties = new HashSet<string>(new[]
        {
            Token,
            DistinctId,
            Ip,
            Time,
            IgnoreTime,
            IgnoreAlias
        });

        public static bool IsMessageSpecialProperty(string name) =>
            MessageSpecialProperties.Contains(name);
    }
}