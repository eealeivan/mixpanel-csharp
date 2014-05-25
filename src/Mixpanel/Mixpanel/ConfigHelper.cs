using System;

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

            return new DefaultJsonSerializer().Serialize;
        }

        public static Func<string, string, bool> GetHttpPostFn(MixpanelConfig config)
        {
            if (config != null && config.HttpPostFn != null)
                return config.HttpPostFn;

            if (MixpanelConfig.Global.HttpPostFn != null)
                return MixpanelConfig.Global.HttpPostFn;

            return new DefaultHttpClient().Post;
        }
        
        public static Action<string, Exception> GetErrorLogFn(MixpanelConfig config)
        {
            if (config != null && config.ErrorLogFn != null)
                return config.ErrorLogFn;

            if (MixpanelConfig.Global.ErrorLogFn != null)
                return MixpanelConfig.Global.ErrorLogFn;

            return null;
        }
    }
}