namespace Mixpanel.Parsers
{
    internal static class NumberParser
    {
        public static bool IsNumber(object value)
        {
            if (value is int || value is double || value is decimal || value is float ||
                value is short || value is ushort || value is uint || value is long ||
                value is ulong || value is byte || value is sbyte)
            {
                return true;
            }

            return false;
        }

        public static ValueParseResult Parse(object rawNumber)
        {
            return IsNumber(rawNumber)
                ? ValueParseResult.CreateSuccess(rawNumber)
                : ValueParseResult.CreateFail("Expected type is: number (int, double etc)");
        }
    }
}