using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public abstract class AbstractCache<K, V> : ICache<K, V>
    {
        public abstract V Get(K key);

        public abstract V Set(K key, V val);
    }
}
