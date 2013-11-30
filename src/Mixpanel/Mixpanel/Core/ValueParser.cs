using System;
using System.Collections;
using System.Linq;
using Mixpanel.Misc;

namespace Mixpanel.Core
{
    internal class ValueParser
    {
        public const string MixpanelDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public Tuple<object, bool> Parse(object value, bool isRecursiveCall = false)
        {
            if (value == null)
            {
                return Valid(null);
            }

            if (value is string || value is int || value is long ||
                value is double || value is decimal || value is bool)
            {
                return Valid(value);
            }

            if (value is DateTime)
            {
                var valueDt = (DateTime)value;
                return Valid(valueDt.ToStableUniversalTime().ToString(MixpanelDateFormat));
            }

            if (value is float || value is short || value is ushort || value is uint || 
                value is ulong || value is byte || value is sbyte || value is char)
            {
                return Valid(value);
            }

            if (!isRecursiveCall && value is IEnumerable)
            {
                var list = (
                    from object val in (value as IEnumerable)
                    select Parse(val, true)
                    into parsedVal
                    where parsedVal.Item2
                    select parsedVal.Item1).ToList();

                return list.Count > 0 ? Valid(list) : Invalid(list);
            }

            return Invalid(value);
        }

        private Tuple<object, bool> Valid(object value)
        {
            return new Tuple<object, bool>(value, true);
        } 
        
        private Tuple<object, bool> Invalid(object value)
        {
            return new Tuple<object, bool>(value, false);
        }
    }
}
