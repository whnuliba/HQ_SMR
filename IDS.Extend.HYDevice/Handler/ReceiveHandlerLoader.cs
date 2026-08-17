using IDS.Device.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.Handler
{
    public class ReceiveHandlerLoader
    {
        /// <summary>
        /// 扫描指定命名空间下所有实现了 IReceiveHandler<T> 的类并实例化
        /// </summary>
        /// <typeparam name="T">泛型参数类型（如 string）</typeparam>
        /// <param name="namespaceName">要扫描的命名空间</param>
        /// <returns>实例列表</returns>
        public static List<IReceiveHandler> LoadHandlers(string namespaceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var handlers = new List<IReceiveHandler>();

            // 1. 获取所有类
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == namespaceName)
                .ToList();

            foreach (var type in types)
            {
                try
                {
                    // 2. 检查是否实现了 IReceiveHandler<T> 接口
                    //var interfaces = type.GetInterfaces();
                    //var targetInterface = interfaces.FirstOrDefault(i =>
                    //    i.IsGenericType &&
                    //    i.GetGenericTypeDefinition().Equals(typeof(IReceiveHandler))
                    //    //&& i.GetGenericArguments()[0].Equals(typeof(T))  // 泛型参数匹配
                    //);

                    //if (targetInterface == null)
                    //    continue;

                    // 3. 创建实例（要求有无参构造函数）
                    var instance = (IReceiveHandler)Activator.CreateInstance(type);
                    handlers.Add(instance);

                }
                catch (Exception ex)
                {
                }
            }

            return handlers;
        }

        /// <summary>
        /// 扫描并实例化所有实现了任意 IReceiveHandler<T> 的类（不限定泛型参数）
        /// </summary>
        public static List<object> LoadAllHandlers(string namespaceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var handlers = new List<object>();

            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == namespaceName)
                .ToList();

            foreach (var type in types)
            {
                try
                {
                    // 检查是否实现了 IReceiveHandler<> 接口
                    var interfaces = type.GetInterfaces();
                    var targetInterface = interfaces.FirstOrDefault(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IReceiveHandler)
                    );

                    if (targetInterface == null)
                        continue;

                    // 创建实例
                    var instance = Activator.CreateInstance(type);
                    handlers.Add(instance);

                    var genericArg = targetInterface.GetGenericArguments()[0];
                    //Console.WriteLine($"✅ 加载成功: {type.Name} (泛型参数: {genericArg.Name})");
                }
                catch (Exception ex)
                {
                   // Console.WriteLine($"❌ 加载失败 {type.Name}: {ex.Message}");
                }
            }

            return handlers;
        }
    }
}
