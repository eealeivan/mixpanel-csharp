using System;
using System.Runtime.Serialization;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that some required Mixpanel property is null or empty.
    /// </summary>
    public class MixpanelRequiredPropertyNullOrEmptyException : Exception
    {
        public MixpanelRequiredPropertyNullOrEmptyException()
        {
        }

        public MixpanelRequiredPropertyNullOrEmptyException(string message)
            : base(message)
        {
        }

        public MixpanelRequiredPropertyNullOrEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MixpanelRequiredPropertyNullOrEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}