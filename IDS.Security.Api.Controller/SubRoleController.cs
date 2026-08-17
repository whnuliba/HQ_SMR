using IDS.Base.Utils;
using IDS.Base;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Security.IService;
using IDS.Ioc;
using IDS.Common;
using IDS.Security.Adapter;

namespace IDS.Security.Api.Controller
{
    [Route("subRole")]
    [PropertiesAutowired]
    [ApiController]
    public class SubRoleController:ControllerBase
    {
      public RoleInfoAdapter RoleInfoAdapter { set; get; }
        //获取已经保存的子角色
        //获取不包含自己的所有角色
        //保存角色
        [Route("guest/already-role")]
        [HttpPost]
        public ResponseEntity<List<RoleInfo>> querySubRoleByRoleId(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<List<RoleInfo>>.Error("参数不能为空");
            return ResponseEntity<List<RoleInfo>>.Success(RoleInfoAdapter.QuerySubRoleByRoleId(data.data));
        }
        [Route("guest/select-all-role")]
        [HttpPost]
        public ResponseEntity<List<RoleInfo>> queryRolesNotContainsCurrId(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<List<RoleInfo>>.Error("参数不能为空");
            return ResponseEntity<List<RoleInfo>>.Success(RoleInfoAdapter.QueryRolesNotContainsCurrId(data.data));
        }
        [Route("batch-save-sub-role")]
        [HttpPost]
        public ResponseEntity<int> BatchInsertSubRole(RequestData<List<SubRole>> data)
        {
            if (!RequestData<List<SubRole>>.isRequest(data))
                return ResponseEntity<int>.Error("参数不能为空");
            data.data.ForEach(c=>{
                c.Id=BaseUtil.uuid();
            });
            return ResponseEntity<int>.Success(RoleInfoAdapter.BatchInsertSubRole(data.data));
        }
    }
}
