using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    public class UrlConstant
    {


    }
    public class Route
    {
        //用户
        public   const String ROUTE_ADD = "add";
        public const String  ROUTE_DEL = "del";
        public const String  ROUTE_EDIT = "edit";
        public const String  ROUTE_LIST = "list";
        public const String  ROUTE_ROOT_USER = "user";
        public const String  ROUTE_ROOT_USER_ROLE = "batch_save_user_role";
        public const String  ROUTE_ROOT_USER_DEL_USER_ROLE = "del_user_role";
        public const String  ROUTE_ROOT_USER_GETALL = "get-all";
        public const String  ROUTE_ROOT_USER_GETALL_IDS = "get-all-ids";

        public const String  ROUTE_ROOT_USER_IS_LOGIN = "is-login";

        public const String  ROUTE_ROOT_USER_KICKED_OUT = "kicked-out";
        public const String  ROUTE_ROOT_USER_LOGIN_USER = "login-user";
        public const String  ROUTE_ROOT_USER_EDIT_PWD = "update-pwd";

        public const String  ROUTE_ROOT_USER_RESET_PWD = "reset-pwd";

        public const String  ROUTE_ROOT_USER_GET_USER_NAME = "get-user-name";
        public const String  ROUTE_ROOT_MENU = "menu";
        public const String  ROUTE_ROOT_MENU_SORT = "menu-sort";
        public const String  ROUTE_ROOT_MENU_DIR = "dir";
        public const String  ROUTE_ROOT_MENU_NAME_DIR = "dir-by-name";
        public const String  ROUTE_ROOT_MENU_DIRS = "all-dir";
        public const String  ROUTE_ROOT_MENU_DEL = "menu_del";
        public const String  ROUTE_ROOT_MENU_TREE = "tree";
        public const String ROUTE_ROOT_MENU_ALL_TREE = "all-tree";
        public const String  ROUTE_ROOT_GET_BUTTON = "get-button";
        public const String  ROUTE_ROOT_MENU_MENU_GROUP = "get-menu-group";
        public const String  ROUTE_ROOT_MENU_RELOAD_DIR = "reload-dir";
        public const String  ROUTE_ROOT_MENU_BY_PID = "pid";
        public const String  ROUTE_ROOT_ROLE = "role";
        public const String  ROUTE_ROOT_ROLE_ALL = "role_all";

        public const String  ROUTE_ROOT_ROLE_QUERY_ALL = "role_items";
        public const String  ROUTE_ROOT_ROLE_FUNCVIEW = "func";
        public const String  ROUTE_ROOT_ROLE_BSAVEFUNC = "batch_save_func";
        public const String  ROUTE_ROOT_ROLE_FUNCBYROLEID = "func_by_roleid";
        public const String  ROUTE_ROOT_ROLE_DEL_ROLE_FUNC = "del_role_del";

        public const String  ROUTE_ROOT_ROLE_ALLOW_AUTH_QUERY_ALL = "allow-auth-query-all";

        public const String  ROUTE_ROOT_ROLE_GRP_ROLE = "query-grp-role";


        public const String  ROUTE_ROOT_ROLE_GRP_ROLE_JOB = "guest/query-grp-role-job";

        public const String  ROUTE_ROOT_ROLE_GRP_ROLE_DEPT = "guest/query-grp-role-dept";

        public const String  ROUTE_ROOT_ROLE_GRP_ROLE_USER_GRP = "guest/query-grp-role-userGrp";

        public const String  ROUTE_ROOT_ROLE_ALLOW_AUTH = "allow-auth";
        public const String  ROUTE_ROOT_ROLE_ALLOW_AUTH_EDIT = "allow-auth_edit";
        public const String  ROUTE_ROOT_ROLE_ALLOW_AUTH_DEL = "allow-auth_del";
        public const String  ROUTE_ROOT_SYS = "sys";
        public const String  ROUTE_ROOT_SYS_P = "sys-p";
        public const String  ROUTE_ROOT_SYS_DELETE = "sys-delete";
        public const String  ROUTE_ROOT_SYS_GET_P_CODE = "sys-p-code";
        public const String  ROUTE_ROOT_SYS_GET_PK_CODE = "sys-pk-code";
        public const String  ROUTE_ROOT_SYS_GET_PARAMETER_AND_DTS = "sys-parameter-dts";

        public const String  ROUTE_ROOT_SYS_GET_PARAMETER_BY_CODE = "get-param-by-code";

        public const String  ROUTE_ROOT_SYS_REFRESH_CACHE_BY_CODE = "refresh_param_to_cache";

        //令牌
        public const String  ROUTE_ROOT_TICKET = "ticket";

    }
}
