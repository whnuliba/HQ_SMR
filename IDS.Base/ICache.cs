using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public interface ICache<K,V>
    {
        V Set(K key, V val);
        V Get(K key);
    }
}
