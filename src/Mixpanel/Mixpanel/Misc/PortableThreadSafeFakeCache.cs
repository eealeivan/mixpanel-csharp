#if (PORTABLE || PORTABLE40)
using System;
using System.Diagnostics;

namespace Mixpanel.Misc
{
    internal class PortableThreadSafeFakeCache<TKey, TValue>
    {
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFn)
        {
            Debug.Assert(valueFn != null);

            return valueFn(key);
        }
    }
}
#endif