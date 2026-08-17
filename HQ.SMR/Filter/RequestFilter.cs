using IDS.Base;
using IDS.Common;
using IDS.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using static IDS.Base.IdsFilter;

namespace HQ.SMR.Filter
{
    /// <summary>
    /// IExceptionFilter 异常拦截
    /// ActionFilterAttribute 请求拦截器
    /// </summary>
    public class RequestFilter : ActionFilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// 在控制器执行之前调用
        /// </summary>
        /// <param name="context">执行的上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 判断是否加上了不需要拦截
            String url = context.HttpContext.Request.Path.Value;
            string headler = context.HttpContext.Request.Headers["Authorization"];

       

            var noNeedCheck = false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                noNeedCheck = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AnonymousAttribute)));
            }
            //匿名访问
            if (noNeedCheck) {
                if (string.IsNullOrWhiteSpace(headler)) return;

                if (headler.StartsWith("Bearer ")) { 
                   headler = headler.Substring(7);
                }
                try
                {

                    var _jwt = new JwtSecurityTokenHandler();
                    JwtSecurityToken _token = _jwt.ReadJwtToken(headler);
                    if (_jwt == null)
                    {

                        var res = new JsonResult(ResponseEntity<String>.Error(401, "header is error:Authorization"))
                        {
                            ContentType = "application/json;charset=UTF-8"
                        };
                        context.Result = res;
                        return;
                    }
                    string _un = _token.Subject;
                    //解析是否登录用户
                    var _auth = new AuthUserInfo()
                    {
                        UserName = _un
                    };
                    CurrentUser.SetUser(_auth);
                    context.HttpContext.Request.Headers.Add("username", _un);
                }
                catch (Exception ex) { 
                
                   
                }

                return;
            }
            if (string.IsNullOrWhiteSpace(headler))
            {

                var res = new JsonResult(ResponseEntity<String>.Error(401, "not found header:Authorization"))
                {
                    ContentType = "application/json;charset=UTF-8"
                };
                context.Result = res;
                return;
            }
            try
            {

                if (headler.StartsWith("Bearer "))
                {
                    headler = headler.Substring(7);
                }
                var jwt = new JwtSecurityTokenHandler();
                JwtSecurityToken token = jwt.ReadJwtToken(headler);

                if (token == null)
                {

                    var res = new JsonResult(ResponseEntity<String>.Error(401, "header is error:Authorization"))
                    {
                        ContentType = "application/json;charset=UTF-8"
                    };
                    context.Result = res;
                    return;
                }

                var validTo = token.ValidTo;
                if (validTo < DateTime.UtcNow) {
                    var res = new JsonResult(ResponseEntity<String>.Error(401, $"Session expiration :Authorization"))
                    {
                        ContentType = "application/json;charset=UTF-8"
                    };
                    context.Result = res;
                    return;
                }
                string un = token.Subject;
                //解析是否登录用户
                var auth = new AuthUserInfo()
                {
                    UserName = un
                };
                CurrentUser.SetUser(auth);


                //权限校验




                context.HttpContext.Request.Headers.Add("username", un);
            }
            catch (Exception ex) {
                var res = new JsonResult(ResponseEntity<String>.Error(401, $"header is error {ex.Message}:Authorization"))
                {
                    ContentType = "application/json;charset=UTF-8"
                };
                context.Result = res;
                return;
            }
        }

        /// <summary>
        /// 在控制器执行之后调用
        /// </summary>
        /// <param name="context">执行的上下文</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // 判断是否加上了不需要拦截
            var noNeedCheck = false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                noNeedCheck = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AnonymousAttribute)));
            }
            if (noNeedCheck) return;

        }

        /// <summary>
        /// 在返回数据执行之前调用
        /// </summary>
        /// <param name="context">执行的上下文</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // 判断是否加上了不需要拦截
            var noNeedCheck = false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                noNeedCheck = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AnonymousAttribute)));
            }
            if (noNeedCheck) return;

        }

        /// <summary>
        /// 在返回数据执行之后调用
        /// </summary>
        /// <param name="context">执行的上下文</param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            // 判断是否加上了不需要拦截
            var noNeedCheck = false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                noNeedCheck = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AnonymousAttribute)));
            }
            if (noNeedCheck) return;

        }

        /// <summary>
        /// 当然是发生异常时被调用了
        /// </summary>
        /// <param name="context">执行的上下文</param>
        public void OnException(ExceptionContext context)
        {
           // context.ExceptionHandled = true;//异常已经处理，不要再次处理了
        }
    }
}