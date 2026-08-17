using IDS.Base;
using IDS.Common;
using IDS.Extension;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.IService.DTO;
using IDS.Security.IService.POCO;
using IDS.Security.Module;
using IDS.Security.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static LinqToDB.Common.Configuration;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class UserInfoAdapter : SecBaseAdapter<UserInfo>
    {
        public IUserInfoService UserInfoService { get; set; }

        public virtual void CreateUser(UserInfo userInfo) {
            UserInfoService.CreateUser(userInfo);
        }
        public virtual UserInfoVo getUser(string userName) {

            return UserInfoService.getUser(userName);
        }
        public void Check(UserInfoVo userInfo) {
            UserInfoService.Check(userInfo);
        }
        public bool CheckPasswordExpire(DateTime? date) {
            return UserInfoService.CheckPasswordExpire(date);
        }
        #region 处理和java同步
        public int BatchInsert(List<UserRole> list)
        {
              return UserInfoService.BatchInsert(list); 
        }


        public int DelUserAndRole(string data) {
            return UserInfoService.DelUserAndRole(data);
        }


        public int UpdatePwd(ChangeUserPwdDto pwdDto) { 
            return UserInfoService.UpdatePwd(pwdDto);
        }

        public int ResetPwd(ChangeUserPwdDto pwdDto) {
            return UserInfoService.ResetPwd(pwdDto);
        }

        public List<VDepartmentUser> QueryAllUser() {
            return UserInfoService.QueryAllUser();
        }

        public List<VDepartmentUser> QueryUserByIds(List<string> list)
        {
            return UserInfoService.QueryUserByIds(list);
        }


        public List<VDepartmentUser> QueryUserByUserNames(List<string> list)
        {
            return UserInfoService.QueryUserByUserNames(list);
        }
        public override ISecBaseService<UserInfo> Service()
        {
            return UserInfoService;
        }

        public UserInfoVo QueryUserRoles(String userName) {
            return UserInfoService.QueryUserRoles(userName);
        }

        public List<UserInfo> SelectDeptUser(string data) { 
           return UserInfoService.SelectDeptUser(data);
        }
        public async Task<UserSessionDto> IsLogin(UserSessionDto sessionDto) {
            return await UserInfoService.IsLogin(sessionDto);
        }
        #endregion
    }
}
