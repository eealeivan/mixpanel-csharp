namespace Mixpanel.Parsers
{
    internal static class BoolParser
    {
        public static ValueParseResult Parse(object rawValue)
        {
            switch (rawValue)
            {
                case null:
                    return ValueParseResult.CreateFail("Can't be null.");

                case bool _:
                    return ValueParseResult.CreateSuccess(rawValue);

                default:
                    return ValueParseResult.CreateFail("Expected type is: bool.");
            }
        }
    }
}