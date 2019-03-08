using System.Collections.Generic;
using System.Linq;

namespace Mixpanel.MessageBuilders
{
    internal sealed class BatchMessageBuildResult
    {
        public IDictionary<string, object>[] Message { get; }

        public BatchMessageBuildResult(IEnumerable<MixpanelMessage> messages)
        {
            Message = messages.Select(message => message.Data).ToArray();
        }
    }
}