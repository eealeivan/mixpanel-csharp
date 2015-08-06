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
        /// <summary>
        /// Creates an instance of <see cref="MixpanelObjectStructureException"/>.
        /// </summary>
        public MixpanelObjectStructureException()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelObjectStructureException"/>.
        /// </summary>
        public MixpanelObjectStructureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelObjectStructureException"/>.
        /// </summary>
        public MixpanelObjectStructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelObjectStructureException"/>.
        /// </summary>
        protected MixpanelObjectStructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
