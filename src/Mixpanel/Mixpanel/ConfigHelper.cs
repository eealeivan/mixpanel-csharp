using System;

namespace Mixpanel
{
    internal static class ConfigHelper
    {
        public static Func<object, string> GetSerializeJsonFn(MixpanelConfig config)
        {
            if (config != null && config.SerializeJsonFn != null)
                return config.SerializeJsonFn;

            if (MixpanelGlobalConfig.SerializeJsonFn != null)
                return MixpanelGlobalConfig.SerializeJsonFn;

            return new DefaultJsonSerializer().Serialize;
        }

        public static Func<string, string, bool> GetHttpPostFn(MixpanelConfig config)
        {
            if (config != null && config.HttpPostFn != null)
                return config.HttpPostFn;

            if (MixpanelGlobalConfig.HttpPostFn != null)
                return MixpanelGlobalConfig.HttpPostFn;

            return new DefaultHttpClient().Post;
        }
        
        public static Action<string, Exception> GetErrorLogFn(MixpanelConfig config)
        {
            if (config != null && config.ErrorLogFn != null)
                return config.ErrorLogFn;

            if (MixpanelGlobalConfig.ErrorLogFn != null)
                return MixpanelGlobalConfig.ErrorLogFn;

            return null;
        }
    }
}