using System.Collections.Generic;

namespace Mixpanel.MessageBuilders
{
    internal sealed class MessageBuildResult
    {
        public bool Success { get; }
        public IDictionary<string, object> Message { get; }
        public string Error { get; }

        public MessageBuildResult(bool success, IDictionary<string, object> message, string error)
        {
            Success = success;
            Message = message;
            Error = error;
        }

        public static MessageBuildResult CreateSuccess(IDictionary<string, object> message)
        {
            return new MessageBuildResult(true, message, null);
        }

        public static MessageBuildResult CreateFail(string error, string details = null)
        {
            return new MessageBuildResult(
                false, 
                null, 
                string.IsNullOrWhiteSpace(details) ? error : error + " " + details);
        }
    }
}