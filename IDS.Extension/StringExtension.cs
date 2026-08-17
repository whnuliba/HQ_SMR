using System.Runtime.CompilerServices;

namespace IDS.Extension
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this String str) {
            if(String.IsNullOrWhiteSpace(str)) return true;
            return false;    
        }

    }
}
