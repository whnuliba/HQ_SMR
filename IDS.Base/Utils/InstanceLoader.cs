using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base.Utils
{
    public class InstanceLoader
    {
        public static List<T> CreateInstances<T>(Assembly assembly,string namespaceName)
        {
            // 1. 获取当前程序集
           // Assembly assembly = Assembly.GetExecutingAssembly();

            // 2. 筛选出指定命名空间下、非抽象、且实现了 T 接口（或继承了 T）的类
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                            && t.Namespace == namespaceName
                            && typeof(T).IsAssignableFrom(t))
                .ToList();

            List<T> instances = new List<T>();

            foreach (var type in types)
            {
                try
                {
                    // 3. 创建实例（要求有无参构造函数）
                    T instance = (T)Activator.CreateInstance(type);
                    instances.Add(instance);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"创建 {type.Name} 失败: {ex.Message}");
                }
            }

            return instances;
        }
    }
}
