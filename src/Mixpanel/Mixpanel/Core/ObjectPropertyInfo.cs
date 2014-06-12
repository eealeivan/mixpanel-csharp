using System.Reflection;

namespace Mixpanel.Core
{
    internal struct ObjectPropertyInfo
    {
        public string PropertyName { get; set; }
        public PropertyNameSource PropertyNameSource { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public ObjectPropertyInfo(
            string propertyName, PropertyNameSource propertyNameSource, PropertyInfo propertyInfo)
            : this()
        {
            PropertyName = propertyName;
            PropertyNameSource = propertyNameSource;
            PropertyInfo = propertyInfo;
        }
    }
}