using System;
using System.Runtime.Serialization;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that Mixpanel object (track, engage) has wrong structure.
    /// Usually it's because some required properties are missing. 
    /// </summary>
    public class MixpanelObjectStructureException : Exception
    {
        public MixpanelObjectStructureException()
        {
        }

        public MixpanelObjectStructureException(string message)
            : base(message)
        {
        }

        public MixpanelObjectStructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MixpanelObjectStructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
