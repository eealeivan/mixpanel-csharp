using System;

namespace Mixpanel
{
    public class MixpanelConfig
    {
        public Func<object, string> SerializeJsonFn { get; set; }
        public Func<string, string, bool> HttpPostFn { get; set; }
        public Action<string, Exception> ErrorLogFn { get; set; } 
        public MixpanelPropertyNameFormat MixpanelPropertyNameFormat { get; set; }

        public static MixpanelConfig Global { get; private set; }

        static MixpanelConfig()
        {
            Global = new MixpanelConfig();
        }

        public void Reset()
        {
            SerializeJsonFn = null;
            HttpPostFn = null;
            ErrorLogFn = null;
            MixpanelPropertyNameFormat = MixpanelPropertyNameFormat.None;
        }
    }
}