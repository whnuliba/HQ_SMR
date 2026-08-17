using IDS.Base;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.IService.DTO;
using IDS.Security.IService.POCO;
using IDS.Security.Module;
using IDS.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{

    [AutoInjection]
    public class MenuInfoAdapter : SecBaseAdapter<MenuInfo>
    {
        public IMenuInfoService MenuInfoService { get; set; }
        public override ISecBaseService<MenuInfo> Service()
        {
            return MenuInfoService;
        }
        public List<MenuTree> GetAllMenuByUserName(string menuGroup) { 
            return MenuInfoService.GetAllMenuByUserName(menuGroup);
        }

        public List<string> QueryAllMenuGroupByUsername(String username) {
            return MenuInfoService.QueryAllMenuGroupByUsername(username);
        }
        public List<MenuTree> GetAllMenuByUserNameAndMenuName(String menuGroup, String menuStr) {
            return MenuInfoService.GetAllMenuByUserNameAndMenuName(menuGroup, menuStr);
        }
        public List<MenuTree> GetAllGroupMenuByUserName(List<String> menuGroup) {
            return MenuInfoService.GetAllGroupMenuByUserName(menuGroup);
        }
        public List<MenuInfo> QueryAllMenus() { 
           return MenuInfoService.QueryAllMenus();
        }
        public List<MenuTree> GetAllMenuTree() {
            return MenuInfoService.GetAllMenuTree();
        }
        public List<MenuInfo> GetButton(String menuGroup) {
            return MenuInfoService.GetButton(menuGroup);
        }
        public List<MenuInfo> QueryAllMenusByPid(String Pid) {
            return MenuInfoService.QueryAllMenusByPid(Pid);
        }
        public int UpdateMenuSort(List<MenuSortDto> list) { 
           return MenuInfoService.UpdateMenuSort(list);
        }

        public int ReloadMenuDir(ChangeMenuInfoDto changeMenuInfoDto) {

            return MenuInfoService.ReloadMenuDir(changeMenuInfoDto);
        }
    }
}
