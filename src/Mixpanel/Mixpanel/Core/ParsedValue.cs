namespace Mixpanel.Core
{
    internal struct ParsedValue
    {
        public object Value { get; private set; }
        public bool IsValid { get; private set; }

        public ParsedValue(object value, bool isValid) : this()
        {
            Value = value;
            IsValid = isValid;
        }
    }
}