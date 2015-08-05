using System.Collections.ObjectModel;

namespace Mixpanel
{
    /// <summary>
    /// Contains information about 'Send(Async)' method work result.
    /// </summary>
    public sealed class SendResult
    {
        /// <summary>
        /// True if all message batches were successfully sent. 
        /// False if at least one message batch failed.
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        /// A collection of successfully sent message batches.
        /// </summary>
        public ReadOnlyCollection<ReadOnlyCollection<MixpanelMessage>> SentBatches { get; internal set; }

        /// <summary>
        /// A collection of failed message batches.
        /// </summary>
        public ReadOnlyCollection<ReadOnlyCollection<MixpanelMessage>> FailedBatches { get; internal set; }
    }
}