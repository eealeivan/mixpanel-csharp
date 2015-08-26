using System;

namespace Mixpanel.Exceptions
{
    public class MixpanelConfigurationException : Exception
    {
        internal MixpanelConfigurationException(string message) : base(message)
        {
        }
    }
}