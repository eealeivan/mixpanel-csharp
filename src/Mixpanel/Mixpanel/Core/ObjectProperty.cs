using System.Diagnostics;

namespace Mixpanel.Core
{
    [DebuggerDisplay("{PropertyName} - {Value}")]
    internal struct ObjectProperty
    {
        public string PropertyName { get; private set; }
        public PropertyNameSource PropertyNameSource { get; private set; }
        public object Value { get; private set; }

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