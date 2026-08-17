using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common.Utils
{
    public class MyObjectUtils
    {
        public static string Serialize(object obj) {
            return JsonConvert.SerializeObject(obj);
        }


        public static List<ValueObject> GetEnumList(Type type)
        {
            var enums = EnumUtil.GetEnumList(type);

            List<ValueObject> result = new List<ValueObject>();
            try
            {
                foreach (var e in enums)
                {
                    ValueObject valueObject = new ValueObject
                    {
                        value = e.Value,
                        name = e.Description,
                    };
                    result.Add(valueObject);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new BussinessException(ex.Message);
            }
        }
    }
}
