using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class Assert
    {
        public Assert()
        {
        }

        public static void notNull(Object arg, String message)
        {
            if (arg == null)
            {
                throw new BussinessException(message);
            }
        }

        public static void notEmpty(Object arg, String message)
        {
            if (arg == null)
            {
                throw new BussinessException(message);
            }
            else if (arg is String && string.IsNullOrWhiteSpace(arg.ToString())) {
                throw new BussinessException(message);
            }
        }

        public static void isNull(Object arg, String message)
        {
            if (arg != null)
            {
                throw new BussinessException(message);
            }
        }

        public static void notNumber(Object arg, String message)
        {

            if (arg == null)
            {
                throw new BussinessException(message);
            }
            bool isNumeric = int.TryParse(arg.ToString(), out int result);

            if (isNumeric)
            {
                throw new BussinessException(message);
            }
            
        }

        public static void isNumber(Object arg, String message)
        {
            if (arg == null)
            {
                throw new BussinessException(message);
            }
            bool isNumeric = int.TryParse(arg.ToString(), out int result);

            if (!isNumeric)
            {
                throw new BussinessException(message);
            }
        }
    }
}
