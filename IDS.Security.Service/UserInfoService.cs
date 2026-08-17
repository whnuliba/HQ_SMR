using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.IService.DTO;
using IDS.Security.Module;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;
using IDS.Base.Utils;
using IDS.Security.IService.POCO;
using System.Linq.Expressions;
using IDS.Extension;
using System.Collections.Generic;
using IDS.Persistence;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class UserInfoService : SecBaseService<UserInfo, AuthDbContext>,IUserInfoService
    {
        public IdsRedis RedisClient { get; set; }

        public IFactoryInfoService FactoryInfoService { get; set; }
        public IdsRedisLock IdsRedisLock { get; set; }
        public virtual int BatchInsert(List<UserRole> list)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {
                    try
                    {

                        int count = ctx.Count<UserRole>(c => c.UserId == list[0].UserId);

                        //插入前先删除
                         int i = ctx.Delete<UserRole>(c => c.UserId == list[0].UserId);
                        if (count != i)
                            throw new BussinessException("数据不匹配");


                        if (list.Count() == 1 && "#".Equals(list[0].RoleId))
                            return i;

                        ctx.AddRange(list);
                        ts.Complete();
                        return i;

                    }
                    catch (Exception ex)
                    {
                        Transaction.Current?.Rollback();
                    }
                }
            }
            return 0;
        }

        public virtual void CreateUser(UserInfo userInfo)
        {
            if (userInfo == null)
                throw new BussinessException("用户信息不能为空!");
            using (var ctx = DbContext())
            {
                userInfo.Id = Guid.NewGuid().ToString("N");
                userInfo.CreateDate = DateTime.Now;
                ctx.Add<UserInfo>(userInfo);
            }
        }

        public int DelUserAndRole(string data)
        {

            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {
                    int i =   ctx.Delete<UserRole>(f => f.UserId == data);
                    i = ctx.Delete<DepartmentUser>(f => f.UserId == data);
                    i = ctx.Delete<UserInfo>(f => f.Id == data);
                    ts.Complete();
                }

            }
            return 1;
        }

        public UserInfoVo getUser(string userName)
        {
            if (userName.IsNullOrEmpty())
            {
                throw new BussinessException("参数不能为空");
            }
            UserInfoVo userInfo = null;
            using (var ctx = DbContext())
            {
              var userInfo1 = ctx.UserInfo.Where(x => x.UserName == userName).Select(vu => new UserInfoVo {
                    Id = vu.Id,
                    CreateDate = vu.CreateDate,
                    CreateUser = vu.CreateUser,
                    LastModifyDate = vu.LastModifyDate,
                    LastModifyUser = vu.LastModifyUser,
                    Status = vu.Status,
                    UserName = vu.UserName,
                    RealName = vu.RealName,
                    Password = vu.Password,
                    UseState = vu.UseState,
                    Email = vu.Email,
                    Sex = vu.Sex,
                    OrgId = vu.OrgId,
                    JobId = vu.JobId,
                    Mobile = vu.Mobile,
                    AccountExpireTime = vu.AccountExpireTime,
                    PasswordExpireTime  = vu.PasswordExpireTime,
                    Alias = vu.Alias,
                    ChangePassword = vu.ChangePassword,
                    NameSpell = vu.NameSpell,
                    Lock = vu.Lock,
                }).FirstOrDefault();

                if (userInfo1 == null) {
                    throw new BussinessException("用户不存在");
                }
                userInfo1.Roles = ctx.VUserRole.Where(f=>f.UserId== userInfo1.Id).ToList();
                userInfo = userInfo1;
            }
            if (userInfo == null)
            {
                throw new BussinessException($"当前用户{userName}不存在!");
            }
            return userInfo;
        }


        public int UpdatePwd(ChangeUserPwdDto pwdDto)
        {
            if (pwdDto.password == null || pwdDto.userName == null || pwdDto.newPassword == null)
                throw new BussinessException("参数不能为空");

            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");

            CredentialVerfiy(pwdDto.newPassword);

            if (pwdDto.password == pwdDto.newPassword) {
                throw new BussinessException("The new password and the original password cannot be repeated！");
            }
            using (var ctx = DbContext())
            {
              var  userInfo = ctx.UserInfo.Where(x => x.UserName == pwdDto.userName).FirstOrDefault();
              string password = RsaHelper.Decrypt(userInfo.Password, prk, true);
                if (string.IsNullOrWhiteSpace(password) || !password.Equals(pwdDto.password))
                    throw new BussinessException("原始密码不正确");
                string sercet = RsaHelper.Encrypt(pwdDto.newPassword, puk, true);

                userInfo.Password = sercet;
                userInfo.ChangePassword = "N";
                string dayStr = AppConfig.GetConfigInfo("JwtTokenOptions:expire:password");
                if (string.IsNullOrEmpty(dayStr)) {
                    dayStr = "30";
                }
                if (!int.TryParse(dayStr, out int day)) {
                    day = 30;
                }
                DateTime dateTime = DateTime.Now.AddDays(day);
                userInfo.PasswordExpireTime = dateTime;

                userInfo.updateInit();
               return  ctx.Update<UserInfo>(userInfo);
            }
        }
        public override int save(UserInfo userInfo, string[] properties = null)
        {
            using (var ctx = DbContext())
            {

                //return ctx.Insert<UserInfo>(record);

                using (var ts = new TransactionScope())
                {
                    if (string.IsNullOrWhiteSpace(userInfo.DeptId))
                    {
                        throw new BussinessException("创建用户部门不能为空");
                    }
                    int i = 0;
                    if (!string.IsNullOrWhiteSpace(userInfo.Id))
                    {

                        //查看部门是否有变更
                        if (!string.IsNullOrWhiteSpace(userInfo.DeptId))
                        {
                            Department department = ctx.Department.Where(f => f.Id == userInfo.DeptId).FirstOrDefault();// departmentMapper.selectByPrimaryKey(userInfo.getDeptId());
                            if (department == null)
                            {
                                throw new BussinessException("当前部门已经不存在，请检查");
                            }
                            userInfo.updateInit();
                            userInfo.OrgId = department.OrgId;
                            userInfo.Password = null;
                            ctx.UpdateByPrimaryKeySelective(userInfo);
                            string update = $" update DEPARTMENT_USER " +
                                $"set DeptId = '{userInfo.DeptId}' where UserId = '{userInfo.Id}'";
                            i = ctx.Sql(update);
                            if (i <= 0)
                            {
                                DepartmentUser departmentUser = new DepartmentUser();
                                departmentUser.saveInit();
                                departmentUser.Id = BaseUtil.uuid();
                                departmentUser.DeptId = userInfo.DeptId;
                                departmentUser.UserId = userInfo.Id;
                                i = ctx.Insert(departmentUser);
                                if (i <= 0)
                                    throw new BussinessException("保存失败");
                            }

                        }
                        ts.Complete();
                        return i;
                    }
                    VDepartmentUser info = ctx.VDepartmentUser.Where(f => f.UserName == userInfo.UserName).FirstOrDefault();// userInfoMapper.queryUserInfoByUserName(userInfo.getUserName());
                    if (info != null)
                    {
                        throw new BussinessException("当前用户已存在,请重新输入。");
                    }
                    string puk = AppConfig.GetConfigInfo("RSA:publicKey");
                    string prk = AppConfig.GetConfigInfo("RSA:privateKey");
                    userInfo.Password = RsaHelper.Encrypt(userInfo.Password, puk, true);
                    string userId = BaseUtil.uuid();
                    userInfo.Id = userId;
                    userInfo.Status = 0;
                    userInfo.saveInit();
                    userInfo.ChangePassword = "Y";
                    CredentialVerfiy(userInfo.Password);
                    //创建部门信息
                    if (!string.IsNullOrWhiteSpace(userInfo.DeptId))
                    {
                        Department department = ctx.Department.Where(f => f.Id == userInfo.DeptId).FirstOrDefault();// departmentMapper.selectByPrimaryKey(userInfo.getDeptId());
                        if (department == null)
                        {
                            throw new BussinessException("当前部门已经不存在，请检查");
                        }
                        userInfo.OrgId = department.OrgId;
                        DepartmentUser departmentUser = new DepartmentUser();
                        departmentUser.saveInit();
                        departmentUser.Id = BaseUtil.uuid();
                        departmentUser.DeptId = userInfo.DeptId;
                        departmentUser.UserId = userId;
                        i = ctx.Insert(userInfo);
                        if (i <= 0)
                            throw new BussinessException("保存失败");
                        i = ctx.Insert(departmentUser);
                        if (i <= 0)
                            throw new BussinessException("保存失败");
                    }
                    ts.Complete();
                    return i;
                }
            }
        }

        public int ResetPwd(ChangeUserPwdDto pwdDto)
        {
            Assert.notEmpty(pwdDto.userName, "用户不能为空");
            pwdDto.newPassword ??= "@!#Cw123456";
            string puk = AppConfig.GetConfigInfo("RSA:publicKey");
            string prk = AppConfig.GetConfigInfo("RSA:privateKey");
            string dayStr = AppConfig.GetConfigInfo("JwtTokenOptions:expire:password");
            if (string.IsNullOrEmpty(dayStr))
            {
                dayStr = "30";
            }
            if (!int.TryParse(dayStr, out int day))
            {
                day = 30;
            }
            DateTime dateTime = DateTime.Now.AddDays(day);

            using (var ctx = DbContext())
            {
                var userInfo = ctx.UserInfo.Where(x => x.UserName == pwdDto.userName).FirstOrDefault();
                //string password = RsaHelper.Decrypt(userInfo.Password, prk, true);
                //if (string.IsNullOrWhiteSpace(password) || !password.Equals(pwdDto.password))
                //    throw new BussinessException("原始密码不正确");
                string sercet = RsaHelper.Encrypt(pwdDto.newPassword, puk, true);
                userInfo.Password = sercet;
                userInfo.ChangePassword = "Y";
                userInfo.PasswordExpireTime = dateTime;
                userInfo.updateInit();
                return ctx.Update<UserInfo>(userInfo);
            }
        }

        public List<VDepartmentUser> QueryAllUser() {

            using (var ctx = DbContext()) {
                return ctx.VDepartmentUser.ToList();
            }
        }

        public List<VDepartmentUser> QueryUserByIds(List<string> list) {
            using (var ctx = DbContext())
            {
                return ctx.VDepartmentUser.Where(x=>list.Contains(x.Id)).ToList();
            }
        }

        public List<VDepartmentUser> QueryUserByUserNames(List<string> list) {
            using (var ctx = DbContext())
            {
                return ctx.VDepartmentUser.Where(x => list.Contains(x.UserName)).ToList();
            }
        }

        public UserInfoVo QueryUserRoles(String userName)
        {
            using (var ctx = DbContext())
            {
                var userRolses = ctx.VDepartmentUser.Join(ctx.VUserRole, vu => vu.Id, vr => vr.UserId, (vu, vr) => new UserInfoVo
                {
                    Id = vu.Id,
                    CreateDate = vu.CreateDate,
                    CreateUser = vu.CreateUser,
                    LastModifyDate = vu.LastModifyDate,
                    LastModifyUser = vu.LastModifyUser,
                    Status = vu.Status,
                    UserName = vu.UserName,
                    RealName = vu.RealName,
                    Password = vu.Password,
                    UseState = vu.UseState,
                    Email = vu.Email,
                    Sex = vu.Sex,
                    OrgId = vu.OrgId,
                    JobId = vu.JobId,
                    Mobile = vu.Mobile,
                    DeptId = vu.DeptId,
                    DeptCode = vu.DeptCode,
                    DeptName = vu.DeptName,
                    DeptType = vu.DeptType,
                    DeptLeader = vu.DeptLeader,
                    DeptLeaderId = vu.DeptLeaderId,
                    DeptLeaderName = vu.DeptLeaderName,
                    UserLeader = vu.UserLeader,
                    UserLeaderId = vu.UserLeaderId,
                    UserLeaderName = vu.UserLeaderName,
                    OrgCode = vu.OrgCode,
                    OrgName = vu.OrgName,
                    UserRole = vr,
                }).Where(f => f.UserName == userName || f.Id == userName).ToList();

                if (userRolses.Count == 0)
                {
                    return null;
                }

                var userInfo = userRolses[0];
                userInfo.Roles = new List<VUserRole>();
                userRolses.ForEach(f =>
                {
                    userInfo.Roles.Add(f.UserRole);
                });

                List<String> roleIds = userInfo.Roles.Select(c => c.Id).ToList();
                if (roleIds != null && roleIds.Count() > 0)
                {
                    List<FactoryInfo> factoryInfos = FactoryInfoService.SelectFactoryByRole(roleIds);
                    if (factoryInfos != null && factoryInfos.Count() > 0)
                    {
                        List<String> facs = factoryInfos.Select(c=>c.FactoryNo).Distinct().ToList();
                        userInfo.Factory =facs;
                        userInfo.FactoryInfo=factoryInfos;
                    }
                }
                userInfo.UserRole = null;
                return userInfo;
            }
           
        }


        public override  Page<UserInfo> List(Page<UserInfo> page, Expression<Func<UserInfo, bool>> predicate=null) {
            using (var ctx = DbContext()) {

                Expression<Func<VDepartmentUser, bool>> where = null;
                if (page.requestData != null)
                {
                    if (!string.IsNullOrWhiteSpace(page.requestData.RealName) || !string.IsNullOrWhiteSpace(page.requestData.UserName))
                        where = f => f.UserName == page.requestData.UserName || (!string.IsNullOrWhiteSpace(page.requestData.RealName) && f.RealName.StartsWith(page.requestData.RealName));
                }
                var req = page.requestData;
                var data = ctx.Query<VDepartmentUser>(where).Skip((page.current - 1) * page.pageSize).Take(page.pageSize).Select(vu => new UserInfo {
                    Id = vu.Id,
                    CreateDate = vu.CreateDate,
                    CreateUser = vu.CreateUser,
                    LastModifyDate = vu.LastModifyDate,
                    LastModifyUser = vu.LastModifyUser,
                    Status = vu.Status,
                    UserName = vu.UserName,
                    RealName = vu.RealName,
                    Password = vu.Password,
                    UseState = vu.UseState,
                    Email = vu.Email,
                    Sex = vu.Sex,
                    OrgId = vu.OrgId,
                    JobId = vu.JobId,
                    Mobile = vu.Mobile,
                    DeptId = vu.DeptId,
                    DeptCode = vu.DeptCode,
                    DeptName = vu.DeptName,
                    DeptLeader = vu.DeptLeader,
                    DeptLeaderId = vu.DeptLeaderId,
                    DeptLeaderName = vu.DeptLeaderName,
                    Leader = vu.UserLeader,
                    LeaderId = vu.UserLeaderId,
                    LeaderName = vu.UserLeaderName,
                    OrgCode = vu.OrgCode,
                    OrgName = vu.OrgName,
                    AccountExpireTime = vu.AccountExpireTime,
                    PasswordExpireTime = vu.PasswordExpireTime,
                    Alias = vu.Alias,
                    ChangePassword = vu.ChangePassword,
                    NameSpell = vu.NameSpell,
                    Lock = vu.Lock,

                }).ToList();
                data.ForEach(item =>
                {
                    item.Roles = new List<RoleInfo>();
                });
                var count = ctx.Count<VDepartmentUser>(where);
                Page<UserInfo> pages = new Page<UserInfo>(count, data, page.pageSize, page.current);

                if (pages != null && pages.data.Count() > 0)
                 {
                    var ids = pages.data.Select(c => c.Id).ToList();
                    var roles = (from vr in ctx.VRoleAndGroup join ur in ctx.UserRole on vr.Id equals ur.RoleId
                                where ids.Contains(ur.UserId) select new RoleInfo { 
                                   Id = vr.Id,
                                   UserId = ur.UserId,
                                   RoleCode=vr.RoleCode,
                                   RoleName=vr.RoleName,
                                   RoleType=vr.RoleType
                                }).ToList();
                    var list = pages.data;
                    if (roles.Count() == 0)
                        return pages;

                    var roleMap = roles.GroupBy(c => c.UserId).ToDictionary(k => k.Key, r => r.ToList());
                    list.ForEach(item=>{
                        if (roleMap.ContainsKey(item.Id))
                        {
                            item.Roles=roleMap[item.Id];
                        }
                    });
                    return pages;
                }
                throw new BussinessException("用户信息不存在!");
            }
 
        
        }

        public List<UserInfo> SelectDeptUser(string data) {
            using (var ctx = DbContext()) {


                var query = from vu in ctx.UserInfo join dept in ctx.DepartmentUser on vu.Id equals dept.UserId
                        where dept.DeptId == data select new UserInfo {

                            Id = vu.Id,
                            CreateDate = vu.CreateDate,
                            CreateUser = vu.CreateUser,
                            LastModifyDate = vu.LastModifyDate,
                            LastModifyUser = vu.LastModifyUser,
                            Status = vu.Status,
                            UserName = vu.UserName,
                            RealName = vu.RealName,
                            Password = vu.Password,
                            UseState = vu.UseState,
                            Email = vu.Email,
                            Sex = vu.Sex,
                            OrgId = vu.OrgId,
                            JobId = vu.JobId,
                            Mobile = vu.Mobile,
                            DeptId = dept.DeptId,
                            AccountExpireTime = vu.AccountExpireTime,
                            PasswordExpireTime = vu.PasswordExpireTime,
                            Alias = vu.Alias,
                            ChangePassword = vu.ChangePassword,
                            NameSpell = vu.NameSpell,
                            Lock = vu.Lock,
                        };
                List < UserInfo > users = query.ToList();
                List<String> ids = users.Select(c=>c.Id).ToList();
                if (ids.Count() > 0)
                {

                    var roleQuery = from vr in ctx.VRoleAndGroup join ur in ctx.UserRole on vr.Id equals ur.RoleId
                            where ids.Contains(ur.UserId) select new RoleInfo {
                                        Id= vr.Id,
                                        RoleCode =vr.RoleCode,
                                        RoleName=vr.RoleName,
                                        UseState=vr.UseState,
                                        UserId = ur.UserId,
                                        RoleType=vr.RoleType

                            };
                    List < RoleInfo > roleInfos = roleQuery.ToList();
                    Dictionary<String, List<RoleInfo>> roleMap = roleInfos.GroupBy(c=>c.UserId).ToDictionary(k=>k.Key,v=>v.ToList());
                    users.ForEach(item=>{
                        if (roleMap.ContainsKey(item.Id))
                        {
                            item.Roles = roleMap[item.Id];
                        }
                    });
                }
                return users;
            }
        
        }
        public void CredentialVerfiy(string credential) {

            if (string.IsNullOrWhiteSpace(credential))
                throw new BussinessException("password is required");
            //需要包含大小写，特殊字符，
            if (credential.Length < 8)
                throw new BussinessException("The password length must not be less than 8 characters");
            string chinese = "[\u4e00-\u9fa5]";
            string rexCharacter = "[\\x20-\\x2f\\x3a-x40\\x5b-x60\\x7b-x7e]+";
            string letter = "[A-Z]";
            string letterLow = "[a-z]";
            string number = "[0-9]";
            char [] charArry = credential.ToCharArray();
            bool isNumber = false;
            bool isCharacter = false;
            bool isLowLetter = false;
            bool isLetter = false;
            foreach (var c in charArry) {
                if ((c >= 32 && c <= 47) || (c >= 58 && c <= 64) || (c >= 91 && c <= 60) || (c >= 123 && c <= 126))
                {
                    isCharacter = true;
                    continue;
                }
                else if (c >= 48 && c <= 57)
                {
                    isNumber = true;
                    continue;
                }
                else if (c >= 65 && c <= 90)
                {
                    isLetter = true;
                    continue;
                }
                else if (c >= 97 && c <= 122) {
                    isLowLetter = true;
                    continue;
                }
                if (isCharacter && isNumber && isLetter && isLowLetter)
                    break;
            }
            if (!isCharacter || !isNumber || !isLetter || !isLowLetter) {
                throw new BussinessException("The password must contain capital letters, lowercase letters, numbers and special characters");
            }
        }
        string LOGIN_PREFIX = "user_login_";
        public async Task<UserSessionDto> IsLogin(UserSessionDto userSessionDto)
        {

            if (userSessionDto == null || string.IsNullOrEmpty(userSessionDto.UserName))
                throw new BussinessException("login user error!");

            string lockStr = "SAAS:COMMON:USER_LOCK:" + userSessionDto.UserName;
            string value = BaseUtil.uuid();
            try
            {
                if (IdsRedisLock.Lock(lockStr, value, TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        string global = AppConfig.GetConfigInfo("AppArguments:SignKey")??"";
                        //首次创建才能生成session,72小时过期
                        if (userSessionDto.State != null && userSessionDto.State == 100)
                        {
                            string sk = BaseUtil.uuid();
                            userSessionDto.SessionKey=sk;
                            DateTime dateTime  = DateTime.Now.AddHours(72);
                           await RedisClient.SetCache(LOGIN_PREFIX + global + "_" + userSessionDto.UserName, sk, dateTime);
                            return userSessionDto;
                        }
                        //判断是否有待key
                        var sessKey =  await RedisClient.GetCache<string>(LOGIN_PREFIX + global + "_" + userSessionDto.UserName);
                        if (!string.IsNullOrEmpty(userSessionDto.SessionKey))
                        {
                            if (sessKey == null || string.IsNullOrEmpty(sessKey))
                            {
                                //说明已经被踢了
                                userSessionDto.SessionKey =BaseUtil.uuid();
                                return userSessionDto;
                            }
                            if (sessKey.Equals(userSessionDto.SessionKey))
                            {
                                //redisClientService.set(UserSessionDto.LOGIN_PREFIX +isLoginKey+"_"+ userSessionDto.getUserName(), userSessionDto.getSessionKey(), 3600*72);
                                return userSessionDto;
                            }
                            userSessionDto.SessionKey = sessKey;
                            return userSessionDto;
                            //userSessionDto.setSessionKey(sessKey.toString());
                        }
                        //userSessionDto.setSessionKey(BaseUtil.getUUID());
                        //redisClientService.set(UserSessionDto.LOGIN_PREFIX+isLoginKey+"_"+userSessionDto.getUserName(),userSessionDto.getSessionKey(),3600*72);
                        //return userSessionDto;
                    }
                    catch (Exception ex)
                    {
                        throw new BussinessException(ex.Message);
                    }
                    finally
                    {
                        IdsRedisLock.UnLock(lockStr, value);
                    }
                }
            }
            catch (Exception e)
            {
                throw new BussinessException(e.Message);
            }
            return userSessionDto;
        }
        public bool CheckPasswordExpire(DateTime? date)
        {
            if (date == null) return false;
            if (date < DateTime.Now)
            {
                return true;
            }
            return false;
        }
        public void Check(UserInfoVo userInfo)
        {
            if(userInfo == null)
                throw new BussinessException("User information is null");
            if (userInfo.AccountExpireTime != null && userInfo.AccountExpireTime < DateTime.Now) {
                //Failed to authenticate since user account has expired
                throw new BussinessException("Failed to authenticate since user account has expired");
            }
            else if (!string.IsNullOrEmpty(userInfo.Lock) && userInfo.Lock == "Y") {
                throw new BussinessException("Failed to authenticate since user account is locked");
            }
            else if (userInfo.UseState!=null && userInfo.UseState==1)
            {
                throw new BussinessException("Failed to authenticate since user account is disabled");
            }
        }
    }
}
