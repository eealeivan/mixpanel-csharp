using System;
using System.Text;
using Mixpanel.Exceptions;
using System.Threading.Tasks;

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

        private string GetMessageBody(Func<object> messageDataFn, MessageKind messageKind)
        {
#if !JSON
            if (!ConfigHelper.SerializeJsonFnSet(config))
            {
                throw new MixpanelConfigurationException(
                    "There is no default JSON serializer in this build of Mixpanel C#. Please use configuration to set it. JSON.NET example: MixpanelConfig.Global.SerializeJsonFn = JsonConvert.SerializeObject;");
            }
#endif

            object messageData;
            try
            {
                messageData = messageDataFn();
            }
            catch (Exception e)
            {
                LogError($"Error creating message data for {messageKind} message.", e);
                return null;
            }

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

            try
            {
                return "data=" + ToBase64(json);
            }
            catch (Exception e)
            {
                LogError("Error converting message JSON to base64.", e);
                return null;
            }
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
            Func<object> getMessageDataFn, MixpanelMessageEndpoint endpoint, MessageKind messageKind)
        {
            string messageBody = GetMessageBody(getMessageDataFn, messageKind);
            if (messageBody == null)
            {
                return false;
            }

            return HttpPost(endpoint, messageBody);
        }

        private bool SendMessageInternal(
            MixpanelMessageEndpoint endpoint, string messageJson)
        {
            string messageBody = ToMixpanelMessageFormat(ToBase64(messageJson));
            if (messageBody == null)
            {
                return false;
            }

            return HttpPost(endpoint, messageBody);
        }

        private async Task<bool> SendMessageInternalAsync(
            Func<object> getMessageDataFn, MixpanelMessageEndpoint endpoint, MessageKind messageKind)
        {
            string messageBody = GetMessageBody(getMessageDataFn, messageKind);
            if (messageBody == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            return await HttpPostAsync(endpoint, messageBody).ConfigureAwait(false);
        }

        private async Task<bool> SendMessageInternalAsync(
            MixpanelMessageEndpoint endpoint, string messageJson)
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