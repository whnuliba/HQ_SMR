using IDS.Base;
using IDS.Common;
using IDS.Extension;
using IDS.Ioc;
using IDS.Security.Adapter;
using IDS.Security.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using IDS.Base.Utils;
using IDS.Persistence;
using IDS.Security.IService.DTO;
using IDS.Security.Service;
using log4net.Core;
using IDS.Security.IService;
using IDS.Security.IService.POCO;
using Microsoft.AspNetCore.Http.Headers;
using System.Reflection.PortableExecutable;
using IDS.Common.Utils;
using System.Linq.Expressions;

namespace IDS.Security.Api.Controller
{
    //[Route("[controller]")]
    [Route("user")]
    [PropertiesAutowired]
    [ApiController]
    public class UserInfoController : DbBaseController<UserInfo>
    {
              //   "RSA": {
              //  "privateKey": "MIICXAIBAAKBgQCtVUSSNIXAZVzmRDR961BjC0V6zr46uh/N1wRS/iUy3mfRlMkW5NrrcdWy5hmuKoTGFY7noyvnVD6XMhgcrp2bLJ8E5AkxmCCOxShwbQ0RDvHvk3PepU6jwcTsk8FRuG13HMEWDBh9dt9pBTwm88MscO+dO5Zzm+JcXJy/krzE9QIDAQABAoGAWZqFuysJpZ8Aukyx8hIuWTUfcs/jiQpknI01wA1/f193veEzXvFptmL/fN70kZOLVbcZS+ePL6PeQ1zzGAiwkghM6vjPp1bd9JZqNronojLuRxkgzsmlRIK/5WTKH1XedeC5fSjMZxYBnu1yVV93rsRlivHtxIBoR+LWOxspEFUCQQDNAqLHGdsXr75SjZyuyOOQy/nvbM17ltpQuzxbZuMsWdfmKYC44GlgocKj66sr1zh4qK4/MEisysuBV0ktV9lHAkEA2HGwJiYcxnKWJ9JRzWBOoWUTvfJJsDCBVOUlHlcugfAInV7EsYCa0AuDrGwM3vutB9kpGQ/0YEmA1CcEwP2N4wJABVr8RZc3UfcVUbLBVQ+KYk3xyX9eHsxiB5ncica2SIJQUYLkCfBS0GNpYj7Vyd9lakF9y8jHHTxu9IIDN1wEIQJATbAIvLijTWtlj+eCqwetUWq5Ok1Tu6S9Vx5kQ06hh8wDG5EfYmK5roDjgyjJOeP1yEJe6Pr8CO95CSz0HN0lZwJBAJBYermjkk3U8Og+GckzOxLzCEw2mBN8t9V+RC64wCDfMZYMxYA0g2P5B6PXW0X3JePU/Ry1ItQH0hznJcswcXc=",
              //  "publicKey":  "MIGJAoGBAK1VRJI0hcBlXOZENH3rUGMLRXrOvjq6H83XBFL+JTLeZ9GUyRbk2utx1bLmGa4qhMYVjuejK+dUPpcyGByunZssnwTkCTGYII7FKHBtDREO8e+Tc96lTqPBxOyTwVG4bXccwRYMGH1232kFPCbzwyxw7507lnOb4lxcnL+SvMT1AgMBAAE="
              //}
        public virtual UserInfoAdapter UserInfoAdapter { set; get; }
        public virtual ILogger<UserInfoController> Logger { set; get; }

        [Route("GetUser")]
        [HttpPost]
        public ResponseEntity<UserInfoVo> GetUser(RequestData<string> data) {

            if (!RequestData<string>.isRequest(data)) {
                return ResponseEntity<UserInfoVo>.Error("用户名不能为空！");
            }
            Assert.notEmpty(data.data, "用户名不能为空");
            var userInfo = UserInfoAdapter.getUser(data.data);
            return ResponseEntity<UserInfoVo>.Success(userInfo);
        }

        [Route("Create")]
        [HttpPost]
        public ResponseEntity<string> CreateUser(UserInfo userInfo) {

            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");

            if(userInfo==null)
                throw new BussinessException("上传用户信息不能为空");

            //实现用户密码加密
            string userName = userInfo.UserName;
            if (userName.IsNullOrEmpty())
            {

                throw new BussinessException("登录名不存在");
            }
            string password = userInfo.Password;
            if (password.IsNullOrEmpty()) {

                throw new BussinessException("用户密码不存在");
            }

            string realName = userInfo.RealName;
            if (password.IsNullOrEmpty())
            {
                throw new BussinessException("用户真实姓名不存在");
            }
            string sercet = RsaHelper.Encrypt(password, puk, true);


            return ResponseEntity<string>.Success("OK");
        }

        #region 与Java同步

        [Route(Route.ROUTE_ROOT_USER_ROLE)]
        [HttpPost]
        public ResponseEntity<object> saveFuncByRole(RequestData<List<UserRole>> list)
                {
                    if (list == null || list.data == null || list.data.Count == 0)
                        return ResponseEntity<object>.Error("参数不能为空");
                    list.data.ForEach(c=>{
                        c.Id= BaseUtil.uuid();
                        c.saveInit();
                    });
                    return ResponseEntity<object>.Success(UserInfoAdapter.BatchInsert(list.data));
                }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<UserInfo> Adapter()
        {
            return UserInfoAdapter;
        }


