using System;
using System.Collections;
using System.Linq;
using Mixpanel.Misc;

namespace Mixpanel.Core
{
    internal class ValueParser
    {
        public const string MixpanelDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public ParsedValue Parse(object value, bool isRecursiveCall = false)
        {
            if (value == null)
            {
                return Valid(null);
            }

            if (value is string || value is char || value is bool || IsNumeric(value))
            {
                return Valid(value);
            }

            if (value is DateTime)
            {
                var valueDt = (DateTime)value;
                return Valid(valueDt.ToStableUniversalTime().ToString(MixpanelDateFormat));
            }

            if (value is Guid)
            {
                return Valid(((Guid)value).ToString());
            }

            if (!isRecursiveCall && IsEnumerable(value))
            {
                var list = (
                        from object val in value as IEnumerable
                        select Parse(val, true)
                        into parsedVal
                        where parsedVal.IsValid
                        select parsedVal.Value)
                    .ToList();

                return Valid(list);
            }

            return Invalid(value);
        }

        public bool IsNumeric(object value)
        {
            if (value is int || value is double || value is decimal || value is float ||
                value is short || value is ushort || value is uint || value is long ||
                value is ulong || value is byte || value is sbyte)
            {
                return true;
            }
            return false;
        }

        public bool IsEnumerable(object value)
        {
            return !(value is string) && value is IEnumerable;
        }

        private ParsedValue Valid(object value)
        {
            return new ParsedValue(value, true);
        }

        private ParsedValue Invalid(object value)
        {
            return new ParsedValue(value, false);
        }
    }
}
