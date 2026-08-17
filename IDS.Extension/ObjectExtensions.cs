using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extension
{
    public static class ObjectExtensions
    {
        public static void CopyPropertiesTo<T>(this T source, T target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(target, prop.GetValue(source, null), null);
                }
            }
        }

        public static void CopyProperties( object source, object target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
           var targetType =  target.GetType();
           var sourceType = source.GetType();
           var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
           var targetProps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var drcPropDic = targetProps.ToDictionary(f=>f.Name,f=>f);

            foreach (var prop in sourceProps)
            {
                if (prop.CanWrite && drcPropDic.ContainsKey(prop.Name))
                {
                    drcPropDic[prop.Name].SetValue(target, prop.GetValue(source, null), null);
                }
            }
        }
    }
}
