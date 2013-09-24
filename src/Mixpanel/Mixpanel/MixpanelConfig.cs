using System;

namespace Mixpanel
{
    public static class MixpanelConfig
    {
        public static Func<object, string> JsonSerializer { get; set; }
        public static bool ConvertPropertyNamesToSentenceCase { get; set; }
        public static bool ConvertPropertyNamesToTitleCase { get; set; }
    }
}