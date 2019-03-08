using System.Diagnostics;

namespace Mixpanel.MessageProperties
{
    [DebuggerDisplay("{PropertyName} - {Value}")]
    internal sealed class ObjectProperty
    {
        public string PropertyName { get; }
        public PropertyNameSource PropertyNameSource { get; }
        public PropertyOrigin Origin { get; }
        public object Value { get; }

        public ObjectProperty(
            string propertyName, 
            PropertyNameSource propertyNameSource, 
            PropertyOrigin origin,
            object value)
        {
            PropertyName = propertyName;
            PropertyNameSource = propertyNameSource;
            Origin = origin;
            Value = value;
        }

        public static ObjectProperty Default(
            string propertyName, 
            PropertyOrigin origin, 
            object value = null)
        {
            return new ObjectProperty(propertyName, PropertyNameSource.Default, origin, value);
        }
    }
}