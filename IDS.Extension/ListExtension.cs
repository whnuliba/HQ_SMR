using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extension
{
    public static class ListExtension
    {
        public static  List<List<T>> Partition<T>(this List<T> list,int length) {
            List<List<T>> values = new List<List<T>>();
            if (list.Count() <= length)
            {
                values.Add(list);
                return values;
            }
            for (int i = 0; i < list.Count; i+= length) {

                List<T> chunk = list.Skip(i).Take(length).ToList();
                values.Add(chunk);
            }
            return values;
        }
    }
}
