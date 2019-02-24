using System;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that Mixpanel was not able to build a message from provided data.
    /// Typically it happens when some required properties are not provided or have a wrong format.
    /// </summary>
    public class MixpanelMessageBuildException : Exception
    {
        internal MixpanelMessageBuildException(string message) : base(message)
        {
        }
    }
}