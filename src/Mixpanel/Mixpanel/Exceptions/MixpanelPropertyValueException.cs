using System;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that there was some problem with parsing property value.
    /// </summary>
    public class MixpanelPropertyValueException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="MixpanelPropertyValueException"/>.
        /// </summary>
        internal MixpanelPropertyValueException()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelPropertyValueException"/>.
        /// </summary>
        internal MixpanelPropertyValueException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelPropertyValueException"/>.
        /// </summary>
        internal MixpanelPropertyValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}