         [Route(Route.ROUTE_ROOT_USER_DEL_USER_ROLE)]
         [HttpPost]
        public ResponseEntity<int> delUserAndRole(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<int>.Error("参数为空");
            return ResponseEntity<int>.Success(UserInfoAdapter.DelUserAndRole(data.data));
        }
        [Route(Route.ROUTE_ROOT_USER_EDIT_PWD)]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<string> updatePwd(RequestData<ChangeUserPwdDto> data)
        {
            if (!RequestData<ChangeUserPwdDto>.isRequest(data))
                return ResponseEntity<string>.Error("参数不能为空!");
            UserInfoAdapter.UpdatePwd(data.data);
            return ResponseEntity<string>.Success("OK");
        }

        [Route(Route.ROUTE_ROOT_USER_RESET_PWD)]
        [HttpPost]
        public ResponseEntity<string> resetPwd(RequestData<ChangeUserPwdDto> data)
        {
            if (!RequestData<ChangeUserPwdDto>.isRequest(data))
                return ResponseEntity<string>.Error("参数不能为空");
            UserInfoAdapter.ResetPwd(data.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route(Route.ROUTE_ROOT_USER_GETALL)]
        [HttpPost]
        public ResponseEntity<List<VDepartmentUser>> QueryAllUser()
        {
            return ResponseEntity<List<VDepartmentUser>>.Success(UserInfoAdapter.QueryAllUser());
        }
        [Route(Route.ROUTE_ROOT_USER_GETALL_IDS)]
        [HttpPost]
        public ResponseEntity<List<VDepartmentUser>> QueryUserByIds(RequestData<List<string>> data)
        {
            if (!RequestData<List<string>>.isRequest(data))
                return ResponseEntity<List<VDepartmentUser>>.Error();
            return ResponseEntity<List<VDepartmentUser>>.Success(UserInfoAdapter.QueryUserByIds(data.data));
        }
        [Route(Route.ROUTE_ROOT_USER_GET_USER_NAME)]
        [HttpPost]

        public ResponseEntity<List<VDepartmentUser>> QueryUserByUserName(RequestData<List<string>> data)
        {
            if (!RequestData<List<string>>.isRequest(data))
                return ResponseEntity<List<VDepartmentUser>>.Error();
            return ResponseEntity<List<VDepartmentUser>>.Success(UserInfoAdapter.QueryUserByUserNames(data.data));
        }

        [Route("fc-userinfo")]
        [HttpPost]
        public ResponseEntity<UserInfoVo> getUserInfo(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                throw new BussinessException("用户参数不能为空");
            UserInfoVo userInfo = UserInfoAdapter.QueryUserRoles(data.data);
            if (userInfo == null)
                throw new BussinessException("用户数据不存在");
            userInfo.Password=null;
            return ResponseEntity<UserInfoVo>.Success(userInfo);
        }
        [Route(Route.ROUTE_ROOT_USER_IS_LOGIN)]
        [HttpPost]
        public async Task<ResponseEntity<UserSessionDto>> isLogin(RequestData<UserSessionDto> data)
        {

            if (!RequestData<UserSessionDto>.isRequest(data))
                return ResponseEntity<UserSessionDto>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            data.data.State =100; //表示首次创建
            var u =await UserInfoAdapter.IsLogin(data.data);
            return ResponseEntity<UserSessionDto>.Success(u);
        }
        public override ResponseEntity<Page<UserInfo>> List(Page<UserInfo> data)
        {
            var page = Adapter().GetPages(data);
            return ResponseEntity<Page<UserInfo>>.Success(page);
        }
        [Route("dept-user")]
        [HttpPost]
        public ResponseEntity<List<UserInfo>> getDeptUsers(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<List<UserInfo>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<UserInfo>>.Success(UserInfoAdapter.SelectDeptUser(data.data));
        }
        /*

         //ROUTE_ROOT_USER_IS_LOGIN
         @PostMapping(UrlConstant.Route.ROUTE_ROOT_USER_IS_LOGIN)
     public ResponseEntity<UserSessionDto> isLogin(@RequestBody Request<UserSessionDto> data, @RequestHeader HttpHeaders headers)
         {

             if (!Request.isRequest(data))
                 return ResponseEntity.error(ErrorCode.PARAMETER_NULL);
             data.getData().setState(100); //表示首次创建
             return ResponseEntity.success(userInfoService.isLogin(data.getData()));
         }


         @PostMapping(UrlConstant.Route.ROUTE_ROOT_USER_KICKED_OUT)
     public ResponseEntity<UserSessionDto> updateLoginState(@RequestBody Request<UserSessionDto> data)
         {
             if (!Request.isRequest(data))
                 return ResponseEntity.error(ErrorCode.PARAMETER_NULL);
             return ResponseEntity.success(userInfoService.updateLoginState(data.getData()));
         }

         @PostMapping(UrlConstant.Route.ROUTE_ROOT_USER_LOGIN_USER)
     public ResponseEntity<UserSessionDto> getLoginUsers()
         {
             return ResponseEntity.success(userInfoService.getLoginUsers());
         }

         @PostMapping("dept-user")
     public ResponseEntity<List<UserInfo>> getDeptUsers(@RequestBody Request<String> data)
         {
             if (!Request.isRequest(data))
                 return ResponseEntity.success(ErrorCode.PARAMETER_NULL);
             return ResponseEntity.success(userInfoService.selectDeptUser(data.getData()));
         }

*/

        #endregion

    }
}
