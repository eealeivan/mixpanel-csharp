using System;
using System.Runtime.Serialization;

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
        public MixpanelRequiredPropertyNullOrEmptyException()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelRequiredPropertyNullOrEmptyException"/>.
        /// </summary>
        public MixpanelRequiredPropertyNullOrEmptyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelRequiredPropertyNullOrEmptyException"/>.
        /// </summary>
        public MixpanelRequiredPropertyNullOrEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelRequiredPropertyNullOrEmptyException"/>.
        /// </summary>
        protected MixpanelRequiredPropertyNullOrEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}