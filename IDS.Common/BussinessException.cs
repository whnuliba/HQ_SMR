using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class BussinessException : Exception
    {
        public BussinessException(string msg):base(msg) {

        }
        public BussinessException(Exception ex) : base(ex.InnerException!=null?ex.InnerException.Message : ex.Message) { }
    }
}
