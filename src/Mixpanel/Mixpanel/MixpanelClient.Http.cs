using System;
using System.Text;
using Mixpanel.Exceptions;
#if !(NET40 || NET35)
using System.Threading.Tasks;
#endif

namespace Mixpanel
{
    public sealed partial class MixpanelClient
    {
        internal const string UrlFormat = "http://api.mixpanel.com/{0}";
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
                    throw new ArgumentOutOfRangeException("endpoint");
            }
        }

        private string GenerateUrl(MixpanelMessageEndpoint endpoint)
        {
            string url = string.Format(UrlFormat, GetEndpoint(endpoint));

            MixpanelIpAddressHandling ipAddressHandling = ConfigHelper.GetIpAddressHandling(_config);
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
            return ConfigHelper.GetSerializeJsonFn(_config)(obj);
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
#if (PORTABLE || PORTABLE40)
            if (!ConfigHelper.SerializeJsonFnSet(_config))
            {
                throw new MixpanelConfigurationException(
                    "There is no default JSON serializer in portable builds. Please use configuration to set it.");
            }
#endif

            object messageData;
            try
            {
                messageData = messageDataFn();
            }
            catch (Exception e)
            {
                LogError(string.Format("Error creating message data for {0} message", messageKind), e);
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
                var httpPostFn = ConfigHelper.GetHttpPostFn(_config);
                return httpPostFn(url, messageBody);
            }
            catch (Exception e)
            {
                LogError(string.Format("POST fails to '{0}' with data '{1}'", url, messageBody), e);
                return false;
            }
        }

        private bool SendMessageInternal(
            Func<object> getMessageDataFn, MixpanelMessageEndpoint endpoint, MessageKind messageKind)
        {
            string messageBody = GetMessageBody(getMessageDataFn, messageKind);
            if (messageBody == null)
            {
                return false;
            }

#if (PORTABLE || PORTABLE40)
            if (!ConfigHelper.HttpPostFnSet(_config))
            {
                throw new MixpanelConfigurationException(
                    "There is no default HTTP POST method in portable builds. Please use configuration to set it.");
            }
#endif

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

#if (PORTABLE || PORTABLE40)
            if (!ConfigHelper.HttpPostFnSet(_config))
            {
                throw new MixpanelConfigurationException(
                    "There is no default HTTP POST method in portable builds. Please use configuration to set it.");
            }
#endif

            return HttpPost(endpoint, messageBody);
        }

#if !(NET40 || NET35)
        private async Task<bool> SendMessageInternalAsync(
            Func<object> getMessageDataFn, MixpanelMessageEndpoint endpoint, MessageKind messageKind)
        {
            string formData = GetMessageBody(getMessageDataFn, messageKind);
            if (formData == null)
            {
                return false;
            }

#if (PORTABLE || PORTABLE40)
            if (!ConfigHelper.AsyncHttpPostFnSet(_config))
            {
                throw new MixpanelConfigurationException(
                    "There is no default async HTTP POST method in portable builds. Please use configuration to set it.");
            }
#endif


            string url = GenerateUrl(endpoint);
            try
            {
                var httpPostFn = ConfigHelper.GetAsyncHttpPostFn(_config);
                return await httpPostFn(url, formData);
            }
            catch (Exception e)
            {
                LogError(string.Format("POST fails to '{0}' with data '{1}'", url, formData), e);
                return false;
            }
        }
#endif
    }
}