using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mixpanel
{
    internal sealed class BatchMessageWrapper
    {
        public List<List<IDictionary<string, object>>> TrackMessages { get; set; }
        public List<List<IDictionary<string, object>>> EngageMessages { get; set; }

        internal const int MaxBatchSize = 50;

        public BatchMessageWrapper(IEnumerable<MixpanelMessage> messages)
        {
            if (messages == null)
            {
                return;
            }
           
            foreach (var message in messages)
            {
                if (message == null || message.Data == null || message.Kind == MessageKind.Batch)
                {
                    continue;
                }

                bool isTrackMessage =
                    message.Kind == MessageKind.Track || message.Kind == MessageKind.Alias;
                if (isTrackMessage)
                {
                    AddTrackMessage(message);
                }
                else
                {
                    AddEngageMessage(message);
                }
            }
        }

        private void AddTrackMessage(MixpanelMessage message)
        {
            Debug.Assert(message != null);

            if (TrackMessages == null)
            {
                TrackMessages = new List<List<IDictionary<string, object>>>();
            }

            AddBatchMessage(TrackMessages, message);
        }

        private void AddEngageMessage(MixpanelMessage message)
        {
            Debug.Assert(message != null);

            if (EngageMessages == null)
            {
                EngageMessages = new List<List<IDictionary<string, object>>>();
            }

            AddBatchMessage(EngageMessages, message);
        }

        private void AddBatchMessage(List<List<IDictionary<string, object>>> list, MixpanelMessage message)
        {
            Debug.Assert(list != null);
            Debug.Assert(message != null);

            var lastInnerList = list.LastOrDefault();
            bool newInnerListNeeded = lastInnerList == null || lastInnerList.Count >= MaxBatchSize;
            if (newInnerListNeeded)
            {
                var newInnerList = new List<IDictionary<string, object>> { message.Data };
                list.Add(newInnerList);
            }
            else
            {
                lastInnerList.Add(message.Data);
            }
        }
    }
}