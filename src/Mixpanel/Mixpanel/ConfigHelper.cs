using System;
using System.Threading.Tasks;
using Mixpanel.Json;

namespace Mixpanel
{
    internal static class ConfigHelper
    {
        public static Func<object, string> GetSerializeJsonFn(MixpanelConfig config)
        {
            if (config != null && config.SerializeJsonFn != null)
                return config.SerializeJsonFn;

            if (MixpanelConfig.Global.SerializeJsonFn != null)
                return MixpanelConfig.Global.SerializeJsonFn;

            return MixpanelJsonSerializer.Serialize;
        }

        public static Func<string, string, Task<bool>> GetHttpPostAsyncFn(MixpanelConfig config)
        {
            if (config != null && config.AsyncHttpPostFn != null)
                return config.AsyncHttpPostFn;

            if (MixpanelConfig.Global.AsyncHttpPostFn != null)
                return MixpanelConfig.Global.AsyncHttpPostFn;

            return new DefaultHttpClient().PostAsync;
        }
        
        public static Action<string, Exception> GetErrorLogFn(MixpanelConfig config)
        {
            if (config != null && config.ErrorLogFn != null)
                return config.ErrorLogFn;

            if (MixpanelConfig.Global.ErrorLogFn != null)
                return MixpanelConfig.Global.ErrorLogFn;

            return null;
        }

        public static MixpanelPropertyNameFormat GetMixpanelPropertyNameFormat(MixpanelConfig config)
        {
            if (config != null && config.MixpanelPropertyNameFormat != null)
            {
                return config.MixpanelPropertyNameFormat.Value;
            }

            if (MixpanelConfig.Global.MixpanelPropertyNameFormat != null)
            {
                return MixpanelConfig.Global.MixpanelPropertyNameFormat.Value;
            }

            return MixpanelPropertyNameFormat.None;
        }

        public static MixpanelIpAddressHandling GetIpAddressHandling(MixpanelConfig config)
        {
            if (config != null && config.IpAddressHandling != null)
            {
                return config.IpAddressHandling.Value;
            }

            if (MixpanelConfig.Global.IpAddressHandling != null)
            {
                return MixpanelConfig.Global.IpAddressHandling.Value;
            }

            return MixpanelIpAddressHandling.None;
        }

        public static MixpanelDataResidencyHandling GetDataResidencyHandling(MixpanelConfig config)
        {
            if (config != null && config.DataResidencyHandling != null)
            {
                return config.DataResidencyHandling.Value;
            }

            if (MixpanelConfig.Global.DataResidencyHandling != null)
            {
                return MixpanelConfig.Global.DataResidencyHandling.Value;
            }

            return MixpanelDataResidencyHandling.Default;
        }
    }
}