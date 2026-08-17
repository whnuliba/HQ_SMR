using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common.Utils
{
    public class EnumUtil
    {
        public static List<EnumItem> GetEnumList(Type enumType)
        {
            List<EnumItem> rows = new List<EnumItem>();
            foreach (int e in Enum.GetValues(enumType))
            {
                string eCode = Enum.GetName(enumType, e);
                string eValue = e.ToString();//获取值
                rows.Add(new EnumItem()
                {
                    Code = eCode,
                    Value = Convert.ToInt32(eValue),
                    Description = GetEnumDescription((Enum)Enum.Parse(enumType, eCode))
                });
            }

            return rows;
        }

        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }
    }
}
