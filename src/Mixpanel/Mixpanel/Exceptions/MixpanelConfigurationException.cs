using System;

namespace Mixpanel.Exceptions
{
    /// <summary>
    /// Exception indicates that Mixpanel is not properly configured. 
    /// </summary>
    public class MixpanelConfigurationException : Exception
    {
        internal MixpanelConfigurationException(string message) : base(message)
        {
        }
    }
}