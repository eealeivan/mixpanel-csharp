using System;
using System.Text;

namespace Mixpanel.Extensibility
{
    /// <summary>
    /// Provides a cached reusable instance of <see cref="StringBuilder"/> per thread.
    /// It's an optimization that reduces the  number of instances constructed and collected.
    /// Inspired by: https://github.com/microsoft/referencesource/blob/master/mscorlib/system/text/stringbuildercache.cs
    /// </summary>
    internal sealed class StringBuilderCache
    {
        [ThreadStatic]
        private static StringBuilder cachedInstance;

        public static StringBuilder Acquire()
        {
            StringBuilder sb = cachedInstance;
            if (sb == null)
            {
                return new StringBuilder();
            }

            cachedInstance = null;
            sb.Clear();
            return sb;

        }

        public static void Release(StringBuilder sb)
        {
            cachedInstance = sb;
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            string result = sb.ToString();
            Release(sb);
            return result;
        }
    }
}