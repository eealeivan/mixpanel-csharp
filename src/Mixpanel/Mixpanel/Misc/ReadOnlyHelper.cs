#if (PORTABLE || PORTABLE40)
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mixpanel.Misc
{
    internal static class ReadOnlyHelper
    {
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
    }
}
#endif