using IDS.Base.Utils;
using IDS.Common;
using IDS.Common.Utils;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class UserInfoService : DbBaseService<UserInfo>, IUserInfoService
    {
        public IdsRedis RedisClient { get; set; }
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override IDSContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public IdsResult<JwtUser> Login(UserInfo userInfo)
        {

            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");
            if (userInfo == null || string.IsNullOrEmpty(userInfo.Username) || string.IsNullOrEmpty(userInfo.Password))
            {
                return IdsResult<JwtUser>.failure("用户或密码不能为空");
            }
            using (var ctx = DbContext()) {
                var user = ctx.Query<UserInfo>(f => f.Username == userInfo.Username).FirstOrDefault();
                if(user==null)
                    return IdsResult<JwtUser>.failure("用户或密码不存在");
                string secrt = RsaHelper.Decrypt(user.Password, prk, true);
                if(!userInfo.Password.Equals(secrt))
                    return IdsResult<JwtUser>.failure("用户或密码错误!");
                string token = RsaHelper.Encrypt(user.Username, puk, true);
                var jwtUser = new JwtUser()
                {
                    Username = user.Username,
                    WorkNo = user.WorkNo,
                    Id= user.Id
                };
                jwtUser.Token = token;
                return IdsResult<JwtUser>.ok(jwtUser);
            }
       
        }

        public IdsResult<JwtUser> Permissions(string username)
        {
            if (string.IsNullOrEmpty(username))
                return IdsResult<JwtUser>.failure("没有权限");
            using (var ctx = DbContext()) {
                var user = ctx.Query<UserInfo>(f => f.Username == username).FirstOrDefault();
                if(user==null)
                    return IdsResult<JwtUser>.failure($"用户{username}没有权限");
                var jwtUser = new JwtUser()
                {
                    Username = user.Username,
                    WorkNo = user.WorkNo,
                    Id = user.Id
                };
                if (!string.IsNullOrEmpty(user.Permissions)) {
                    jwtUser.Permissions = user.Permissions.Split(",").ToList();
                }
                return IdsResult<JwtUser>.ok(jwtUser);
            }
        }

        public override int save(UserInfo userInfo, string?[] properites = null)
        {
            //新增用户
            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");
            string _procPwd = RsaHelper.Encrypt(userInfo.Password, puk, true);

            using (var ctx = DbContext())
            {

                if (userInfo == null || string.IsNullOrEmpty(userInfo.Username) || string.IsNullOrEmpty(userInfo.Password))
                {
                    throw  new BussinessException("用户或密码不能为空");
                }
                var user = ctx.Query<UserInfo>(f=>f.Username == userInfo.Username).FirstOrDefault();
                if (user != null)
                {
                    throw new BussinessException($"用户{userInfo.Username}已经存在不可重复创建");
                }
                using (var ts = new TransactionScope())
                {
                    int i = 0;
                    userInfo.Password = RsaHelper.Encrypt(userInfo.Password, puk, true);
                    long userId = IdUtils.Id;
                    userInfo.Id = userId+"";
                    userInfo.Status = 0;
                    userInfo.Password = _procPwd;
                    userInfo.saveInit();
                    ctx.Add(userInfo);
                    ctx.SaveChanges();
                    ts.Complete();
                    return i;
                }
            }
        }

        public int UpdatePwd(ChangeUserPassword pwdDto)
        {
            if (pwdDto.password == null || pwdDto.userName == null || pwdDto.newPassword == null)
                throw new BussinessException("参数不能为空");

            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");


            if (pwdDto.password == pwdDto.newPassword)
            {
                throw new BussinessException("The new password and the original password cannot be repeated！");
            }
            using (var ctx = DbContext())
            {
                var userInfo = ctx.Query<UserInfo>(x => x.Username == pwdDto.userName).FirstOrDefault();
                string password = RsaHelper.Decrypt(userInfo.Password, prk, true);
                if (string.IsNullOrWhiteSpace(password) || !password.Equals(pwdDto.password))
                    throw new BussinessException("原始密码不正确");
                string sercet = RsaHelper.Encrypt(pwdDto.newPassword, puk, true);
                userInfo.Password = sercet;
                userInfo.updateInit();
                return ctx.Update<UserInfo>(userInfo);
            }
        }
    }

}
