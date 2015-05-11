#if (NET40 || NET35)
using System;
using System.Linq;
using System.Reflection;

namespace Mixpanel.Misc
{
    internal static class ReflectionHelper
    {
        public static T GetCustomAttribute<T>(this MemberInfo element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            var attribute = element
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .FirstOrDefault();
            return attribute;
        }
    }
}
#endif