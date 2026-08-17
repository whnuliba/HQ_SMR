using IDS.Base.Utils;
using IDS.Base;
using IDS.Common;
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
using IDS.Common.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDS.Security.Api.Controller
{
    [Route("department")]
    [PropertiesAutowired]
    [ApiController]
    public class DepartmentController : DbBaseController<Department>
    {
        public virtual DepartmentAdapter DepartmentAdapter { set; get; }
        public virtual ILogger<DepartmentController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<Department> Adapter()
        {
            return DepartmentAdapter;
        }

        [Route("change-user-dept")]
        [HttpPost]
        public ResponseEntity<int> changeUserDept(RequestData<DepartmentUser> data)
        {
            if (!RequestData<DepartmentUser>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<int>.Success(DepartmentAdapter.UpdateUserDept(data.data.DeptId, data.data.UserId));
        }

        [Route("dept-user-dept")]
        [HttpPost]
        public ResponseEntity<string> DelUserDept(RequestData<DepartmentUser> data)
        {
            if (!RequestData<DepartmentUser>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            DepartmentAdapter.DeleteByUserId(data.data.UserId);
            return ResponseEntity<string>.Success("OK");
        }

        [Route("batch-dept-role")]
        [HttpPost]
        public ResponseEntity<int> saveFuncByRole(RequestData<List<DepartmentRole>> list)
        {
            if (!RequestData<List<DepartmentRole>>.isRequest(list))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            list.data.ForEach(c=>{
                c.Id = BaseUtil.uuid();
            });
            return ResponseEntity<int>.Success(DepartmentAdapter.BatchInsert(list.data));
        }
    }
}
