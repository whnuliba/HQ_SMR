using IDS.Base;
using IDS.Common;
using IDS.Extension;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.IService.DTO;
using IDS.Security.IService.POCO;
using IDS.Security.Module;
using log4net.Core;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class MenuInfoService : SecBaseService<MenuInfo, AuthDbContext>, IMenuInfoService
    {

        public IRoleInfoService RoleInfoService { get; set; }

        private const string SEQ_LOCK="MENU_LOCK";
        private const string SEQ_PREFIX="MF_";
        private const int SEQ_LEN = 6;

        public SequenceGeneratorService SequenceGeneratorService { get; set; }

        public override  int save(MenuInfo menuInfo, string?[] properites = null)
        {
            String menuGrp = menuInfo.MenuGroup;
            int menuType = menuInfo.MenuType;
            if (string.IsNullOrWhiteSpace(menuGrp) || menuType == null)
                throw new BussinessException("参数为空");
            if (string.IsNullOrWhiteSpace(menuInfo.MenuCode))
            {
                IdsResult<string> res =  SequenceGeneratorService.GeneratorNo(SEQ_PREFIX, SEQ_PREFIX + menuInfo.MenuGroup.ToUpper(), SEQ_LEN, SEQ_LOCK);
                if (!res.Success)
                {
                    throw new BussinessException("没有获取到菜单编码");
                }
                menuInfo.MenuCode = res.Data;
            }
            using (var ctx = DbContext()) {
            if (string.IsNullOrWhiteSpace(menuInfo.Id))
            {
                int count = ctx.Count<MenuInfo>(f => f.MenuType == menuType && f.MenuGroup == menuGrp); //menuInfoMapper.findCountByTypeAndGroup(menuGrp, menuType);
                if (menuType == 2 && count >= 1)
                    throw new BussinessException(string.Format("当前菜单组{0}和菜单类型{1},已经存在", menuGrp, menuType));
            }
            }
            return base.save(menuInfo, properites);
        }



        //public override int deleteById(string id)
        //{
        //    using (var ctx = DbContext())
        //    {
        //        try
        //        {
        //            //判断需要删除的菜单是否来之菜单组
        //            String currusername = CurrentUser.GetUserInfo()?.UserName;
        //            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
        //            if (!IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) && !RoleInfoService.IsSupperAdmin(currusername))
        //            {
        //                throw new BussinessException("你没有权限对该数据执行删除操作");
        //            }
        //            MenuGrpInfo menuGrpInfo = ctx.MenuGrpInfo.Where(f => f.Id == id).FirstOrDefault();// menuGrpInfoMapper.selectByPrimaryKey(id);
        //            List<MenuInfo> menuInfos = new List<MenuInfo>();
        //            GetMenuAndSubMenu(id, menuInfos);
        //            int i = 0;
        //            if (menuGrpInfo == null || menuInfos.Count() >= 0)
        //            {
        //                List<String> ids = menuInfos.Select(c => c.Id).ToList();
        //                ids.Add(id);
        //                i = ctx.Delete<MenuInfo>(f => ids.Contains(f.Id));
        //                i += ctx.Delete<RoleFunction>(f => ids.Contains(f.FuncId));
        //                i += ctx.Delete<AllowAuthorized>(f => ids.Contains(f.FuncId));
        //            }
        //            //同步删除菜单组
        //            if (menuGrpInfo != null)
        //                i += ctx.Delete<MenuGrpInfo>(f => f.Id == id); //menuGrpInfoMapper.deleteByPrimaryKey(id);
        //            return i;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }

        //    }
        //}

        public List<MenuTree> GetAllMenuByUserName(string menuGroup)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");

            using (var ctx = DbContext())
            {
                List<MenuInfo> menuInfos = null;
                List<int> types = new List<int> { 0, 2 };
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
                {
                    var vmenuInfos = ctx.VOrgMenuInfo.Where(f => f.Status == 1 && types.Contains(f.MenuType) && f.MenuGroup == menuGroup).OrderBy(f => f.Sort);
                    menuInfos = vmenuInfos.Distinct().Select(menu => new MenuInfo
                    {
                        Id = menu.Id,
                        CreateDate = menu.CreateDate ?? DateTime.Now,
                        CreateUser = menu.CreateUser,
                        LastModifyDate = menu.LastModifyDate,
                        LastModifyUser = menu.LastModifyUser,
                        Status = menu.Status,
                        MenuRoute = menu.MenuRoute,
                        MenuName = menu.MenuName,
                        MenuNameEn = menu.MenuNameEn,
                        MenuCode = menu.MenuCode,
                        Pid = menu.Pid,
                        Sort = menu.Sort,
                        MenuType = menu.MenuType,
                        TextIcon = menu.TextIcon,
                        MenuGroup = menu.MenuGroup,
                        Href = menu.Href,
                        Component = menu.Component,
                        OrgId = menu.OrgId,
                        Platform = menu.Platform,
                        Udf1 = menu.Udf1,
                        Udf2 = menu.Udf2,
                        Udf3 = menu.Udf3,
                        Udf4 = menu.Udf4,
                        Udf5 = menu.Udf5,
                        Udf6 = menu.Udf6

                    }).ToList();
                    var s = menuInfos.Select(c => c.MenuRoute).ToList();
                }
                else
                {

                    var dms = from menu in ctx.VUserRoleFunction
                              join auth in ctx.AllowAuthorized on menu.Id equals auth.FuncId
                              where menu.Status == 1 && types.Contains(menu.MenuType) && menu.MenuGroup == menuGroup && menu.UserName == currusername
                              && ((menu.Scope == "0" && (from vd in ctx.VUserOrgDepartment where vd.UserName == currusername select vd.OrgId).ToList().Contains(menu.OrgId)) || menu.Scope == "1")
                              select new MenuInfo {
                                  Id = menu.Id,
                                  CreateDate = menu.CreateDate??DateTime.Now,
                                  CreateUser = menu.CreateUser,
                                  LastModifyDate = menu.LastModifyDate,
                                  LastModifyUser = menu.LastModifyUser,
                                  Status = menu.Status,
                                  MenuRoute = menu.MenuRoute,
                                  MenuName = menu.FuncName,
                                  MenuNameEn = menu.MenuNameEn,
                                  MenuCode = menu.FuncCode,
                                  Pid = menu.Pid,
                                  Sort = menu.Sort,
                                  MenuType = menu.MenuType,
                                  TextIcon = menu.TextIcon,
                                  MenuGroup = menu.MenuGroup,
                                  Href = menu.Href,
                                  Component = menu.Component,
                                  OrgId = menu.OrgId,
                                  Platform = menu.Platform,
                                  Udf1 = menu.Udf1,
                                  Udf2 = menu.Udf2,
                                  Udf3 = menu.Udf3,
                                  Udf4 = menu.Udf4,
                                  Udf5 = menu.Udf5,
                                  Udf6 = menu.Udf6

                              };
                    menuInfos = dms.Distinct().ToList();


                }
                if (null == menuInfos || menuInfos.Count() == 0)
                    return null;
                return MenuTree.createMenu(menuInfos);

            }



        }

        public List<MenuTree> GetAllMenuTree() {

            List<MenuInfo> menuInfos = null;
            using (var ctx = DbContext())
            {
                List<int> types = new List<int> { 0,1,2 };
                menuInfos = ctx.VOrgMenuInfo.Distinct().Select(menu => new MenuInfo
                {
                    Id = menu.Id,
                    CreateDate = menu.CreateDate ?? DateTime.Now,
                    CreateUser = menu.CreateUser,
                    LastModifyDate = menu.LastModifyDate,
                    LastModifyUser = menu.LastModifyUser,
                    Status = menu.Status,
                    MenuRoute = menu.MenuRoute,
                    MenuName = menu.MenuName,
                    MenuNameEn = menu.MenuNameEn,
                    MenuCode = menu.MenuCode,
                    Pid = menu.Pid,
                    Sort = menu.Sort,
                    MenuType = menu.MenuType,
                    TextIcon = menu.TextIcon,
                    MenuGroup = menu.MenuGroup,
                    Href = menu.Href,
                    Component = menu.Component,
                    OrgId = menu.OrgId,
                    Platform = menu.Platform,
                    Udf1 = menu.Udf1,
                    Udf2 = menu.Udf2,
                    Udf3 = menu.Udf3,
                    Udf4 = menu.Udf4,
                    Udf5 = menu.Udf5,
                    Udf6 = menu.Udf6

                }).ToList();
                if (null == menuInfos || menuInfos.Count() == 0)
                    return null;
            }
            return MenuTree.createAllTreeMenu(menuInfos);
        }
        public List<string> QueryAllMenuGroupByUsername(String username)
        {
            using (var ctx = DbContext())
            {

                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(username) || RoleInfoService.IsSupperAdmin(username))
                {
                    var codes = ctx.MenuInfo.Where(c => c.Status == 1 && c.MenuType == 0 && c.MenuGroup != null).Select(f => f.MenuGroup).Distinct().ToList();
                   var  gcodes = ctx.Query<MenuGrpInfo>(f => codes.Contains(f.GroupCode) && f.Status == 1).Select(f => f.GroupCode);
                    return gcodes.ToList();
                }
                else
                {
                    var codes = from func in ctx.VUserRoleFunction
                                join auth in ctx.AllowAuthorized on func.Id equals auth.FuncId
                                where func.Status == 1 && func.MenuType == 0 && func.UserName == username && func.Pid != "0" && func.MenuGroup != null
                                && ((func.Scope == "0" && (from vd in ctx.VUserOrgDepartment where vd.UserName == username select vd.OrgId).ToList().Contains(func.OrgId)) || func.Scope == "1")
                                select func.MenuGroup;
                    var lst = codes.Distinct().ToList();
                    codes =  ctx.Query<MenuGrpInfo>(f => lst.Contains(f.GroupCode) && f.Status == 1).Select(f=>f.GroupCode);
                    return codes.Distinct().ToList();
                }
            }

        }
        public List<MenuTree> GetAllMenuByUserNameAndMenuName(String menuGroup, String menuStr)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            List<MenuInfo> menuInfos = null;
            using (var ctx = DbContext())
            {
                List<int> types = new List<int> { 0, 2 };
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
                {
                    List<VOrgMenuInfo> vmenuInfos = ctx.VOrgMenuInfo.Where(f => f.Status == 1 && types.Contains(f.MenuType) && f.MenuGroup == menuGroup).OrderBy(f => f.Sort).ToList();
                    menuInfos = vmenuInfos.Select(menu => new MenuInfo
                    {
                        Id = menu.Id,
                        CreateDate = menu.CreateDate ?? DateTime.Now,
                        CreateUser = menu.CreateUser,
                        LastModifyDate = menu.LastModifyDate,
                        LastModifyUser = menu.LastModifyUser,
                        Status = menu.Status,
                        MenuRoute = menu.MenuRoute,
                        MenuName = menu.MenuName,
                        MenuNameEn = menu.MenuNameEn,
                        MenuCode = menu.MenuCode,
                        Pid = menu.Pid,
                        Sort = menu.Sort,
                        MenuType = menu.MenuType,
                        TextIcon = menu.TextIcon,
                        MenuGroup = menu.MenuGroup,
                        Href = menu.Href,
                        Component = menu.Component,
                        OrgId = menu.OrgId,
                        Platform = menu.Platform,
                        Udf1 = menu.Udf1,
                        Udf2 = menu.Udf2,
                        Udf3 = menu.Udf3,
                        Udf4 = menu.Udf4,
                        Udf5 = menu.Udf5,
                        Udf6 = menu.Udf6

                    }).ToList();
                }
                else
                {

                    var dms = from menu in ctx.VUserRoleFunction
                              join auth in ctx.AllowAuthorized on menu.Id equals auth.FuncId
                              where menu.Status == 1 && types.Contains(menu.MenuType) && menu.MenuGroup == menuGroup && menu.UserName == currusername
                              && ((menu.Scope == "0" && (from vd in ctx.VUserOrgDepartment where vd.UserName == currusername select vd.OrgId).ToList().Contains(menu.OrgId)) || menu.Scope == "1")
                              select  new MenuInfo
                              {
                                  Id = menu.Id,
                                  CreateDate = menu.CreateDate ?? DateTime.Now,
                                  CreateUser = menu.CreateUser,
                                  LastModifyDate = menu.LastModifyDate,
                                  LastModifyUser = menu.LastModifyUser,
                                  Status = menu.Status,
                                  MenuRoute = menu.MenuRoute,
                                  MenuName = menu.FuncName,
                                  MenuNameEn = menu.MenuNameEn,
                                  MenuCode = menu.FuncCode,
                                  Pid = menu.Pid,
                                  Sort = menu.Sort,
                                  MenuType = menu.MenuType,
                                  TextIcon = menu.TextIcon,
                                  MenuGroup = menu.MenuGroup,
                                  Href = menu.Href,
                                  Component = menu.Component,
                                  OrgId = menu.OrgId,
                                  Platform = menu.Platform,
                                  Udf1 = menu.Udf1,
                                  Udf2 = menu.Udf2,
                                  Udf3 = menu.Udf3,
                                  Udf4 = menu.Udf4,
                                  Udf5 = menu.Udf5,
                                  Udf6 = menu.Udf6

                              };

                    menuInfos = dms.Distinct().ToList();
                }
                if (null == menuInfos || menuInfos.Count() == 0)
                    return null;
                if (string.IsNullOrEmpty(menuStr))
                    return MenuTree.createMenu(menuInfos);
                return MenuTree.createSearchMenu(menuInfos, menuStr);
            }

        }


        public List<MenuTree> GetAllGroupMenuByUserName(List<String> menuGroup)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            List<MenuInfo> menuInfos = null;
            using (var ctx = DbContext())
            {
                List<int> types = new List<int> { 0, 2 };
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
                {
                    var vmenuInfos = ctx.VOrgMenuInfo.Where(f => f.Status == 1 && types.Contains(f.MenuType) && menuGroup.Contains(f.MenuGroup)).Distinct().ToList();
                    menuInfos = vmenuInfos.Select(menu => new MenuInfo
                    {
                        Id = menu.Id,
                        CreateDate = menu.CreateDate ?? DateTime.Now,
                        CreateUser = menu.CreateUser,
                        LastModifyDate = menu.LastModifyDate,
                        LastModifyUser = menu.LastModifyUser,
                        Status = menu.Status,
                        MenuRoute = menu.MenuRoute,
                        MenuName = menu.MenuName,
                        MenuNameEn = menu.MenuNameEn,
                        MenuCode = menu.MenuCode,
                        Pid = menu.Pid,
                        Sort = menu.Sort,
                        MenuType = menu.MenuType,
                        TextIcon = menu.TextIcon,
                        MenuGroup = menu.MenuGroup,
                        Href = menu.Href,
                        Component = menu.Component,
                        OrgId = menu.OrgId,
                        Platform = menu.Platform,
                        Udf1 = menu.Udf1,
                        Udf2 = menu.Udf2,
                        Udf3 = menu.Udf3,
                        Udf4 = menu.Udf4,
                        Udf5 = menu.Udf5,
                        Udf6 = menu.Udf6

                    }).ToList();
                }
                else
                {
                    var dms = from menu in ctx.VUserRoleFunction
                              join auth in ctx.AllowAuthorized on menu.Id equals auth.FuncId
                              where menu.Status == 1 && types.Contains(menu.MenuType) && menuGroup.Contains(menu.MenuGroup) && menu.UserName == currusername
                              && ((menu.Scope == "0" && (from vd in ctx.VUserOrgDepartment where vd.UserName == currusername select vd.OrgId).ToList().Contains(menu.OrgId)) || menu.Scope == "1")
                              select new MenuInfo
                              {
                                  Id = menu.Id,
                                  CreateDate = menu.CreateDate ?? DateTime.Now,
                                  CreateUser = menu.CreateUser,
                                  LastModifyDate = menu.LastModifyDate,
                                  LastModifyUser = menu.LastModifyUser,
                                  Status = menu.Status,
                                  MenuRoute = menu.MenuRoute,
                                  MenuName = menu.FuncName,
                                  MenuNameEn = menu.MenuNameEn,
                                  MenuCode = menu.FuncCode,
                                  Pid = menu.Pid,
                                  Sort = menu.Sort,
                                  MenuType = menu.MenuType,
                                  TextIcon = menu.TextIcon,
                                  MenuGroup = menu.MenuGroup,
                                  Href = menu.Href,
                                  Component = menu.Component,
                                  OrgId = menu.OrgId,
                                  Platform = menu.Platform,
                                  Udf1 = menu.Udf1,
                                  Udf2 = menu.Udf2,
                                  Udf3 = menu.Udf3,
                                  Udf4 = menu.Udf4,
                                  Udf5 = menu.Udf5,
                                  Udf6 = menu.Udf6

                              };

                    menuInfos = dms.Distinct().ToList();


                }
                if (null == menuInfos || menuInfos.Count() == 0)
                    return null;
            }
            return MenuTree.createAllMenu(menuInfos);
        }


        public List<MenuInfo> QueryAllMenus()
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            List<MenuInfo> menuInfos = null;
            using (var ctx = DbContext())
            {
                List<int> types = new List<int> { 0, 1, 2 };
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
                {

                    var vmenuInfos = ctx.VOrgMenuInfo.Where(f => types.Contains(f.MenuType)).ToList();
                    menuInfos = vmenuInfos.Select(menu => new MenuInfo
                    {
                        Id = menu.Id,
                        CreateDate = menu.CreateDate ?? DateTime.Now,
                        CreateUser = menu.CreateUser,
                        LastModifyDate = menu.LastModifyDate,
                        LastModifyUser = menu.LastModifyUser,
                        Status = menu.Status,
                        MenuRoute = menu.MenuRoute,
                        MenuName = menu.MenuName,
                        MenuNameEn = menu.MenuNameEn,
                        MenuCode = menu.MenuCode,
                        Pid = menu.Pid,
                        Sort = menu.Sort,
                        MenuType = menu.MenuType,
                        TextIcon = menu.TextIcon,
                        MenuGroup = menu.MenuGroup,
                        Href = menu.Href,
                        Component = menu.Component,
                        OrgId = menu.OrgId,
                        Platform = menu.Platform,
                        Udf1 = menu.Udf1,
                        Udf2 = menu.Udf2,
                        Udf3 = menu.Udf3,
                        Udf4 = menu.Udf4,
                        Udf5 = menu.Udf5,
                        Udf6 = menu.Udf6

                    }).ToList();
                    return menuInfos;
                }

                var dms = from menu in ctx.VOrgMenuInfo
                          join uod in ctx.VUserOrgDepartment on menu.OrgId equals uod.OrgId
                          where types.Contains(menu.MenuType) && uod.UserName == currusername
                          orderby menu.Sort ascending
                          select new MenuInfo
                          {
                              Id = menu.Id,
                              CreateDate = menu.CreateDate ?? DateTime.Now,
                              CreateUser = menu.CreateUser,
                              LastModifyDate = menu.LastModifyDate,
                              LastModifyUser = menu.LastModifyUser,
                              Status = menu.Status,
                              MenuRoute = menu.MenuRoute,
                              MenuName = menu.MenuName,
                              MenuNameEn = menu.MenuNameEn,
                              MenuCode = menu.MenuCode,
                              Pid = menu.Pid,
                              Sort = menu.Sort,
                              MenuType = menu.MenuType,
                              TextIcon = menu.TextIcon,
                              MenuGroup = menu.MenuGroup,
                              Href = menu.Href,
                              Component = menu.Component,
                              OrgId = menu.OrgId,
                              Platform = menu.Platform,
                              Udf1 = menu.Udf1,
                              Udf2 = menu.Udf2,
                              Udf3 = menu.Udf3,
                              Udf4 = menu.Udf4,
                              Udf5 = menu.Udf5,
                              Udf6 = menu.Udf6

                          };


                menuInfos = dms.Distinct().ToList().Select(f =>
                {
                    var menu = new MenuInfo();
                    ObjectExtensions.CopyProperties(f, menu);
                    return menu;
                }).ToList();
                return menuInfos;
            }
        }


        public List<MenuInfo> GetButton(String menuGroup)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            using (var ctx = DbContext())
            {
                var dms = ctx.VUserRoleFunction.Where(f => f.UserName == currusername && f.MenuGroup == menuGroup).OrderBy(f => f.Sort).Distinct().Select(menu=> new MenuInfo
                {
                    Id = menu.Id,
                    CreateDate = menu.CreateDate ?? DateTime.Now,
                    CreateUser = menu.CreateUser,
                    LastModifyDate = menu.LastModifyDate,
                    LastModifyUser = menu.LastModifyUser,
                    Status = menu.Status,
                    MenuRoute = menu.MenuRoute,
                    MenuName = menu.FuncName,
                    MenuNameEn = menu.MenuNameEn,
                    MenuCode = menu.FuncCode,
                    Pid = menu.Pid,
                    Sort = menu.Sort,
                    MenuType = menu.MenuType,
                    TextIcon = menu.TextIcon,
                    MenuGroup = menu.MenuGroup,
                    Href = menu.Href,
                    Component = menu.Component,
                    OrgId = menu.OrgId,
                    Platform = menu.Platform,
                    Udf1 = menu.Udf1,
                    Udf2 = menu.Udf2,
                    Udf3 = menu.Udf3,
                    Udf4 = menu.Udf4,
                    Udf5 = menu.Udf5,
                    Udf6 = menu.Udf6

                });
                return dms.ToList(); 
            }
        }

        public List<MenuInfo> QueryAllMenusByPid(String Pid)
        {
            List<int> types = new List<int> { 0, 1, 2 };
            using (var ctx = DbContext())
            {
                var vmenuInfos = ctx.VOrgMenuInfo.Where(f => f.Pid == Pid && types.Contains(f.MenuType)).Distinct().Select(menu=> new MenuInfo
                {
                    Id = menu.Id,
                    CreateDate = menu.CreateDate ?? DateTime.Now,
                    CreateUser = menu.CreateUser,
                    LastModifyDate = menu.LastModifyDate,
                    LastModifyUser = menu.LastModifyUser,
                    Status = menu.Status,
                    MenuRoute = menu.MenuRoute,
                    MenuName = menu.MenuName,
                    MenuNameEn = menu.MenuNameEn,
                    MenuCode = menu.MenuCode,
                    Pid = menu.Pid,
                    Sort = menu.Sort,
                    MenuType = menu.MenuType,
                    TextIcon = menu.TextIcon,
                    MenuGroup = menu.MenuGroup,
                    Href = menu.Href,
                    Component = menu.Component,
                    OrgId = menu.OrgId,
                    Platform = menu.Platform,
                    Udf1 = menu.Udf1,
                    Udf2 = menu.Udf2,
                    Udf3 = menu.Udf3,
                    Udf4 = menu.Udf4,
                    Udf5 = menu.Udf5,
                    Udf6 = menu.Udf6

                });
                List<MenuInfo> menuInfos = vmenuInfos.ToList();
                return menuInfos;
            }

        }

        public void GetMenuAndSubMenu(List<String> pid, List<MenuInfo> menuInfoList)
        {
            if (pid == null || pid.Count() == 0)
                throw new BussinessException("菜单ID是空，请检查");

            using (var ctx = DbContext())
            {
                var vmenuInfos = ctx.VOrgMenuInfo.Where(f => pid.Contains(f.Pid)).Distinct().Select(menu=> new MenuInfo
                {
                    Id = menu.Id,
                    CreateDate = menu.CreateDate ?? DateTime.Now,
                    CreateUser = menu.CreateUser,
                    LastModifyDate = menu.LastModifyDate,
                    LastModifyUser = menu.LastModifyUser,
                    Status = menu.Status,
                    MenuRoute = menu.MenuRoute,
                    MenuName = menu.MenuName,
                    MenuNameEn = menu.MenuNameEn,
                    MenuCode = menu.MenuCode,
                    Pid = menu.Pid,
                    Sort = menu.Sort,
                    MenuType = menu.MenuType,
                    TextIcon = menu.TextIcon,
                    MenuGroup = menu.MenuGroup,
                    Href = menu.Href,
                    Component = menu.Component,
                    OrgId = menu.OrgId,
                    Platform = menu.Platform,
                    Udf1 = menu.Udf1,
                    Udf2 = menu.Udf2,
                    Udf3 = menu.Udf3,
                    Udf4 = menu.Udf4,
                    Udf5 = menu.Udf5,
                    Udf6 = menu.Udf6

                });
                List<MenuInfo> menuInfos = vmenuInfos.ToList();

                if (menuInfos != null && menuInfos.Count() > 0)
                    menuInfoList.AddRange(menuInfos);
                List<String> pids = menuInfos.Select(c => c.Id).ToList();
                if (pids != null && pids.Count() > 0)
                    GetMenuAndSubMenu(pids, menuInfoList);
            }
        }
        public void GetMenuAndSubMenu(String pid, List<MenuInfo> menuInfoList)
        {
            GetMenuAndSubMenu(new List<string> { pid }, menuInfoList);
        }

        public override int deleteById(string id)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {

                    //判断需要删除的菜单是否来之菜单组
                    string currusername = CurrentUser.GetUserInfo()?.UserName;
                    if (string.IsNullOrEmpty(currusername))
                        throw new BussinessException("当前用户未登陆");

                    if (!IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) && !RoleInfoService.IsSupperAdmin(currusername))
                    {
                        throw new BussinessException("你没有权限对该数据执行删除操作");
                    }
                    MenuGrpInfo menuGrpInfo = ctx.MenuGrpInfo.Where(f => f.Id == id).FirstOrDefault();
                    List<MenuInfo> menuInfos = new List<MenuInfo>();
                    GetMenuAndSubMenu(id, menuInfos);
                    int i = 0;
                    if (menuGrpInfo == null || menuInfos.Count() >= 0)
                    {
                        List<String> ids = menuInfos.Select(c => c.Id).ToList();
                        ids.Add(id);

                        ctx.Delete<MenuInfo>(f => ids.Contains(f.Id));
                        ctx.Delete<RoleFunction>(f => ids.Contains(f.FuncId));
                        ctx.Delete<AllowAuthorized>(f => ids.Contains(f.FuncId));
                    }
                    //同步删除菜单组
                    if (menuGrpInfo != null)
                    {
                        ctx.Delete<MenuGrpInfo>(f => f.Id == id);
                    }
                    ts.Complete();
                    return i;

                }

            }
        }

        public int UpdateMenuSort(List<MenuSortDto> list)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {
                    foreach (var item in list)
                    {
                        string sql = $"update MENU_INFO set Sort = {item.sort} where Id = '{item.id}'";
                        ctx.Sql(sql);
                    }
                    ts.Complete();
                }
            }
            return 1;
        }
        public List<String> GetMenuIds(List<String> id)
        {
            using (var ctx = DbContext())
            {
                var vmenuInfos = ctx.VOrgMenuInfo.Where(f => id.Contains(f.Pid)).ToList();
                List<MenuInfo> menuInfos = vmenuInfos.Select(f =>
                {
                    var menu = new MenuInfo();
                    ObjectExtensions.CopyProperties(f, menu);
                    return menu;
                }).ToList();
                List<String> ids = new List<String>();
                if (menuInfos != null && menuInfos.Count() > 0)
                {
                    ids = menuInfos.Select(c => c.Id).ToList();
                    ids.AddRange(GetMenuIds(ids));
                }
                return ids;
            }


        }
        public int ReloadMenuDir(ChangeMenuInfoDto changeMenuInfoDto)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {
                    {
                        List<string> subList = new List<string>();

                        var vmenuInfos = ctx.VOrgMenuInfo.Where(f => changeMenuInfoDto.id.Contains(f.Pid)).ToList();
                        List<MenuInfo> menuInfos = vmenuInfos.Select(f =>
                        {
                            var menu = new MenuInfo();
                            ObjectExtensions.CopyProperties(f, menu);
                            return menu;
                        }).ToList();

                        List<String> ids = new List<String>();
                        int i = 0;
                        if (menuInfos != null && menuInfos.Count() > 0)
                        {
                            ids = menuInfos.Select(c => c.Id).ToList();
                            ids.AddRange(GetMenuIds(ids));
                            ChangeMenuInfoDto changeMenuInfoDto1 = new ChangeMenuInfoDto();
                            changeMenuInfoDto1.menuGroup = changeMenuInfoDto.menuGroup;
                            changeMenuInfoDto1.id = ids;

                            foreach (var item in ids)
                            {
                                string sql = $"UPDATE MENU_INFO SET MenuGroup = '{changeMenuInfoDto.menuGroup}' where Id = '{item}'";
                                ctx.Sql(sql);

                            }
                        }
                        changeMenuInfoDto.id.ForEach(item =>
                        {
                            string mSql = $"UPDATE MENU_INFO SET Pid ='{changeMenuInfoDto.pid}', MenuGroup = '{changeMenuInfoDto.menuGroup}' where Id = '{item}'";
                            ctx.Sql(mSql);
                        });
                        ts.Complete();
                        return i;//

                    }
                }

            }


            //menuInfos = ctx.MenuInfo
            //        .FromSqlRaw<MenuInfo>($" SELECT DISTINCT Id,CreateDate,CreateUser," +
            //        $"LastModifyDate,LastModifyUser,Status,MenuRoute,\r\n " +
            //        $"  FuncName AS MenuName,MenuNameEn,FuncCode AS MenuCode,Pid,Sort,MenuType,TextIcon,MenuGroup,Href,Component,OrgId,\r\n                         " +
            //        $"Platform,Udf1,Udf2,Udf3,Udf4,Udf5,Udf6 FROM V_USER_ROLE_FUNCTION UserRoleFuncView\r\n                        " +
            //        $" inner join ALLOW_AUTHORIZED AllowAuthorized on AllowAuthorized.FuncId =  UserRoleFuncView.Id\r\n        " +
            //        $"where UserRoleFuncView.Status=1 and UserRoleFuncView.MenuType in(0,2) and UserRoleFuncView.UserName =  '{currusername}' " +
            //        $"and UserRoleFuncView.MenuGroup = '{menuGroup}'\r\n            " +
            //        $"AND ((UserRoleFuncView.Scope = '0' AND UserRoleFuncView.OrgId IN (SELECT uod.OrgId FROM V_USER_ORG_DEPARTMENT uod " +
            //        $"where uod.UserName= '{currusername}' )) " +
            //        $"OR UserRoleFuncView.Scope = '1')\r\n        order by UserRoleFuncView.Sort asc")
            //        .ToList();
        }
    }
}


