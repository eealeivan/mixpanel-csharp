using System;

namespace Mixpanel
{
    /// <summary>
    /// If specified, then value of <see cref="Name"/> will be used as a property name,
    /// instead of target property name.
    /// </summary>
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
