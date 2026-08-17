using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IDepartmentService : ISecBaseService<Department>
    {
        public int UpdateUserDept(string deptId, string userId);
        public int DeleteByUserId(string userId);
        public int BatchInsert(List<DepartmentRole> list);
    }
}
