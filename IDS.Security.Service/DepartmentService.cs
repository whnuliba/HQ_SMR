using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class DepartmentService : SecBaseService<Department, AuthDbContext>, IDepartmentService
    {
        public int UpdateUserDept(string deptId, string userId)
        {
            using (var ctx = DbContext()) {
                string sql = $"update DEPARTMENT_USER set DeptId='{deptId}' where UserId = '{userId}'";
                return ctx.Sql(sql);
            }
        }
        public int DeleteByUserId(string userId) {
            using (var ctx = DbContext()) {
                return ctx.Delete<DepartmentUser>(f => f.UserId == userId);
            }
        }

        public int BatchInsert(List<DepartmentRole> list) {

            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope()) {


                    int i = ctx.Count<DepartmentRole>(f => f.DeptId == list[0].DeptId);
                    if (i > 0)
                    {
                        i= ctx.Delete<DepartmentRole>(f => f.DeptId == list[0].DeptId);
                        if (i <= 0)
                            throw new BussinessException("删除部门角色失败");
                    }
                    if (list.Count() == 1 && "#".Equals(list[0].RoleId))
                        return i;
                    ctx.AddRange(list);
                    ts.Complete();
                    return i;
                }
            }
        }
    }
}
