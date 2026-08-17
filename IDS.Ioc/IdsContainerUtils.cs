using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Ioc
{
    public class IdsContainerUtils
    {
        private static ConcurrentDictionary<string, IdsIocNamed> _IdsIocNamed = new ConcurrentDictionary<string, IdsIocNamed>();
        public static void InitIocNamed(string name, Type type)
        {
            if (_IdsIocNamed.ContainsKey(name))
            {
                _IdsIocNamed[name] = new IdsIocNamed
                {
                    Name = name,
                    Type = type,
                };
                return;
            }
            var named = new IdsIocNamed
            {
                Name = name,
                Type = type,
            };
            _IdsIocNamed.AddOrUpdate(name, named, (k1, v1) => named);
        }
        public static object GetService(string key)
        {

            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("The request header must have ResourceNo Key");
            if (!_IdsIocNamed.ContainsKey(key))
                throw new Exception("The specified service is not registered");
            var named = _IdsIocNamed[key];
            var obj = ContainerUtils.AutofacServiceProvider.GetKeyedService(named.Type, named.Name);
            return obj;
        }
    }

    public class IdsIocNamed
    {
        public string? Name { get; set; }
        public Type Type { get; set; }
    }
}
