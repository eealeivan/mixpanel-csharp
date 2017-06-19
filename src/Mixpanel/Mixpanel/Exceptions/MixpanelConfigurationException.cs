using System;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that Mixpanel is not proerly configured. 
    /// The most common case is missing JSON or HTTP functionality.
    /// </summary>
    public class MixpanelConfigurationException : Exception
    {
        internal MixpanelConfigurationException(string message) : base(message)
        {
        }
    }
}