using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common.Utils
{
    public class ModUtils
    {
        public static string GetModStringValue(string str,int size=12) {
            if(string.IsNullOrWhiteSpace(str))
               return "";
            if(!long.TryParse(str,out long m)){
                return "01";
            }
            long mod = m % size;
            if(mod==0)
                mod = 12;
            return $"{mod}".PadLeft(2, '0');
        }

        public static int GetModValue(string str, int size = 12)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            int mod = Math.Abs(str.GetHashCode()) % size;
            if (mod == 0)
                mod = 12;
            return mod;
        }
        public static string GetModStringValue(long str, int size = 12)
        {
            if (str==0)
                return "";
            long mod = str % size;
            if (mod == 0)
                mod = 12;
            return $"{mod}".PadLeft(2, '0');
        }

        public static int GetModValue(long str, int size = 12)
        {
            if (str == 0)
                return 0;
            int mod = (int)str % size;
            if (mod == 0)
                mod = 12;
            return mod;
        }
    }
}
