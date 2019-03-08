using Mixpanel.MessageProperties;

namespace Mixpanel.Parsers
{
    internal static class PeopleSpecialPropertyParser
    {
        public static ValueParseResult Parse(
            string specialPropertyName,
            object rawValue)
        {
            switch (specialPropertyName)
            {
                case PeopleSpecialProperty.Token:
                    return StringParser.Parse(rawValue);

                case PeopleSpecialProperty.DistinctId:
                    return DistinctIdParser.Parse(rawValue);
                   
                case PeopleSpecialProperty.Ip:
                    return IpParser.Parse(rawValue);

                case PeopleSpecialProperty.Time:
                    return TimeParser.ParseUnix(rawValue);

                case PeopleSpecialProperty.IgnoreTime:
                case PeopleSpecialProperty.IgnoreAlias:
                    return BoolParser.Parse(rawValue);

                case PeopleSpecialProperty.FirstName:
                case PeopleSpecialProperty.LastName:
                case PeopleSpecialProperty.Name:
                case PeopleSpecialProperty.Email:
                case PeopleSpecialProperty.Phone:
                    return StringParser.Parse(rawValue);

                case PeopleSpecialProperty.Created:
                    return TimeParser.ParseMixpanelFormat(rawValue);

                default:
                    return ValueParseResult.CreateFail($"No parser for '{nameof(specialPropertyName)}'.");
            }
        }
    }
}