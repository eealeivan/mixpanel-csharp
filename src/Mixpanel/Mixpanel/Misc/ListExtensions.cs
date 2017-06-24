using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mixpanel.Misc
{
    internal static class ListExtensions
    {
#if NETSTANDARD11
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
#endif
    }
}