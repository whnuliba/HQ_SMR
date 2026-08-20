using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service.Adapter;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using static LinqToDB.Common.Configuration;

namespace IDS.HQ.Controller
{
    [Route("user")]
    [PropertiesAutowired]
    [ApiController]
    public class UserInfoController : DbBaseController<UserInfo>
    {
        public UserInfoAdapter _aserInfoAdapter { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<UserInfo> Adapter()
        {
            return _aserInfoAdapter;
        }

        [HttpPost]
        [Route("UpdatePwd")]
        public  ResponseEntity<object> UpdatePwd(RequestData<ChangeUserPassword> data)
        {
            if (!RequestData<ChangeUserPassword>.isRequest(data))
                return ResponseEntity<object>.Error("上传信息为空");
            _aserInfoAdapter.UpdatePwd(data.data);
          return ResponseEntity<object>.Success("");
        }
        [HttpPost]
        [Route("Login")]
        public ResponseEntity<JwtUser> Login(RequestData<UserInfo> data)
        {
            if (!RequestData<UserInfo>.isRequest(data))
                return ResponseEntity<JwtUser>.Error("上传信息为空");
            IdsResult<JwtUser> res = _aserInfoAdapter.Login(data.data);
            if (res.Success)
                return ResponseEntity<JwtUser>.Success(res.Data);
            else return ResponseEntity<JwtUser>.Error(res.Message);
        }
        [HttpPost]
        [Route("Permissions")]
        public ResponseEntity<JwtUser> Permissions()
        {

            var headers = HttpContext.Request.Headers;
            string token = string.Empty;
            if (headers.ContainsKey("token")) {
                token = headers["token"];
            }
            if (string.IsNullOrEmpty(token)) {
                return ResponseEntity<JwtUser>.Error("当前用户没有权限");
            }
            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");
            string username = string.Empty;
            try
            {
                username = RsaHelper.Decrypt(token, prk, true);
            }
            catch (Exception ex) {
                return ResponseEntity<JwtUser>.Error("当前用户没有权限");
            }
            IdsResult<JwtUser> res = _aserInfoAdapter.Permissions(username);
            if (res.Success)
                return ResponseEntity<JwtUser>.Success(res.Data);
            else return ResponseEntity<JwtUser>.Error(res.Message);
        }
    }
}
