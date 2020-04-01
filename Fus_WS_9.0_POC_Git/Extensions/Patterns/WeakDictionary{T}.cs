using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Extensions.Patterns
{
    public class WeakDictionary<TKey, TValue> where TValue : class
    {
        private readonly Dictionary<TKey, WeakReference<TValue>> _dict = new Dictionary<TKey, WeakReference<TValue>>();

        public TValue GetValue(TKey key, Func<TKey, TValue> factory)
        {
            WeakReference<TValue> weakValue;
            TValue value;
            if (!_dict.TryGetValue(key, out weakValue) || !weakValue.TryGetTarget(out value))
            {
                value = factory(key);
                _dict[key] = new WeakReference<TValue>(value);
            }

            return value;
        }

        public int Clean()
        {
            var orphans = _dict.Where(kvp =>
            {
                TValue val;
                return !kvp.Value.TryGetTarget(out val);
            }).ToArray();

            foreach (var orphan in orphans)
                _dict.Remove(orphan.Key);

            return orphans.Length;
        }
    }

    public static class WeakDictionaryExtensions
    {
        public static int Clean<TKey, TValue>(this IDictionary<TKey, WeakReference<TValue>> cache) where TValue : class
        {
            var orphans = cache.Where(kvp =>
            {
                TValue val;
                return !kvp.Value.TryGetTarget(out val);
            }).ToArray();

            foreach (var orphan in orphans)
                cache.Remove(orphan.Key);

            return orphans.Length;
        }
    }
}
