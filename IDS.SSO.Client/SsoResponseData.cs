using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.SSO.Client
{
    internal class SsoResponseData<T>
    {
        public T data { set; get; }
        public string status { set; get; }
        public int code { set; get; }

        public SsoResponseData() { }
        public static SsoResponseData<Object> error(Object o)
        {
            return new SsoResponseData<Object>(ResponseStatus.ERROR, o);
        }
        public SsoResponseData(ResponseStatus status, T data)
        {
            this.data = data;
            this.status = status.ToString();
            this.code = (int)status;
        }
        public SsoResponseData(int code, ResponseStatus status, T data)
        {
            this.data = data;
            this.status = status.ToString();
            this.code = code;
        }
        public SsoResponseData(String status, T data)
        {
            this.data = data;
            this.status = status;
        }

        public SsoResponseData(int code, String status, T data)
        {
            this.data = data;
            this.status = status;
            this.code = code;
        }


        public static SsoResponseData<object> success()
        {
            return success(ResponseStatus.SUCCESS);
        }
        public static SsoResponseData<object> error(int code, Object t)
        {
            return new SsoResponseData<object>(code, ResponseStatus.ERROR, t);
        }

        public static SsoResponseData<object> success(Object t)
        {
            return new SsoResponseData<object>(ResponseStatus.SUCCESS, t);
        }
        public static SsoResponseData<object> success(int code, Object t)
        {
            return new SsoResponseData<object>(code, ResponseStatus.SUCCESS, t);
        }
        public static SsoResponseData<object> error(int code, string status, Object t)
        {
            return new SsoResponseData<object>(code, status, t);
        }
    }
}
