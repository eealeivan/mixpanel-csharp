using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Mixpanel.Json
{
    internal static class MixpanelJsonSerializer
    {
        public static string Serialize(object obj)
        {
            switch (obj)
            {
                case IDictionary<string, object> message:
                    return SerializeMessage(message);

                case IDictionary<string, object>[] messages:
                    return SerializeMessages(messages);

                default:
                    throw new Exception(
                        "Expected types are IDictionary<string, object> or IDictionary<string, object>[].");
            }
        }

        private static string SerializeMessage(IDictionary<string, object> message)
        {
            var json = new StringBuilder(256);

            Object(json, message);

            return json.ToString();
        }

        private static string SerializeMessages(IDictionary<string, object>[] messages)
        {
            var json = new StringBuilder(512);

            Array(json, messages);

            return json.ToString();
        }

        private static void Object(StringBuilder json, IDictionary<string, object> members)
        {
            json.Append('{');

            int membersCount = members.Count;
            int counter = 0;
            foreach (KeyValuePair<string, object> member in members)
            {
                Member(json, member.Key, member.Value);

                counter++;
                bool isLast = membersCount == counter;
                if (!isLast)
                {
                    json.Append(',');
                }
            }

            json.Append('}');
        }

        private static void Array(StringBuilder json, ICollection values)
        {
            json.Append('[');

            int valuesCount = values.Count;
            int counter = 0;
            foreach (object value in values)
            {
                Value(json, value);

                counter++;
                bool isLast = valuesCount == counter;
                if (!isLast)
                {
                    json.Append(',');
                }
            }

            json.Append(']');
        }

        private static void Member(StringBuilder json, string name, object value)
        {
            Quote(json);
            json.Append(name);
            Quote(json);
            json.Append(':');

            Value(json, value);
        }

        private static void Value(StringBuilder json, object value)
        {
            switch (value)
            {
                case null:
                    json.Append("null");
                    break;

                case bool boolValue:
                    json.Append(boolValue ? "true" : "false");
                    break;

                case char charValue:
                    Quote(json);
                    Escape(json, charValue.ToString());
                    Quote(json);
                    break;

                case string stringValue:
                    Quote(json);
                    Escape(json, stringValue);
                    Quote(json);
                    break;

                case byte byteValue:
                    json.Append(byteValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case sbyte sbyteValue:
                    json.Append(sbyteValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case short shortValue:
                    json.Append(shortValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case ushort ushortValue:
                    json.Append(ushortValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case int intValue:
                    json.Append(intValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case uint uintValue:
                    json.Append(uintValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case long longValue:
                    json.Append(longValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case ulong ulongValue:
                    json.Append(ulongValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case float floatValue:
                    json.Append(floatValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case double doubleValue:
                    json.Append(doubleValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case decimal decimalValue:
                    json.Append(decimalValue.ToString(CultureInfo.InvariantCulture));
                    break;

                case Guid guidValue:
                    Quote(json);
                    json.Append(guidValue.ToString());
                    Quote(json);
                    break;

                case TimeSpan timeSpanValue:
                    Quote(json);
                    json.Append(timeSpanValue.ToString("c", CultureInfo.InvariantCulture));
                    Quote(json);
                    break;

                case IDictionary<string, object> @object:
                    Object(json, @object);
                    break;

                case ICollection array:
                    Array(json, array);
                    break;

                default:
                    throw new Exception($"Unsupported type {value.GetType()}.");
            }
        }

        private static void Escape(StringBuilder json, string stringValue)
        {
            foreach (char @char in stringValue)
            {
                switch (@char)
                {
                    case '"':
                        json.Append('\\');
                        Quote(json);
                        break;

                    case '\\':
                        json.Append('\\');
                        json.Append('\\');
                        break;

                    case '\b':
                        json.Append('\\');
                        json.Append('b');
                        break;

                    case '\f':
                        json.Append('\\');
                        json.Append('f');
                        break;

                    case '\n':
                        json.Append('\\');
                        json.Append('n');
                        break;

                    case '\r':
                        json.Append('\\');
                        json.Append('r');
                        break;

                    case '\t':
                        json.Append('\\');
                        json.Append('t');
                        break;

                    default:
                        json.Append(@char);
                        break;

                }
            }
        }

        private static void Quote(StringBuilder json)
        {
            json.Append('"');
        }
    }
}