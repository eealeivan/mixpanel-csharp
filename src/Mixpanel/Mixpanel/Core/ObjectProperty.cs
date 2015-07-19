using System.Diagnostics;

namespace Mixpanel.Core
{
    [DebuggerDisplay("{PropertyName} - {Value}")]
    internal struct ObjectProperty
    {
        public string PropertyName { get; set; }
        public PropertyNameSource PropertyNameSource { get; set; }
        public object Value { get; set; }

        public ObjectProperty(
            string propertyName, PropertyNameSource propertyNameSource, object value)
            : this()
        {
            PropertyName = propertyName;
            PropertyNameSource = propertyNameSource;
            Value = value;
        }
    }
}