using System;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that some required Mixpanel property is null or empty.
    /// </summary>
    public class MixpanelRequiredPropertyNullOrEmptyException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="MixpanelRequiredPropertyNullOrEmptyException"/>.
        /// </summary>
        internal MixpanelRequiredPropertyNullOrEmptyException()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelRequiredPropertyNullOrEmptyException"/>.
        /// </summary>
        internal MixpanelRequiredPropertyNullOrEmptyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelRequiredPropertyNullOrEmptyException"/>.
        /// </summary>
        internal MixpanelRequiredPropertyNullOrEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}