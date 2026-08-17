using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.IService.DTO;
using IDS.Security.IService.POCO;
using IDS.Security.Module;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    [Route("menu")]
    [PropertiesAutowired]
    [ApiController]
    public class MenuInfoController : DbBaseController<MenuInfo>
    {
        public virtual MenuInfoAdapter MenuInfoAdapter { set; get; }
        public virtual ILogger<MenuInfoController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<MenuInfo> Adapter()
        {
            return MenuInfoAdapter;
        }

        [Route(Route.ROUTE_ROOT_MENU_DIR)]
        [HttpPost]
        public ResponseEntity<List<MenuTree>> GetMenus(RequestData<string> data)
        {
            string menuGroup = "SYS";
            if (RequestData<string>.isRequest(data))
                menuGroup = data.data;
            List<MenuTree> lst = MenuInfoAdapter.GetAllMenuByUserName(menuGroup);//menuService.getMenus();
            if (null == lst || lst.Count() == 0)
            {
                menuGroup = "SYS"; //重新回到配置页
                lst = MenuInfoAdapter.GetAllMenuByUserName(menuGroup);//menuService.getMenus();
                if (lst == null || lst.Count() == 0)
                {
                    //若不存在则拉取用户可用组的其中之一
                    string username = CurrentUser.GetUserInfo()?.UserName;
                    if (username == null)
                        throw new BussinessException("用户未登录");
                    List<string> groupList = MenuInfoAdapter.QueryAllMenuGroupByUsername(username);
                    if (groupList.Count() == 0)
                        throw new BussinessException("用户没有权限登录系统");
                    for (int i = 0; i < groupList.Count(); i++)
                    {
                        menuGroup = groupList[i];
                        lst = MenuInfoAdapter.GetAllMenuByUserName(menuGroup);//menuService.getMenus();
                        if (lst.Count() > 0)
                            break;
                    }
                    if (lst == null || lst.Count() == 0)
                        return ResponseEntity<List<MenuTree>>.Error("菜单信息不存在");
                }
            }
            return ResponseEntity<List<MenuTree>>.Success(lst);
        }


        [Route(Route.ROUTE_ROOT_MENU_NAME_DIR)]
        [HttpPost]
        public ResponseEntity<List<MenuTree>> getMenusByRouteName(RequestData<MenuDto> data)
        {
            string menuGroup = "SYS";
            if (!RequestData<MenuDto>.isRequest(data))
                throw new BussinessException("当前页面可能已经失效，传入的参数已经为空了");
            Assert.notEmpty(data.data.appCode, "应用编码不能为空");
            menuGroup = data.data.appCode;
            List<MenuTree> lst = MenuInfoAdapter.GetAllMenuByUserNameAndMenuName(menuGroup, data.data.routeName);//menuService.getMenus();
            if (null == lst || lst.Count() == 0)
            {
                throw new BussinessException("没有找到您需要的菜单");
            }
            return ResponseEntity<List<MenuTree>>.Success(lst);
        }



        [Route(Route.ROUTE_ROOT_MENU_DIRS)]
        [HttpPost]
        public ResponseEntity<List<MenuTree>> getDirs(RequestData<List<string>> data)
        {
            List<MenuTree> lst = MenuInfoAdapter.GetAllGroupMenuByUserName(data.data);//menuService.getMenus();
            if (null == lst) return ResponseEntity<List<MenuTree>>.Error("菜单不存在");
            ResponseEntity<List<MenuTree>> st = ResponseEntity<List<MenuTree>>.Success(lst);
            return st;
        }

        [Route(Route.ROUTE_ROOT_MENU_TREE)]
        [HttpPost]
        public ResponseEntity<List<MenuInfo>> queryAllMenus()
        {
            List<MenuInfo> lst = MenuInfoAdapter.QueryAllMenus();
            if (null == lst) return ResponseEntity<List<MenuInfo>>.Error("菜单不存在");
            return ResponseEntity<List<MenuInfo>>.Success(lst);
        }

        [Route(Route.ROUTE_ROOT_MENU_ALL_TREE)]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<List<MenuTree>> GetAllMenuTree()
        {
            List<MenuTree> lst = MenuInfoAdapter.GetAllMenuTree();
            if (null == lst) return ResponseEntity<List<MenuTree>>.Error("菜单不存在");
            return ResponseEntity<List<MenuTree>>.Success(lst);
        }


        [Route(Route.ROUTE_ROOT_GET_BUTTON)]
        [HttpPost]
       //public async Task<ResponseEntity<List<MenuInfo>>> getButton(RequestData<string> data)
       public ResponseEntity<List<MenuInfo>> GetButton(RequestData<string> data)
        {
            string menuGroup = "SYS";
            if (RequestData<string>.isRequest(data))
                menuGroup = data.data;
            List<MenuInfo> lst = MenuInfoAdapter.GetButton(menuGroup);
            if (null == lst || lst.Count() == 0)
            {
                //若不存在则拉取用户可用组的其中之一
                string username = CurrentUser.GetUserInfo()?.UserName;
                if (username == null)
                    throw new BussinessException("用户未登录");
                List<string> groupList = MenuInfoAdapter.QueryAllMenuGroupByUsername(username);
                if (groupList.Count() == 0)
                    throw new BussinessException("用户没有权限登录系统");
                for (int i = 0; i < groupList.Count(); i++)
                {
                    menuGroup = groupList[i];
                    lst = MenuInfoAdapter.GetButton(menuGroup);//menuService.getMenus();
                    if (lst.Count() > 0)
                        break;
                }
                if (lst == null || lst.Count() == 0)
                    return ResponseEntity<List<MenuInfo>>.Error("菜单不存在");
            }
            return ResponseEntity<List<MenuInfo>>.Success(lst);
        }


        [Route(Route.ROUTE_ROOT_MENU_BY_PID)]
        [HttpPost]
        public ResponseEntity<List<MenuInfo>> queryAllMenusByPid(RequestData<string> data)
        {
            if (null == data.data) return ResponseEntity<List<MenuInfo>>.Error("参数不能为空");
            List<MenuInfo> lst = MenuInfoAdapter.QueryAllMenusByPid(data.data);
            if (null == lst) return ResponseEntity<List<MenuInfo>>.Error("菜单不存在");
            return ResponseEntity<List<MenuInfo>>.Success(lst);
        }


        [Route(Route.ROUTE_ROOT_MENU_DEL)]
        [HttpPost]
        public ResponseEntity<string> menuDel(RequestData<string> data)
        {
            MenuInfoAdapter.deleteById(data.data);
            return ResponseEntity<string>.Success("ok");
        }

        [Route(Route.ROUTE_ROOT_MENU_SORT)]
        [HttpPost]
        public ResponseEntity<string> updateMenuSort(RequestData<List<MenuSortDto>> data)
            {
                if (null == data.data) return ResponseEntity<string>.Error("参数不能为空");
                MenuInfoAdapter.UpdateMenuSort(data.data);
                return ResponseEntity<string>.Success("ok");
            }


        [Route(Route.ROUTE_ROOT_MENU_MENU_GROUP)]
        [HttpPost]
        public ResponseEntity<List<string>> queryAllMenuGroupByUsername()
        {
            string username = CurrentUser.GetUserInfo()?.UserName;
            if (username == null)
                throw new BussinessException("用户未登录");
            return ResponseEntity<List<string>>.Success(MenuInfoAdapter.QueryAllMenuGroupByUsername(username));
        }



        [Route(Route.ROUTE_ROOT_MENU_RELOAD_DIR)]
        [HttpPost]
        public ResponseEntity<string> reloadMenuDir(RequestData<ChangeMenuInfoDto> data)
        {
            if (!RequestData<ChangeMenuInfoDto>.isRequest(data))
                return ResponseEntity<string>.Error("参数不能为空");
            MenuInfoAdapter.ReloadMenuDir(data.data);
            return ResponseEntity<string>.Success("OK");
        }

    }




}
