using System.Collections.Generic;
using System.Linq;
using Mixpanel.Extensibility;

namespace Mixpanel
{
    internal sealed class SendResultInternal
    {
        private bool success;
        private List<List<MixpanelMessage>> sentBatches;
        private List<List<MixpanelMessage>> failedBatches;

        public SendResultInternal()
        {
            success = true;
        }

        public void Update(bool batchSuccess, List<MixpanelMessage> mixpanelMessages)
        {
            success &= batchSuccess;

            if (batchSuccess)
            {
                if (sentBatches == null)
                {
                    sentBatches = new List<List<MixpanelMessage>>();
                }

                sentBatches.Add(mixpanelMessages);
            }
            else
            {
                if (failedBatches == null)
                {
                    failedBatches = new List<List<MixpanelMessage>>();
                }

                failedBatches.Add(mixpanelMessages);
            }
        }

        public SendResult ToRealSendResult()
        {
            var result = new SendResult { Success = success };

            if (sentBatches != null)
            {
                result.SentBatches = sentBatches
                    .Select(x => x.AsReadOnly())
                    .ToList()
                    .AsReadOnly();
            }

            if (failedBatches != null)
            {
                result.FailedBatches = failedBatches
                    .Select(x => x.AsReadOnly())
                    .ToList()
                    .AsReadOnly();
            }

            return result;
        }
    }
}