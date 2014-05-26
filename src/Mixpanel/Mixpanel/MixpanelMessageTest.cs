using System;
using System.Collections.Generic;

namespace Mixpanel
{
    /// <summary>
    /// Can be used for debugging creating process of Mixapnel message.
    /// </summary>
    public class MixpanelMessageTest
    {
        /// <summary>
        /// Message data that was constructed from user input.
        /// </summary>
        public IDictionary<string, object> Data { get; set; }

        /// <summary>
        /// <see cref="Data"/> serialized to JSON.
        /// </summary>
        public string Json { get; set; }

        /// <summary>
        /// <see cref="Json"/> converted to Base64 string.
        /// </summary>
        public string Base64 { get; set; }

        /// <summary>
        /// Contains the exception if some error occurs during the process of creating a message.
        /// </summary>
        public Exception Exception { get; set; }
    }
}