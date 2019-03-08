using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mixpanel.Extensibility
{
    internal static class NetStandard11Extensions
    {
#if NETSTANDARD11
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
#endif
    }
}