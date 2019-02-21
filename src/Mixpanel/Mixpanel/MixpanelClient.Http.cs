using System;
using System.Text;
using System.Threading.Tasks;
using Mixpanel.Exceptions;
using Mixpanel.MessageBuilders;

namespace Mixpanel
{
    public sealed partial class MixpanelClient
    {
        internal const string UrlFormat = "https://api.mixpanel.com/{0}";
        internal const string EndpointTrack = "track";
        internal const string EndpointEngage = "engage";

        private string GetEndpoint(MixpanelMessageEndpoint endpoint)
        {
            switch (endpoint)
            {
                case MixpanelMessageEndpoint.Track:
                    return EndpointTrack;
                case MixpanelMessageEndpoint.Engage:
                    return EndpointEngage;
                default:
                    throw new ArgumentOutOfRangeException(nameof(endpoint));
            }
        }

        private string GenerateUrl(MixpanelMessageEndpoint endpoint)
        {
            string url = string.Format(UrlFormat, GetEndpoint(endpoint));

            MixpanelIpAddressHandling ipAddressHandling = ConfigHelper.GetIpAddressHandling(config);
            switch (ipAddressHandling)
            {
                case MixpanelIpAddressHandling.UseRequestIp:
                    url += "?ip=1";
                    break;
                case MixpanelIpAddressHandling.IgnoreRequestIp:
                    url += "?ip=0";
                    break;
            }

            return url;
        }

        private string ToJson(object obj)
        {
            return ConfigHelper.GetSerializeJsonFn(config)(obj);
        }

        private string ToBase64(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private string ToMixpanelMessageFormat(string base64)
        {
            return "data=" + base64;
        }

        private void ThrowIfJsonNotConfigured()
        {
#if !JSON
            if (!ConfigHelper.SerializeJsonFnSet(config))
            {
                throw new MixpanelConfigurationException(
                    "There is no default JSON serializer in this build of Mixpanel C#. " +
                    "Please use configuration to set it. " +
                    "JSON.NET example: MixpanelConfig.Global.SerializeJsonFn = JsonConvert.SerializeObject;");
            }
#endif
        }

        private string GetMessageBody(Func<MessageBuildResult> messageBuildResultFn, MessageKind messageKind)
        {
            ThrowIfJsonNotConfigured();

            MessageBuildResult messageBuildResult;
            try
            {
                messageBuildResult = messageBuildResultFn();
            }
            catch (Exception e)
            {
                LogError($"Error building message for {messageKind}.", e);
                return null;
            }

            if (!messageBuildResult.Success)
            {
                LogError(
                    $"Cannot build message for {messageKind}.",
                    new Exception(messageBuildResult.Error));
                return null;
            }

            return GetMessageBody(messageBuildResult.Message);
        }

        private string GetMessageBody(Func<BatchMessageBuildResult> getBatchMessageBuildResultFn)
        {
            ThrowIfJsonNotConfigured();

            BatchMessageBuildResult batchMessageBuildResult;
            try
            {
                batchMessageBuildResult = getBatchMessageBuildResultFn();
            }
            catch (Exception e)
            {
                LogError($"Error building message for {MessageKind.Batch}.", e);
                return null;
            }

            return GetMessageBody(batchMessageBuildResult.Message);
        }

        private string GetMessageBody(object messageData)
        {
            string json;
            try
            {
                json = ToJson(messageData);
            }
            catch (Exception e)
            {
                LogError("Error serializing message to JSON.", e);
                return null;
            }

            string base64Message;
            try
            {
                base64Message = "data=" + ToBase64(json);
            }
            catch (Exception e)
            {
                LogError("Error converting message JSON to base64.", e);
                return null;
            }

            return base64Message;
        }

        private bool HttpPost(MixpanelMessageEndpoint endpoint, string messageBody)
        {
            string url = GenerateUrl(endpoint);
            try
            {
                var httpPostFn = ConfigHelper.GetHttpPostFn(config);
                return httpPostFn(url, messageBody);
            }
            catch (Exception e)
            {
                LogError($"POST fails to '{url}' with data '{messageBody}'.", e);
            }

            return false;
        }

        private async Task<bool> HttpPostAsync(MixpanelMessageEndpoint endpoint, string messageBody)
        {
            string url = GenerateUrl(endpoint);
            try
            {
                var httpPostFn = ConfigHelper.GetAsyncHttpPostFn(config);
                return await httpPostFn(url, messageBody).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LogError($"POST fails to '{url}' with data '{messageBody}'.", e);
            }

            return await Task.FromResult(false).ConfigureAwait(false);
        }

        private bool SendMessageInternal(
            MessageKind messageKind,
            MixpanelMessageEndpoint endpoint,
            Func<MessageBuildResult> getMessageBuildResultFn)
        {
            string messageBody = GetMessageBody(getMessageBuildResultFn, messageKind);
            if (messageBody == null)
            {
                return false;
            }

            return HttpPost(endpoint, messageBody);
        }

        private bool SendMessageInternal(
            MixpanelMessageEndpoint endpoint,
            Func<BatchMessageBuildResult> getBatchMessageBuildResultFn)
        {
            string messageBody = GetMessageBody(getBatchMessageBuildResultFn);
            if (messageBody == null)
            {
                return false;
            }

            return HttpPost(endpoint, messageBody);
        }

        private async Task<bool> SendMessageInternalAsync(
            MessageKind messageKind,
            MixpanelMessageEndpoint endpoint,
            Func<MessageBuildResult> getMessageBuildResultFn)
        {
            string messageBody = GetMessageBody(getMessageBuildResultFn, messageKind);
            if (messageBody == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            return await HttpPostAsync(endpoint, messageBody).ConfigureAwait(false);
        }

        private async Task<bool> SendMessageInternalAsync(
            MixpanelMessageEndpoint endpoint,
            Func<BatchMessageBuildResult> getBatchMessageBuildResultFn)
        {
            string messageBody = GetMessageBody(getBatchMessageBuildResultFn);
            if (messageBody == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            return await HttpPostAsync(endpoint, messageBody).ConfigureAwait(false);
        }

        private bool SendMessageInternal(
            MixpanelMessageEndpoint endpoint,
            string messageJson)
        {
            string messageBody = ToMixpanelMessageFormat(ToBase64(messageJson));
            if (messageBody == null)
            {
                return false;
            }

            return HttpPost(endpoint, messageBody);
        }

        private async Task<bool> SendMessageInternalAsync(
            MixpanelMessageEndpoint endpoint, 
            string messageJson)
        {
            string messageBody = ToMixpanelMessageFormat(ToBase64(messageJson));
            if (messageBody == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            return await HttpPostAsync(endpoint, messageBody).ConfigureAwait(false);
        }
    }
}