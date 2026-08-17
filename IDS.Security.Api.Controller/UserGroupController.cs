using IDS.Base.Utils;
using IDS.Base;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Common;

namespace IDS.Security.Api.Controller
{
    [Route("userGrp")]
    [PropertiesAutowired]
    [ApiController]
    public class UserGroupController : DbBaseController<UserGroup>
    {
        public virtual UserGroupAdapter UserGroupAdapter { set; get; }
        public virtual ILogger<UserGroupController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<UserGroup> Adapter()
        {
            return UserGroupAdapter;
        }



        [Route("guest/items")]
        [HttpPost]
        public ResponseEntity<Page<UserGroup>> queryItems(Page<UserGroup> data)
        {
            return ResponseEntity<Page<UserGroup>>.Success(UserGroupAdapter.GetPages(data));
        }
        [Route("guest/userGrp-user")]
        [HttpPost]
        public ResponseEntity<List<UserInfo>> queryUserByUserGrpId(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<List<UserInfo>>.Error("参数不能为空");
            return ResponseEntity<List<UserInfo>>.Success(UserGroupAdapter.QueryUserByUserGrpId(data.data));
        }
        [Route("batch-job-role")]
        [HttpPost]
        public ResponseEntity<int> saveFuncByRole(RequestData<List<UserGroupRole>> list)
        {
            if (list == null || list.data == null || list.data.Count == 0)
                return ResponseEntity<int>.Error("参数不能为空");
            list.data.ForEach(c=>{
                c.Id=BaseUtil.uuid();
            });
            return ResponseEntity<int>.Success(UserGroupAdapter.BatchInsert(list.data));
        }


        [Route("batch-userGrp-user")]
        [HttpPost]
        public ResponseEntity<int> saveUserGrpUser(RequestData<List<UserGroupUser>> list)
        {
            if (list == null || list.data == null || list.data.Count == 0)
                return ResponseEntity<int>.Error("参数不能为空");
            list.data.ForEach(c => {
                c.Id = BaseUtil.uuid();
            });
            return ResponseEntity<int>.Success(UserGroupAdapter.BatchInsertUserGrp(list.data));
        }

    }
}
