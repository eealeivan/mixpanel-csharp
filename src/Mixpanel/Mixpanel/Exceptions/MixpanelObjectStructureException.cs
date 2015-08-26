using System;

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
        internal MixpanelObjectStructureException()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelObjectStructureException"/>.
        /// </summary>
        internal MixpanelObjectStructureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MixpanelObjectStructureException"/>.
        /// </summary>
        internal MixpanelObjectStructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
