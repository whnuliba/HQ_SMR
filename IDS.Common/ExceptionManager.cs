using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class ExceptionManager
    {
        public static void ThrowException(Exception ex) {
            throw new BussinessException(ex);
        }
    }
}
