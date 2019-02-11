using System.Collections.Generic;
using Mixpanel.Parsers;

namespace Mixpanel.MessageProperties
{
    internal static class PeopleSpecialPropertyMapper
    {
        private static readonly Dictionary<string, string> RawNameToSpecialPropertyMap;

        static PeopleSpecialPropertyMapper()
        {
            RawNameToSpecialPropertyMap = new Dictionary<string, string>(new PropertyNameComparer())
            {
                { "token", PeopleSpecialProperty.Token },
                { "$token", PeopleSpecialProperty.Token },

                { "distinctid", PeopleSpecialProperty.DistinctId },
                { "distinct_id", PeopleSpecialProperty.DistinctId },
                { "$distinctid", PeopleSpecialProperty.DistinctId },
                { "$distinct_id", PeopleSpecialProperty.DistinctId },
                
                { "ip", PeopleSpecialProperty.Ip },
                { "$ip", PeopleSpecialProperty.Ip },

                { "time", PeopleSpecialProperty.Time },
                { "$time", PeopleSpecialProperty.Time },

                { "ignoretime", PeopleSpecialProperty.IgnoreTime },
                { "ignore_time", PeopleSpecialProperty.IgnoreTime },
                { "$ignoretime", PeopleSpecialProperty.IgnoreTime },
                { "$ignore_time", PeopleSpecialProperty.IgnoreTime },

                { "ignorealias", PeopleSpecialProperty.IgnoreAlias },
                { "ignore_alias", PeopleSpecialProperty.IgnoreAlias },
                { "$ignorealias", PeopleSpecialProperty.IgnoreAlias },
                { "$ignore_alias", PeopleSpecialProperty.IgnoreAlias },

                {"firstname", PeopleSpecialProperty.FirstName },
                {"first_name", PeopleSpecialProperty.FirstName },
                {"$firstname", PeopleSpecialProperty.FirstName },
                {"$first_name", PeopleSpecialProperty.FirstName },

                {"lastname", PeopleSpecialProperty.LastName },
                {"last_name", PeopleSpecialProperty.LastName },
                {"$lastname", PeopleSpecialProperty.LastName },
                {"$last_name", PeopleSpecialProperty.LastName },

                {"name", PeopleSpecialProperty.Name },
                {"$name", PeopleSpecialProperty.Name },

                {"email", PeopleSpecialProperty.Email },
                {"$email", PeopleSpecialProperty.Email },
                {"e-mail", PeopleSpecialProperty.Email },
                {"$e-mail", PeopleSpecialProperty.Email },

                {"phone", PeopleSpecialProperty.Phone },
                {"$phone", PeopleSpecialProperty.Phone },

                {"created", PeopleSpecialProperty.Created },
                {"$created", PeopleSpecialProperty.Created }
            };
        }

        public static string RawNameToSpecialProperty(string propertyName)
        {
            return RawNameToSpecialPropertyMap.TryGetValue(propertyName, out var specialProperty)
                ? specialProperty
                : null;
        }
    }
}