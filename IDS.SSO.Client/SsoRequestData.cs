using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.SSO.Client
{
    public class SsoRequestData<T> 
    {
        public T data { set; get; }
    }

    public enum ResponseStatus
    {
        SUCCESS = 200,
        INSUFFICIENT_PERMISSIONS = 401,
        ERROR = 500
    }
}
