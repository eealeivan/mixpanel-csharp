using System;
using System.Runtime.Serialization;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that some required Mixpanel property is null or empty.
    /// </summary>
    public class MixpanelPropertyNullOrEmptyException : Exception
    {
        public MixpanelPropertyNullOrEmptyException()
        {
        }

        public MixpanelPropertyNullOrEmptyException(string message)
            : base(message)
        {
        }

        public MixpanelPropertyNullOrEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MixpanelPropertyNullOrEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}