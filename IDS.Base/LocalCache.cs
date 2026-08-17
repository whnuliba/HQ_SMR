using IDS.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public class LocalCache<K, V> : ICache<K, V> where V : class where K : class
    {

        private  static ConcurrentDictionary<K, V> cache =new  ConcurrentDictionary<K, V>();
        private static object obj = new object();
        private LocalCache() { }
        private static volatile LocalCache<K, V> localCache;
        public static LocalCache<K, V> GetInstance()
        {
            if (null == localCache)
            {
                
                lock(obj) {
                if(null == localCache) {
                    localCache = new LocalCache<K, V>();
            }
}
        }
        return localCache;
    }
        public V Get(K key)
        {
            if (!cache.ContainsKey(key))
                return null;
            return cache[key];
        }

        public V Set(K key, V val)
        {
            cache.AddOrUpdate(key, val, (k, v) => {
                return v;
            });
            return val;
        }
    }
}
