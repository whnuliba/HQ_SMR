using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.IService
{
    public interface IUserInfoService : IDbLongBaseService<UserInfo>
    {
        public int UpdatePwd(ChangeUserPassword pwdDto);
        public IdsResult<JwtUser> Login(UserInfo user);
    }

    public class ChangeUserPassword
    {
        public String? userName { set; get; }
        public String? password { set; get; }
        public String? newPassword { set; get; }
    }


    public class JwtUser:UserInfo
    {
        public string? Token { set; get; }
    }
}
