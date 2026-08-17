using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class GlobalExceptionDictionary
    {
        private static Dictionary<string, Dictionary<string, object>> exceptionDictionary = new Dictionary<string, Dictionary<string, object>>();
        public static Dictionary<string, object> GetExceptionDictionary(string code)
        {
            if (exceptionDictionary.ContainsKey(code))
                return exceptionDictionary[code];
            return null;
        }

        public static string GetExceptionDictionary(string code, string ln)
        {
            if (exceptionDictionary.ContainsKey(code) && exceptionDictionary[code].ContainsKey(ln))
                return exceptionDictionary[code][ln].ToString();
            return null;
        }
        public static void SetExceptionDictionary(string code, Dictionary<string, object> exception)
        {
            if (exceptionDictionary.ContainsKey(code)) {
                exceptionDictionary[code] = exception;
                return;
            }
            exceptionDictionary.Add(code, exception);
        }

        public static void SetExceptionDictionaryAll(Dictionary<string, Dictionary<string, object>> exception)
        {
            foreach (var d in exception) {
                SetExceptionDictionary(d.Key, d.Value);
            }
        }
    }
}
