using IDS.Base;
using IDS.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using static IDS.Base.IdsFilter;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace IDS.SSO.Client
{
    /// <summary>
    /// IExceptionFilter 异常拦截
    /// ActionFilterAttribute 请求拦截器
    /// </summary>
    public class ActionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public string authuri = "";
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 判断是否加上了不需要拦截
            string url = context.HttpContext.Request.Path.Value;
            string headler = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authuri)) {
                //var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
               // var config = configBuilder.Build();
                // authuri = config.GetValue<string>("authUrl");
                authuri = AppConfig.GetConfigInfo("authUrl");
            }
            var noNeedCheck = false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                noNeedCheck = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AnonymousAttribute)));
                if (noNeedCheck) {
                    var anonymous = controllerActionDescriptor.MethodInfo.GetCustomAttribute<AnonymousAttribute>();
                    if (!anonymous.CheckAudience) { 
                         return;
                    }
                }
            }
            //匿名访问
            if (noNeedCheck)
            {
                if (string.IsNullOrWhiteSpace(headler)) return;

                if (headler.StartsWith("Bearer "))
                {
                    headler = headler.Substring(7);
                }
                try
                {



                    var _jwt = new JwtSecurityTokenHandler();
                    JwtSecurityToken _token = _jwt.ReadJwtToken(headler);
                    if (_jwt == null)
                    {

                        var res = new JsonResult(ResponseEntity<string>.Error(401, "header is error:Authorization"))
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


                    #region 获取用户信息
                    string userUrl = "";
                    if (authuri.EndsWith("/"))
                    {
                        userUrl = authuri + "getUser";
                    }
                    else
                        userUrl = authuri + "/getUser";
                    SsoRequestData<string> data = new()
                    {
                        data = _un
                    };
                    string str = HttpUtil.Post(userUrl, JsonConvert.SerializeObject(data));

                    SsoResponseData<string> userResp = JsonConvert.DeserializeObject<SsoResponseData<string>>(str);
                    if (userResp.code == 200)
                    {
                        string userStr = userResp.data;
                        //AurhUserInfo
                        var userInfo = JsonConvert.DeserializeObject<AuthUserInfo>(userStr);
                        CurrentUser.SetUser(userInfo);
                        return;
                    }
                    #endregion

                    context.HttpContext.Request.Headers.Add("username", _un);

                }
                catch (Exception ex)
                {

                }
                return;
            }
            if (string.IsNullOrWhiteSpace(headler))
            {

                var res = new JsonResult(ResponseEntity<string>.Error(401, "not found header:Authorization"))
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

                    var res = new JsonResult(ResponseEntity<string>.Error(401, "header is error:Authorization"))
                    {
                        ContentType = "application/json;charset=UTF-8"
                    };
                    context.Result = res;
                    return;
                }

                var validTo = token.ValidTo;
                if (validTo < DateTime.UtcNow)
                {
                    var res = new JsonResult(ResponseEntity<string>.Error(401, $"Session expiration :Authorization"))
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
                context.HttpContext.Request.Headers.Add("username", un);

                //权限校验

                var keyValues = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {headler}" }
            };

                string resourceUrl = "";
                if (authuri.EndsWith("/"))
                {
                    resourceUrl = authuri + "validatePathAndToken";
                }
                else
                    resourceUrl = authuri + "/validatePathAndToken";
                SsoRequestData<string> data = new()
                {
                    data = url
                };
                string str = HttpUtil.Post(resourceUrl, JsonConvert.SerializeObject(data), keyValues);

                if (string.IsNullOrWhiteSpace(str))
                {
                    var res = new JsonResult(SsoResponseData<string>.error(401, "ERROR", "Authorization is failure"))
                    {
                        ContentType = "application/json;charset=UTF-8"
                    };
                    context.Result = res;
                    return;
                }
                SsoResponseData<string> authResp = JsonConvert.DeserializeObject<SsoResponseData<string>>(str);
                if (authResp == null)
                {

                    var res = new JsonResult(SsoResponseData<string>.error(500, "ERROR", "request auth vail failure"))
                    {
                        ContentType = "application/json;charset=UTF-8"
                    };
                    context.Result = res;
                    return;
                }
                if (authResp.code != 200)
                {
                    var res = new JsonResult(str)
                    {
                        ContentType = "application/json;charset=UTF-8"
                    };
                    context.Result = res;
                    return;
                }
                string userStr = authResp.data;
                //AurhUserInfo
                var userInfo =  JsonConvert.DeserializeObject<IdsUserInfo>(userStr);
                CurrentUser.SetUser(userInfo);
            }
            catch (Exception ex)
            {
                var res = new JsonResult(ResponseEntity<string>.Error(401, $"header is error {ex.Message}:Authorization"))
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
                  .Any(a => a.GetType().Equals(typeof(AuthFilterAttribute)));
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
                  .Any(a => a.GetType().Equals(typeof(AuthFilterAttribute)));
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
                  .Any(a => a.GetType().Equals(typeof(AuthFilterAttribute)));
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

    /// <summary>
    /// 不需要登陆的地方加个这个空的拦截器
    /// </summary>
    public class AuthFilterAttribute : ActionFilterAttribute { }
}