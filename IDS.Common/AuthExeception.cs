using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class AuthExeception :Exception
    {
        public AuthExeception(string msg):base(msg) {
        }
    }
}
