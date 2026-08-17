using IDS.Base;
using IDS.Security.IService.DTO;
using IDS.Security.IService.POCO;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IUserInfoService: ISecBaseService<UserInfo>
    {
        public int BatchInsert(List<UserRole> list);

        public void CreateUser(UserInfo userInfo);
        public UserInfoVo getUser(string userName);
        public int DelUserAndRole(string data);
        public int UpdatePwd(ChangeUserPwdDto pwdDto);
        int ResetPwd(ChangeUserPwdDto pwdDto);
        List<VDepartmentUser> QueryAllUser();
        List<VDepartmentUser> QueryUserByIds(List<string> list);
        List<VDepartmentUser> QueryUserByUserNames(List<string> list);
        //List<VDepartmentUser> selectDeptUser(String deptId);
       UserInfoVo QueryUserRoles(String userName);
       List<UserInfo> SelectDeptUser(string data);
        Task<UserSessionDto> IsLogin(UserSessionDto sessionDto);
        void Check(UserInfoVo userInfo);
        public bool CheckPasswordExpire(DateTime? date);

    }
}
