#if NET35
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Mixpanel.Misc
{
    /// <summary>
    /// Used for Net 3.5 to substitute ConcurrentDictionary.
    /// </summary>
    internal class ThreadSafeCache<TKey, TValue>
    {
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _innerCache = new Dictionary<TKey, TValue>();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFn)
        {
            Debug.Assert(valueFn != null);

            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                TValue value;
                if (_innerCache.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    _cacheLock.EnterWriteLock();
                    value = valueFn(key);
                    try
                    {
                        _innerCache[key] = value;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                    return value;
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        ~ThreadSafeCache()
        {
            if (_cacheLock != null) _cacheLock.Dispose();
        }
    }
}
#endif