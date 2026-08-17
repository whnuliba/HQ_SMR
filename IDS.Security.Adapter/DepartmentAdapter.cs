using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class DepartmentAdapter : SecBaseAdapter<Department>
    {
        public IDepartmentService DepartmentService { get; set; }
        public override IDbBaseService<Department> Service()
        {
            return DepartmentService;
        }
        public int UpdateUserDept(string deptId, string userId) {
            return DepartmentService.UpdateUserDept(deptId, userId);
        }

        public int DeleteByUserId(string userId) {
            return DepartmentService.DeleteByUserId(userId);
        }
        public int BatchInsert(List<DepartmentRole> list) {
            return DepartmentService.BatchInsert(list);
        }
    }
}
