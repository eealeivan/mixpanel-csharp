
using System;
using System.Linq;

namespace Mixpanel.Misc
{
    public static class StringHelper
    {
        public static bool IsNullOrWhiteSpace(this String value)
        {
#if NET35
            return value == null || value.All(Char.IsWhiteSpace);
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }
    }
}
