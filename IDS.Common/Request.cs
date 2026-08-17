using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDS.Common
{
    public class RequestData<T>
    {
        public T data { set; get; }
        public static bool isRequest(RequestData<T> r)
        {
            if (r == null || r.data == null)
                return false;
            if (r.data is List<T>&& (r.data as List<T>).Count==0)
             return false;
            if (r.data is string && string.IsNullOrWhiteSpace(r.data as string))
                return false;
            return true;
        }

        public static RequestData<T> create(T e)
        {
            RequestData<T> request = new RequestData<T>();
            request.data=e;
            return request;
        }
    }
}
