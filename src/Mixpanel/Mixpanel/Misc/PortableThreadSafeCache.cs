using System.Collections.Generic;
#if (PORTABLE || PORTABLE40)
using System;
using System.Diagnostics;

namespace Mixpanel.Misc
{
    internal class PortableThreadSafeCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _innerCache = new Dictionary<TKey, TValue>();
        private readonly object _cacheLock = new object();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFn)
        {
            Debug.Assert(valueFn != null);

            TValue value;
            lock (_cacheLock)
            {
                if (!_innerCache.TryGetValue(key, out value))
                {
                    value = valueFn(key);
                    _innerCache[key] = value;
                }
            }

            return value;
        }
    }
}
#endif