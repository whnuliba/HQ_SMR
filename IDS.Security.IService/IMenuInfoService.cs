using IDS.Security.Module;
using IDS.Security.IService.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Persistence;
using IDS.Security.IService.DTO;

namespace IDS.Security.IService
{
    public interface IMenuInfoService : ISecBaseService<MenuInfo>
    {
        List<MenuTree> GetAllMenuByUserName(String menuGroup);
        List<MenuTree> GetAllMenuTree();
        List<String> QueryAllMenuGroupByUsername(String username);
        List<MenuTree> GetAllMenuByUserNameAndMenuName(String menuGroup, String menuStr);
        List<MenuTree> GetAllGroupMenuByUserName(List<String> menuGroup);
        List<MenuInfo> QueryAllMenus();
        List<MenuInfo> GetButton(String menuGroup);
        List<MenuInfo> QueryAllMenusByPid(String Pid);
        void GetMenuAndSubMenu(List<String> pid, List<MenuInfo> menuInfoList);

        void GetMenuAndSubMenu(String pid, List<MenuInfo> menuInfoList);
        int UpdateMenuSort(List<MenuSortDto> list);
        int ReloadMenuDir(ChangeMenuInfoDto changeMenuInfoDto);
    }
}
