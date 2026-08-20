using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Adapter
{
    [AutoInjection]
    public class UserInfoAdapter : DbBaseAdapter<UserInfo>
    {
        public IUserInfoService _userInfoService { set; get; }
        public override IDbBaseService<UserInfo> Service()
        {
           return _userInfoService;
        }

        public int UpdatePwd(ChangeUserPassword pwdDto) {
           return  _userInfoService.UpdatePwd(pwdDto);
        }
        public IdsResult<JwtUser> Login(UserInfo user) {
            return _userInfoService.Login(user);
        }
        public IdsResult<JwtUser> Permissions(string username) {
            return _userInfoService.Permissions(username);
        }
    }
}
