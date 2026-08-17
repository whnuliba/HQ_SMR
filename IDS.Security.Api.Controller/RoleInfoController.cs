using IDS.Base;
using IDS.Base.Utils;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.IService;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    [Route("role")]
    [PropertiesAutowired]
    [ApiController]
    public class RoleInfoController : DbBaseController<RoleInfo>
    {
        public RoleInfoAdapter RoleInfoAdapter { get; set; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RoleInfo> Adapter()
        {
            return RoleInfoAdapter;
        }

        [Route(Route.ROUTE_ROOT_ROLE_FUNCVIEW)]
        [HttpPost]
        public async Task<ResponseEntity<List<VFunctionInfo>>> getFuncByRole()
        {
            return ResponseEntity<List<VFunctionInfo>>.Success(RoleInfoAdapter.QueryAllFuncView());
        }


        [Route(Route.ROUTE_ROOT_ROLE_BSAVEFUNC)]
        [HttpPost]
        public ResponseEntity<string> saveFuncByRole(RequestData<List<RoleFunction>> list)
        {
            if (list == null || list.data == null || list.data.Count() == 0)
                return ResponseEntity<string>.Error("参数不能为空!");
            list.data.ForEach(c=>{
                c.Id =BaseUtil.uuid();
                c.saveInit();
                c.MenuId = c.FuncId;
            });
            RoleInfoAdapter.BatchInsert(list.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route(Route.ROUTE_ROOT_ROLE_FUNCBYROLEID)]
        [HttpPost]
        public ResponseEntity<List<RoleFunction>> selectByRoleId(RequestData<string> data)
        {
            if (data == null || data.data == null)
                return ResponseEntity<List<RoleFunction>>.Error("参数不能为空");

            return ResponseEntity<List<RoleFunction>>.Success(RoleInfoAdapter.SelectByRoleId(data.data));
        }
        [Route(Route.ROUTE_ROOT_ROLE_ALL)]
        [HttpPost]
        public ResponseEntity<List<RoleInfo>> queryAllRoles()
        {
            return ResponseEntity<List<RoleInfo>>.Success(RoleInfoAdapter.QueryAllRoles());
        }
        [Route(Route.ROUTE_ROOT_ROLE_DEL_ROLE_FUNC)]
        [HttpPost]
        public ResponseEntity<int> delRoleAndFunc(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<int>.Error("参数不能为空");
            return ResponseEntity<int>.Success(RoleInfoAdapter.DelRole(data.data));
        }

        [Route(Route.ROUTE_ROOT_ROLE_ALLOW_AUTH)]
        [HttpPost]
        public ResponseEntity<List<String>> allowAuthQuery(RequestData<List<String>> data)
        {
            if (!RequestData<List<String>>.isRequest(data))
                return ResponseEntity<List<String>>.Error("参数不能为空");
            return ResponseEntity<List<String>>.Success(RoleInfoAdapter.QueryAllowAuthAll());
        }
        [Route(Route.ROUTE_ROOT_ROLE_ALLOW_AUTH_DEL)]
        [HttpPost]
        public ResponseEntity<int> allowAuthDel(RequestData<List<String>> data)
        {
            if (!RequestData<List<String>>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<int>.Success(RoleInfoAdapter.DeleteAllowAuthByFuncIds(data.data));
        }
        [Route(Route.ROUTE_ROOT_ROLE_ALLOW_AUTH_EDIT)]
        [HttpPost]
        public ResponseEntity<int> allowAuthEdit(RequestData<List<String>> data)
        {
            if (!RequestData<List<String>>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<int>.Success(RoleInfoAdapter.ReplaceAllowAuth(data.data));
        }

        [Route(Route.ROUTE_ROOT_ROLE_ALLOW_AUTH_QUERY_ALL)]
        [HttpPost]
        public ResponseEntity<List<VFunctionInfo>> allowAuthQueryAll()
        {
            return ResponseEntity<List<VFunctionInfo>>.Success(RoleInfoAdapter.QueryAllFuncViewAll());
        }

        [Route(Route.ROUTE_ROOT_ROLE_GRP_ROLE)]
        [HttpPost]
        public ResponseEntity<List<RoleInfo>> queryGrpRoles(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<RoleInfo>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<RoleInfo>>.Success(RoleInfoAdapter.QueryGrpRoles(data.data));
        }

        [Route(Route.ROUTE_ROOT_ROLE_GRP_ROLE_JOB)]
        [HttpPost]
        public ResponseEntity<List<RoleInfo>> queryGrpRolesByJob(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<RoleInfo>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<RoleInfo>>.Success(RoleInfoAdapter.QueryGrpRolesByJob(data.data));
        }

        [Route(Route.ROUTE_ROOT_ROLE_GRP_ROLE_DEPT)]
        [HttpPost]
        public ResponseEntity<List<RoleInfo>> queryGrpRolesByDept(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<RoleInfo>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<RoleInfo>>.Success(RoleInfoAdapter.QueryGrpRolesByDept(data.data));
        }


        [Route(Route.ROUTE_ROOT_ROLE_GRP_ROLE_USER_GRP)]
        [HttpPost]
        public ResponseEntity<List<RoleInfo>> queryGrpRolesByUserGroup(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<RoleInfo>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<RoleInfo>>.Success(RoleInfoAdapter.QueryGrpRolesByUserGroup(data.data));
        }

    }
}
