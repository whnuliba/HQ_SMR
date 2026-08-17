using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public enum ResponseStatus
    {
        SUCCESS = 200,
        ERROR = 500,
        WARNING = 300
    }
    public class ResponseEntity<T>
    {
        public T data { get; set; }
        public String status { get; set; }
        public int code { get; set; }
        public string message { get; set; }

        public ResponseEntity() { }
        public ResponseEntity(ResponseStatus status, T data)
        {
            this.data = data;
            this.status = status.ToString();
            this.code = (int)status;
        }

        public ResponseEntity(ResponseStatus status, T data,string message)
        {
            this.data = data;
            this.status = status.ToString();
            this.code = (int)status;
            this.message = message;
        }


        public static ResponseEntity<T> Error(string mesg)
        {
            var r = new ResponseEntity<T>();
            r.code = (int)ResponseStatus.ERROR;
            r.message = mesg;
            return r;
        }

        public static ResponseEntity<T> Error(int code ,T t, string mesg)
        {
            var r = new ResponseEntity<T>();
            r.code = (int)ResponseStatus.ERROR;
            r.message = mesg;
            return r;
        }

        public static ResponseEntity<T> Error(T t,string mesg)
        {
            return new ResponseEntity<T>(ResponseStatus.ERROR, t, mesg);
        }

        public static ResponseEntity<T> Error()
        {
            var r = new ResponseEntity<T>();
            r.code = (int)ResponseStatus.ERROR;
            r.message = "error";
            return r;
        }
        //public static ResponseEntity<String> success()
        //{
        //    return success(ResponseStatus.SUCCESS.getMessage());
        //}
        public static ResponseEntity<T> Error(int code, T t)
        {
            return new ResponseEntity<T>(code, ResponseStatus.ERROR, t);
        }

        //public static ResponseEntity error(ErrorCode t)
        //{
        //    return new ResponseEntity(t.code(), ResponseStatus.ERROR, t.message());
        //}
        public static ResponseEntity<T> Success(T t)
        {
            return new ResponseEntity<T>(ResponseStatus.SUCCESS, t);
        }
        public static ResponseEntity<T> Success(int code, T t)
        {
            return new ResponseEntity<T>(code, ResponseStatus.SUCCESS, t);
        }
        public ResponseEntity(int code, ResponseStatus status, T data)
        {
            this.data = data;
            this.status = status.ToString();
            this.code = code;
        }
        public ResponseEntity(String status, T data)
        {
            this.data = data;
            this.status = status;
        }

        public ResponseEntity(int code, String status, T data)
        {
            this.data = data;
            this.status = status;
            this.code = code;
        }
    }
}
