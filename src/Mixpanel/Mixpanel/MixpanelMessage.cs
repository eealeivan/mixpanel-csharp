using System.Collections.Generic;

namespace Mixpanel
{
    /// <summary>
    /// Used as a return type for Get*Message methods. <see cref="Data"/> property contains generated 
    /// message that is ready to be serialized to JSON. This class can be passed as argument
    /// to Send(Async) method.
    /// </summary>
    public sealed class MixpanelMessage
    {
        /// <summary>
        /// Kind (type) of the message.
        /// </summary>
        public MessageKind Kind { get; set; }

        /// <summary>
        /// Generated message data with correct structure ready to be serialized to JSON.
        /// </summary>
        public IDictionary<string, object> Data { get; set; }
    }
}