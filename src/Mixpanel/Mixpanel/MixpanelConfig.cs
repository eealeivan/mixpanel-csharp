using System;

namespace Mixpanel
{
    /// <summary>
    /// Provides properties for configuring custom behaviour of the library. You can use config by setting 
    /// values on MixpanelConfig.Global, or by passing an instance to <see cref="MixpanelClient"/>
    /// constructor (in this case MixpanelConfig.Global will be ignored). 
    /// </summary>
    public class MixpanelConfig
    {
        /// <summary>
        /// Gets or sets user defined JSON serialization function. Takes an object as a parameter and returns 
        /// serialized JSON a string.
        /// </summary>
        public Func<object, string> SerializeJsonFn { get; set; }

        /// <summary>
        /// Gets or sets user defined function that will make HTTP POST requests to mixpnael endpoints.
        /// Takes 2 parameters: url and form data. Returns true if call was successful, and false otherwise.
        /// </summary>
        public Func<string, string, bool> HttpPostFn { get; set; }

        /// <summary>
        /// Gets ot sets used defined function for retrievenig error logs. Takes 2 parameters: message and exception.
        /// </summary>
        public Action<string, Exception> ErrorLogFn { get; set; } 

        /// <summary>
        /// Gets or sets the format for mixpanel properties.
        /// </summary>
        public MixpanelPropertyNameFormat MixpanelPropertyNameFormat { get; set; }

        /// <summary>
        /// A global instance of the config.
        /// </summary>
        public static MixpanelConfig Global { get; private set; }

        static MixpanelConfig()
        {
            Global = new MixpanelConfig();
        }

        /// <summary>
        /// Resets all properties for current instance to it's default values.
        /// </summary>
        public void Reset()
        {
            SerializeJsonFn = null;
            HttpPostFn = null;
            ErrorLogFn = null;
            MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
        }
    }
}