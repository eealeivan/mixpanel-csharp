namespace Mixpanel.Parsers
{
    internal  static class StringParser
    {
        public static ValueParseResult Parse(object rawValue)
        {
            switch (rawValue)
            {
                case null:
                    return ValueParseResult.CreateFail("Can't be null.");
                case string _:
                    return ValueParseResult.CreateSuccess(rawValue);
                default:
                    return ValueParseResult.CreateFail("Expected type is: string.");
            }
        }
    }
}