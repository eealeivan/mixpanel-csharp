namespace Mixpanel.Core
{
    internal struct ObjectProperty
    {
        public PropertyNameSource PropertyNameSource { get; set; }
        public object Value { get; set; }

        public ObjectProperty(PropertyNameSource propertyNameSource, object value)
            : this()
        {
            PropertyNameSource = propertyNameSource;
            Value = value;
        }
    }
}