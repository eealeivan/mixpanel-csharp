using System;

namespace Mixpanel.Builders
{
    internal class ValueParser
    {
        public Tuple<object, bool> Parse(object value)
        {
            if (value is string || value is int || value is long ||
                value is double || value is decimal || value is bool)
            {
                return Tuple.Create(value, true);
            }

            if (value is DateTime)
            {
                var valueDt = (DateTime)value;
                return new Tuple<object, bool>(valueDt.ToString("yyyy-MM-ddTHH:mm:ss"), true);
            }

            if (value is float || value is short || value is ushort || value is uint || 
                value is ulong || value is byte || value is sbyte || value is char)
            {
                return Tuple.Create(value, true);
            }

            if (value == null)
            {
                return new Tuple<object, bool>(null, true);
            }

            return new Tuple<object, bool>(value, false);
        }
    }
}
