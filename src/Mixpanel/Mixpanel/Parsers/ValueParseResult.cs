namespace Mixpanel.Parsers
{
    public sealed class ValueParseResult
    {
        public bool Success { get; }
        public object Value { get; }
        public string ErrorDetails { get; }

        public ValueParseResult(bool success, object value, string errorDetails)
        {
            Success = success;
            Value = value;
            ErrorDetails = errorDetails;
        }

        public static ValueParseResult CreateSuccess(object value)
        {
            return new ValueParseResult(true, value, null);
        }

        public static ValueParseResult CreateFail(string message)
        {
            return new ValueParseResult(false, null, message);
        }
    }
}