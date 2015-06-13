using System;
using System.Text;
using System.Threading.Tasks;

namespace Mixpanel
{
    public sealed partial class MixpanelClient
    {
        internal const string UrlFormat = "http://api.mixpanel.com/{0}";
        internal const string EndpointTrack = "track";
        internal const string EndpointEngage = "engage";

        private string GenerateUrl(string endpoint)
        {
            return string.Format(UrlFormat, endpoint);
        }

        private string ToJson(object obj)
        {
            return ConfigHelper.GetSerializeJsonFn(_config)(obj);
        }

        private string ToBase64(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private string GetFormData(Func<object> messageDataFn, MessageKind messageKind)
        {
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

        private bool SendMessageInternal(
            Func<object> getMessageDataFn, string endpoint, MessageKind messageKind)
        {
            string formData = GetFormData(getMessageDataFn, messageKind);
            if (formData == null)
            {
                return false;
            }

            string url = GenerateUrl(endpoint);
            try
            {
                var httpPostFn = ConfigHelper.GetHttpPostFn(_config);
                return httpPostFn(url, formData);
            }
            catch (Exception e)
            {
                LogError(string.Format("POST fails to '{0}' with data '{1}'", url, formData), e);
                return false;
            }
        }

#if !(NET40 || NET35)
        private async Task<bool> SendMessageInternalAsync(
            Func<object> getMessageDataFn, string endpoint, MessageKind messageKind)
        {
            string formData = GetFormData(getMessageDataFn, messageKind);
            if (formData == null)
            {
                return false;
            }

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