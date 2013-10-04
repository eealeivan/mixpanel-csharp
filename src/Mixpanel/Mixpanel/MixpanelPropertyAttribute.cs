using System;

namespace Mixpanel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MixpanelPropertyAttribute : Attribute
    {
        public readonly string Name;

        /// <summary>
        /// Creates an instance of <see cref="MixpanelPropertyAttribute"/> class.
        /// </summary>
        /// <param name="name">Alternative name for property that will be sent to Mixpanel.</param>
        public MixpanelPropertyAttribute(string name)
        {
            Name = name;
        }
    }
}
