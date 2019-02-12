using System;
using System.Collections.Generic;

namespace Mixpanel
{
    /// <summary>
    /// Can be used to check all steps of creating a batch mixpanel messages for
    /// MixpanelClient 'Send' and 'SendAsync' methods.
    /// </summary>
    public sealed class MixpanelBatchMessageTest
    {
        /// <summary>
        /// Message data that was constructed from user input.
        /// </summary>
        public IList<IDictionary<string, object>> Data { get; internal set; }

        /// <summary>
        /// <see cref="Data"/> serialized to JSON.
        /// </summary>
        public string Json { get; internal set; }

        /// <summary>
        /// <see cref="Json"/> converted to Base64 string.
        /// </summary>
        public string Base64 { get; internal set; }

        /// <summary>
        /// Contains the exception if some error occurs during the process of creating a message.
        /// </summary>
        public Exception Exception { get; internal set; }
    }
}