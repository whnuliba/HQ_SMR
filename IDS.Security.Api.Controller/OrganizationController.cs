using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.IService.POCO;
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

namespace IDS.Security.Api.Controller
{
    [Route("org")]
    [PropertiesAutowired]
    [ApiController]
    public class OrganizationController : DbBaseController<Organization>
    {
        public virtual OrganizationAdapter OrganizationAdapter { set; get; }
        public virtual ILogger<OrganizationController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<Organization> Adapter()
        {
            return OrganizationAdapter;
        }

        [Route("org-tree")]
        [HttpPost]
        public ResponseEntity<List<OrganizationTree>> getOrgTree()
        {
            return ResponseEntity<List<OrganizationTree>>.Success(OrganizationAdapter.GetOrgTree());
        }
        [Route("org-dept")]
        [HttpPost]
        public ResponseEntity<List<VOrganization>> getOrgDept(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<VOrganization>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<VOrganization>>.Success(OrganizationAdapter.SelectOrgViewBy(data.data));
        }

        [Route("org-tree-user")]
        [HttpPost]
        public ResponseEntity<List<OrganizationTree>> getAllUserTree(RequestData<String> data)
        {
            String username = null;
            if (RequestData<String>.isRequest(data))
                username = data.data;
            return ResponseEntity<List<OrganizationTree>>.Success(OrganizationAdapter.GetAllUserTree(username));
        }
    }
}
