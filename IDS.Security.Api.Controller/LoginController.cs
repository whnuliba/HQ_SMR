using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extension;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.Api.Controller.DTO;
using IDS.Security.IService;
using IDS.Security.Jwt;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Tsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IDS.Security.Api.Controller
{
    [PropertiesAutowired]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public virtual UserInfoAdapter UserInfoAdapter { set; get; }
        public virtual RoleInfoAdapter RoleInfoAdapter { set; get; }

        public virtual UserGroupAdapter UserGroupAdapter { set; get; }
        public virtual ILogger<UserInfoController> Logger { set; get; }

        public virtual IdsRedis RedisClient { set; get; }
        [Route("login")]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<JwtResponse> Login(LoginUser loginUser)
        {
            if (loginUser == null)
                throw new BussinessException("登录用户不能为空|The logged-in user cannot be empty");
            string userid = loginUser.username;
            Assert.notEmpty(userid, "用户名不能为空|The username cannot be empty");
            string password = loginUser.password;
            Assert.notEmpty(password, "密码不能为空|The password cannot be empty");
            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");
            if (!string.IsNullOrWhiteSpace(userid) && !string.IsNullOrWhiteSpace(password))
            {
                var userInfo = UserInfoAdapter.getUser(userid);
                if (userInfo == null)
                    throw new BussinessException("密码不正确或用户不存在!|The password is incorrect or the user does not exist");
                //判断密码
                UserInfoAdapter.Check(userInfo);
                try {
                    string _procPwd = password;// RsaHelper.Decrypt(password, prk, true);
                    string _password = userInfo.Password;
                    if (string.IsNullOrWhiteSpace(_password))
                    {
                        throw new BussinessException("用户密码不存在|The password is incorrect or the user does not exist");
                    }

                    string pwd = RsaHelper.Decrypt(_password, prk, true);

                    if (!pwd.Equals(_procPwd))
                    {
                        throw new BussinessException("密码不正确或用户不存在|The password is incorrect or the user does not exist");
                    }
                } catch (Exception ex) {

                    throw new BussinessException(ex.Message);
                }
                List<Claim> userClaim = new List<Claim>() {
                        new Claim(ClaimTypes.Name,userInfo.UserName),
                        new Claim("sub",userInfo.UserName)
                };
                JwtSecurityToken token = new JwtSecurityToken(
                        issuer: AppConfig.GetConfigInfo("JwtTokenOptions:Issuer"),
                        audience: AppConfig.GetConfigInfo("JwtTokenOptions:Audience"),
                        claims: userClaim,
                        expires: DateTime.Now.AddMinutes(60 * 24),// DateTime.Now.AddSeconds(30),// ,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.GetConfigInfo("JwtTokenOptions:SecurityKey"))), SecurityAlgorithms.HmacSha256)
                    );
                string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
                JwtUser jwtUser = new JwtUser();
                jwtUser.mobile = userInfo.Mobile;
                jwtUser.id = userInfo.Id;
                jwtUser.username = userInfo.UserName;
                jwtUser.realName = userInfo.RealName;
                jwtUser.AccountExpireTime=userInfo.AccountExpireTime;
                jwtUser.PasswordExpireTime=userInfo.PasswordExpireTime;
                jwtUser.ChangePassword=userInfo.ChangePassword;
                jwtUser.Lock=userInfo.Lock;
                jwtUser.Alias=userInfo.Alias;
                jwtUser.NameSpell=userInfo.NameSpell;
                var auths = new List<SimpleGrantedAuthority>();
                userInfo.Roles?.ForEach(c => {
                    auths.Add(new SimpleGrantedAuthority
                    {
                        Role = IdsConstant.ROLE_PREFIX + c.RoleCode
                    });

                });
                jwtUser.Authorities = auths;
                var jwtReponse = new JwtResponse(tokenStr, jwtUser);
                var resp = ResponseEntity<JwtResponse>.Success(jwtReponse);
                return resp;
                
            }
            return ResponseEntity<JwtResponse>.Error();
        }

        [Route("auth")]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<JwtResponse> auth(LoginUser loginUser)
        {
            if (loginUser == null)
                throw new BussinessException("登录用户不能为空|The logged-in user cannot be empty");
            string userid = loginUser.username;
            Assert.notEmpty(userid, "用户名不能为空|The username cannot be empty");
            string password = loginUser.password;
            Assert.notEmpty(password, "密码不能为空|The password cannot be empty");
            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");
            if (!string.IsNullOrWhiteSpace(userid) && !string.IsNullOrWhiteSpace(password))
            {
                var userInfo = UserInfoAdapter.getUser(userid);
                if (userInfo == null)
                    throw new BussinessException("密码不正确或用户不存在!|The password is incorrect or the user does not exist");
                //判断密码
                UserInfoAdapter.Check(userInfo);
                try
                {
                    string _procPwd = RsaHelper.Decrypt(password, prk, true);
                    string _password = userInfo.Password;
                    if (string.IsNullOrWhiteSpace(_password))
                    {
                        throw new BussinessException("用户密码不存在|The user password does not exist");
                    }

                    string pwd = RsaHelper.Decrypt(_password, prk, true);

                    if (!pwd.Equals(_procPwd))
                    {
                        throw new BussinessException("密码不正确或用户不存在|The password is incorrect or the user does not exist");
                    }
                }
                catch (Exception ex)
                {

                    throw new BussinessException(ex.Message);
                }
                List<Claim> userClaim = new List<Claim>() {
                        new Claim(ClaimTypes.Name,userInfo.UserName),
                        new Claim("sub",userInfo.UserName)
                };
                JwtSecurityToken token = new JwtSecurityToken(
                        issuer: AppConfig.GetConfigInfo("JwtTokenOptions:Issuer"),
                        audience: AppConfig.GetConfigInfo("JwtTokenOptions:Audience"),
                        claims: userClaim,
                        expires: DateTime.Now.AddMinutes(60 * 24),// DateTime.Now.AddSeconds(30),// ,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.GetConfigInfo("JwtTokenOptions:SecurityKey"))), SecurityAlgorithms.HmacSha256)
                    );
                string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
                JwtUser jwtUser = new JwtUser();
                jwtUser.mobile = userInfo.Mobile;
                jwtUser.id = userInfo.Id;
                jwtUser.username = userInfo.UserName;
                jwtUser.realName = userInfo.RealName;
                jwtUser.AccountExpireTime = userInfo.AccountExpireTime;
                jwtUser.PasswordExpireTime = userInfo.PasswordExpireTime;
                jwtUser.ChangePassword = userInfo.ChangePassword;
                jwtUser.Lock = userInfo.Lock;
                jwtUser.Alias = userInfo.Alias;
                jwtUser.NameSpell = userInfo.NameSpell;
                var auths = new List<SimpleGrantedAuthority>();
                userInfo.Roles?.ForEach(c => {
                    auths.Add(new SimpleGrantedAuthority
                    {
                        Role = IdsConstant.ROLE_PREFIX + c.RoleCode
                    });

                });
                jwtUser.Authorities = auths;
                var jwtReponse = new JwtResponse(tokenStr, jwtUser);
                var resp = ResponseEntity<JwtResponse>.Success(jwtReponse);
                if (UserInfoAdapter.CheckPasswordExpire(userInfo.PasswordExpireTime))
                {
                    jwtUser.ChangePassword = "Y";
                    resp.message = "Failed to authenticate since user password has expired";
                }
              
                return resp;

            }
            return ResponseEntity<JwtResponse>.Error();
        }


        [Route("encrypt-key")]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<string> GetPublicKey()
        {
            return ResponseEntity<string>.Success(AppConfig.GetConfigInfo("RSA:publicKey"));
        }
        [Route("getUser")]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<string> GetUser(RequestData<string> data) {

            if (data == null || string.IsNullOrEmpty(data.data))
            {
                return ResponseEntity<string>.Error();
            }
            var uinfo = UserInfoAdapter.QueryUserRoles(data.data);

            if (uinfo == null)
            {
                return ResponseEntity<string>.Error(401, "", "不存在该用户");
            }
            uinfo.Password = null;
            if (uinfo.Roles == null || uinfo.Roles.Count() == 0)
            {
                return ResponseEntity<string>.Error(401, "", "用户角色不存在");
            }

            uinfo.RoleList = uinfo.Roles.Select(f => f.RoleCode).ToList();

            uinfo.UserRole = null;
            //获取用户组
            var userGroups = UserGroupAdapter.SelectByUserId(uinfo.Id);

            if (userGroups != null && userGroups.Count() > 0)
            {
                uinfo.UserGroup = userGroups.Select(c => c.GroupNo).ToList();
            }

            var roles = uinfo.Roles.Select(f => f.RoleCode).Distinct().ToList();

            foreach (string roleStr in roles)
            {
                if (IdsConstant.SUPER_ADMIN_ROLE.Equals(IdsConstant.ROLE_PREFIX + roleStr))
                {
                    try
                    {
                        //string str = RsaHelper.Encrypt(MyObjectUtils.Serialize(uinfo), puk, true);
                        return ResponseEntity<string>.Success(MyObjectUtils.Serialize(uinfo));
                    }
                    catch (Exception e)
                    {
                        throw new BussinessException(e.Message);
                    }

                }
            }
            uinfo.Roles = null;

            return ResponseEntity<string>.Success(MyObjectUtils.Serialize(uinfo));
        }

        [Route("validatePathAndToken")]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<string> checkPermission(RequestData<string> data) {

            // 判断是否加上了不需要拦截
            String url = HttpContext.Request.Path.Value;
            string headler = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(headler)) { 
               return ResponseEntity<string>.Error(401,"", "认证失效，或权限不足或会话失效,或没有认证的令牌");
            }
            if (headler.StartsWith("Bearer "))
            {
                headler = headler.Substring(7);
            }
            else {

                return ResponseEntity<string>.Error(401, "", "不是有效的令牌");
            }

            var _jwt = new JwtSecurityTokenHandler();
            JwtSecurityToken _token = _jwt.ReadJwtToken(headler);
            if (_jwt == null) {
                return ResponseEntity<string>.Error(401, "", "不是有效的令牌");
            }
            string username = _token.Subject;

            var validTo = _token.ValidTo;
            if (validTo < DateTime.UtcNow) {
                return ResponseEntity<string>.Error(401, "", "令牌已经过期");
            }

            //判断是否是管理员
            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");
            if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(username)) {

                var u = new UserInfo
                {
                    UserName = username
                };
                //string str =RsaHelper.Encrypt(MyObjectUtils.Serialize(u), puk,true);
                return ResponseEntity<string>.Success(MyObjectUtils.Serialize(u));
            }

            var uinfo =  UserInfoAdapter.QueryUserRoles(username);

            if (uinfo == null) {
                return ResponseEntity<string>.Error(401, "", "不存在该用户");
            }
            uinfo.Password = null;
            if (uinfo.Roles == null || uinfo.Roles.Count() == 0) {
                return ResponseEntity<string>.Error(401, "", "用户角色不存在");
            }

            uinfo.RoleList = uinfo.Roles.Select(f => f.RoleCode).Distinct().ToList();
           
            uinfo.UserRole = null;
            //获取用户组
            var userGroups =  UserGroupAdapter.SelectByUserId(uinfo.Id);

            if (userGroups != null && userGroups.Count() > 0)
            {
                uinfo.UserGroup = userGroups.Select(c => c.GroupNo).ToList();
            }

            var roles = uinfo.Roles.Select(f=>f.RoleCode).ToList();

            foreach (string roleStr in roles)
            {
                if (IdsConstant.SUPER_ADMIN_ROLE.Equals(IdsConstant.ROLE_PREFIX + roleStr))
                {
                    try
                    {
                        //string str = RsaHelper.Encrypt(MyObjectUtils.Serialize(uinfo), puk, true);
                        return ResponseEntity<string>.Success(MyObjectUtils.Serialize(uinfo));
                    }
                    catch (Exception e)
                    {
                        throw new BussinessException(e.Message);
                    }

                }
            }
            uinfo.Roles = null;
            var funcs = RoleInfoAdapter.QueryAllFuncViewByUserNameAndRoute(username, data.data);

            if (funcs.Count() > 0)
            {
                try
                {
                    //string str = RsaHelper.Encrypt(MyObjectUtils.Serialize(uinfo), puk, true);
                    return ResponseEntity<string>.Success(MyObjectUtils.Serialize(uinfo));
                }
                catch (Exception e)
                {
                    return ResponseEntity<string>.Error(401,null,"功能权限不存在");
                }
            }

            if (RoleInfoAdapter.IsSupperAdmin(username))
            {
                try
                {
                    //string str = RsaHelper.Encrypt(MyObjectUtils.Serialize(uinfo), puk, true);
                    return ResponseEntity<string>.Success(MyObjectUtils.Serialize(uinfo));
                }
                catch (Exception e)
                {
                    return ResponseEntity<string>.Error(401, null, "功能权限不存在");
                }
            }
            return ResponseEntity<string>.Error(401, null, "功能权限不存在");

        }
    }
}
