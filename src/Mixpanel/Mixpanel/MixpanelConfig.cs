using System;

namespace Mixpanel
{
    public static class MixpanelGlobalConfig
    {
        public static Func<object, string> SerializeJsonFn { get; set; }
        public static Func<string, string, bool> HttpPostFn { get; set; }
        public static PropertyNameFormat PropertyNameFormat { get; set; }

        public static void Reset()
        {
            SerializeJsonFn = null;
            HttpPostFn = null;
            PropertyNameFormat = PropertyNameFormat.None;
        }
    }

    public class MixpanelConfig
    {
        public Func<object, string> SerializeJsonFn { get; set; }
        public Func<string, string, bool> HttpPostFn { get; set; }
        public PropertyNameFormat PropertyNameFormat { get; set; }
    }

    public enum PropertyNameFormat
    {
        None,
        SentenceTitleCase,
        SentenseCapitilized,
        SentenceLowerCase
    }
}