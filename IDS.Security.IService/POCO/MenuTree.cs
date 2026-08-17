using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService.POCO
{
    public class MenuTree
    {
        public string name{set;get;}
        public string MenuGroup{set;get;}
        public string path{set;get;}
        public string TextIcon{set;get;}
        public int?sort{set;get;}
        public string id{set;get;}
        public string pid{set;get;}
        public string title{set;get;}
        public string icon{set;get;}
        public string href{set;get;}
        public string orgId{set;get;}
        public string component{set;get;}
        public bool spread{set;get;}
        public string scope{set;get;}
        public string enTitle{set;get;}
        public string platform{set;get;}
        public string udf1{set;get;}
        public string udf2{set;get;}
        public string udf3{set;get;}
        public string udf4{set;get;}
        public string udf5{set;get;}
        public string udf6{set;get;}

        public string componentName{set;get;}
        public List<MenuTree> children{set;get;}

        public static List<MenuTree> createMenu(List<MenuInfo> menu)
        {
            string pid = "#";
            MenuInfo  menuInfo = menu.Where(c=>c.MenuType == 2).FirstOrDefault();
            if (menuInfo != null) {           
                 pid = menuInfo.Id; 
            }
            return createMenu(menu, pid);
        }

        public static List<MenuTree> createAllMenu(List<MenuInfo> menu)
        {
            string pid = "#";
            List<MenuInfo> pids = menu.Where(c => c.MenuType == 2).ToList();
            List<MenuTree> menuTreeList = new List<MenuTree>();
            pids.ForEach(c=>{
                menuTreeList.AddRange(createMenu(menu, c.Id));
            });
            return menuTreeList;
        }


        public static List<MenuTree> createAllTreeMenu(List<MenuInfo> menu)
        {
            string pid = "0";
            //List<MenuInfo> pids = menu.Where(c => c.Pid == pid).ToList();
            List<MenuTree> menuTreeList = new List<MenuTree>();
            menuTreeList.AddRange(createMenu(menu, pid));
            //pids.ForEach(c => {
            //    menuTreeList.AddRange(createMenu(menu, c.Id));
            //});
            return menuTreeList;
        }

        private static List<MenuTree> createMenu(List<MenuInfo> menu, string pid)
        {
            List<MenuTree> pids = menu.Where(m=>m.Pid.Trim().Equals(pid)).Select(m=>{
                MenuTree mt = new MenuTree();
                mt.name = m.MenuName;
                mt.title= m.MenuName;
                mt.path = m.MenuRoute;
                mt.icon = m.TextIcon;// (m.getTextIcon());
                mt.TextIcon = m.TextIcon;// (m.getTextIcon());
                mt.href = m.Href;// (m.getHref());
                mt.id = m.Id;// (m.getId());
                mt.pid = m.Pid;// (m.getPid());
                mt.sort = m.Sort;// (m.getSort());
                mt.spread=false;
                mt.MenuGroup = m.MenuGroup;
                mt.orgId = m.OrgId;// (m.getOrgId());
                mt.enTitle = m.MenuNameEn;// (m.getMenuNameEn());
                mt.scope = m.Scope;// (m.getScope());
                mt.component = m.Component;// (m.getComponent());
                mt.platform = m.Platform;// (m.getPlatform());
                mt.udf1 = m.Udf1;// (m.getUdf1());
                mt.udf2 = m.Udf2;// (m.getUdf2());
                mt.udf3 = m.Udf3;// (m.getUdf3());
                mt.udf4 = m.Udf4;// (m.getUdf4());
                mt.udf5 = m.Udf5;// (m.getUdf5());
                mt.componentName = m.MenuCode;// (m.getMenuCode());
                mt.udf6 = m.Udf6;//(m.getUdf6());
                return mt;
            }).ToList();
            foreach (MenuTree t in pids)
            {
                t.children=createMenu(menu, t.id);
            }
            //过滤子目录是空的
            //filterMenuEmptyDir(pids);
            return pids;
        }


        //带字符串搜索的匹配

        public static bool filterMenus(List<MenuTree> menuTrees, string routeName)
        {
            List<MenuTree> waitRemove = new List<MenuTree> ();
            foreach (var tree in menuTrees) {
                if (string.IsNullOrEmpty(routeName))
                    continue;
                if (string.IsNullOrEmpty(tree.name))
                    waitRemove.Add(tree);
                  //menuTrees.Remove(tree);
                if (tree.name.Contains(routeName))
                {
                    continue;
                }
                if ((tree.children == null || tree.children.Count() == 0) && !tree.name.Contains(routeName))
                    waitRemove.Add(tree);
                //menuTrees.Remove(tree);
                if (tree.children.Count() > 0 && !filterMenus(tree.children, routeName))
                {
                    waitRemove.Add(tree);
                    //menuTrees.Remove(tree);
                }

            }
            foreach (var tree in waitRemove) {
                menuTrees.Remove(tree);
            }
            return menuTrees.Count() > 0;
        }

        public static List<MenuTree> createSearchMenu(List<MenuInfo> menu, string routeName)
        {
            string pid = "#";
            MenuInfo menuInfo = menu.Where(c => c.MenuType == 2).FirstOrDefault();
            if (menuInfo != null)
            {
                pid = menuInfo.Pid;
            }
            List<MenuTree> sMenus = createMenu(menu, pid);
            filterMenus(sMenus, routeName);
            return sMenus;
        }
        private static void filterMenuEmptyDir(List<MenuTree> menuTrees)
        {

            foreach (var m in menuTrees) {

                if (string.IsNullOrEmpty(m.href) && !m.href.Contains("/") && (m.children == null || m.children.Count() == 0))
                {
                    //edit by wanghao 2024-06-28移除对href的支持，改为窗口支持如_blank,_self等
                    // if(m.getChildren()==null || m.getChildren().size()==0){
                    menuTrees.Remove(m);
                }
                else if (m.children.Count() > 0)
                    filterMenuEmptyDir(m.children);
            }
        }
    }
}
