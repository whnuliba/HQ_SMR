using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class IdsResult<E> 
    {

        public bool Success { set; get; }
        public string Message { set; get; }
        public E Data { set; get; }
        public IdsResult()
        {
        }

        public IdsResult(bool Success, String Message, E t)
        {
            this.Success = Success;
            this.Message = Message;
            this.Data = t;
        }


        public static IdsResult<E> failure(String msg)
        {
            var result = new IdsResult<E>();
            result.Success = false;
            result.Message = msg;
            return result;
        }

        public static IdsResult<E> failure()
        {
            var result = new IdsResult<E>();
            result.Success = false;
            result.Message = "failure";
            return result;
        }
        public static IdsResult<E> ok(bool state)
        {
            var result = new IdsResult<E>();
            result.Success = state;
            return result;
        }
        public static IdsResult<E> ok()
        {
            var result = new IdsResult<E>();
            result.Success = true;
            return result;
        }
        public static   IdsResult<E> ok(bool state, String Message)
        {
            var result = new IdsResult<E>();
            result.Success = state;
            result.Message = Message;
            return result;
        }
        public static  IdsResult<E> ok(bool state, String Message, E e)
        {
            return new IdsResult<E>(state, Message, e);
        }
        public static  IdsResult<E> ok(bool state, E e)
        {
            return new IdsResult<E>(state, null, e);
        }
        public static  IdsResult<E> ok(E e)
        {
            return new IdsResult<E>(true, "ok", e);
        }
    }
}
