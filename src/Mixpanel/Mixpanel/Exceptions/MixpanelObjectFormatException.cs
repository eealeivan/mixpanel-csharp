using System;
using System.Runtime.Serialization;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that object needed to be sent to Mixpanel has wrong format.
    /// Usually it's because some required properties are missing. 
    /// </summary>
    public class MixpanelObjectFormatException : Exception
    {
        public MixpanelObjectFormatException()
        {
        }

        public MixpanelObjectFormatException(string message)
            : base(message)
        {
        }

        public MixpanelObjectFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MixpanelObjectFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
