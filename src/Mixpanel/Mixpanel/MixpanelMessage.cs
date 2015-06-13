using System.Collections.Generic;

namespace Mixpanel
{
    public sealed class MixpanelMessage
    {
        public MessageKind Kind { get; set; }
        public IDictionary<string, object> Data { get; set; }
    }
}