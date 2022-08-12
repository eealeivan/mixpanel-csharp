using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Mixpanel.Extensibility;

namespace Mixpanel.Json
{
    internal static class MixpanelJsonSerializerOptimized
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

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
            var json = StringBuilderCache.Acquire();

            Object(json, message);

            return StringBuilderCache.GetStringAndRelease(json);
        }

        private static string SerializeMessages(IDictionary<string, object>[] messages)
        {
            var json = StringBuilderCache.Acquire();

            Array(json, messages);

            return StringBuilderCache.GetStringAndRelease(json);
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
            json.Append('"');
            json.Append(name);
            json.Append('"');
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
                    json.Append('"');
                    Escape(json, charValue.ToString());
                    json.Append('"');
                    break;

                case string stringValue:
                    json.Append('"');
                    Escape(json, stringValue);
                    json.Append('"');
                    break;

                case byte byteValue:
                    json.Append(byteValue.ToString(Invariant));
                    break;

                case sbyte sbyteValue:
                    json.Append(sbyteValue.ToString(Invariant));
                    break;

                case short shortValue:
                    json.Append(shortValue.ToString(Invariant));
                    break;

                case ushort ushortValue:
                    json.Append(ushortValue.ToString(Invariant));
                    break;

                case int intValue:
                    json.Append(intValue.ToString(Invariant));
                    break;

                case uint uintValue:
                    json.Append(uintValue.ToString(Invariant));
                    break;

                case long longValue:
                    json.Append(longValue.ToString(Invariant));
                    break;

                case ulong ulongValue:
                    json.Append(ulongValue.ToString(Invariant));
                    break;

                case float floatValue:
                    json.Append(floatValue.ToString(Invariant));
                    break;

                case double doubleValue:
                    json.Append(doubleValue.ToString(Invariant));
                    break;

                case decimal decimalValue:
                    json.Append(decimalValue.ToString(Invariant));
                    break;

                case Guid guidValue:
                    json.Append('"');
                    json.Append(guidValue.ToString());
                    json.Append('"');
                    break;

                case TimeSpan timeSpanValue:
                    json.Append('"');
                    json.Append(timeSpanValue.ToString("c", Invariant));
                    json.Append('"');
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
                        json.Append('"');
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
    }
}