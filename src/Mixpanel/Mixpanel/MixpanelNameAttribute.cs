using System;

namespace Mixpanel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MixpanelNameAttribute : Attribute
    {
        public readonly string Name;

        /// <summary>
        /// Creates an instance of <see cref="MixpanelNameAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// Alternative name for property that will be sent to Mixpanel.
        /// For special Mixpanel properties use values from <see cref="MixpanelProperty"/> class.
        /// </param>
        public MixpanelNameAttribute(string name)
        {
            Name = name;
        }
    }
}
