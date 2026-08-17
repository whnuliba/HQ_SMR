using IDS.Base;
using IDS.Common;
using IDS.Extension;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.IService.DTO;
using IDS.Security.Module;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class SysParameterService : SecBaseService<SysParamter, AuthDbContext>, ISysParameterService
    {
        public IRoleInfoService RoleInfoService { get; set; }
        public IOrganizationService OrganizationService { get; set; }
        public IdsRedis RedisClient { get; set; }
        public SysParamter SelectByParamCode(String paramCode)
        {
            using (var ctx = DbContext()) {

                return ctx.SysParamter.Where(f => f.ParamCode == paramCode).FirstOrDefault();
            }
        }

        public int DeleteParams(String id)
        {
            using (var ctx = DbContext())
            {

                using (var ts = new TransactionScope()) {

                    ctx.Delete<SysParameterDts>(f => f.ParamId == id);
                    ctx.Delete<SysParamter>(f => f.Id == id);
                    ts.Complete();
                }
            }
            return 0;
        }

        public List<SysParameterAndDts> GetSysParameterAndDtsByParamCode(String paramCode)
        {
            using (var ctx = DbContext())
            {
                var query = from sys in ctx.SysParamter join sysdts in ctx.SysParameterDts on sys.Id equals sysdts.ParamId
                            where sys.ParamCode == paramCode && sys.Status == 0 && sysdts.Status == 0
                            select new SysParameterAndDts {
                               ParamName =sys.ParamName,
                                ParamCode=sys.ParamCode,
                                ParamKey =sysdts.ParamKey,
                                ParamValue =sysdts.ParamValue
                       };

                return query.ToList();
            }
        }

        public List<SysParamDto> QueryParamsByCode(String paramCode)
        {
            using (var ctx = DbContext())
            {
                var query = from sys in ctx.SysParamter
                            join sysdts in ctx.SysParameterDts on sys.Id equals sysdts.ParamId
                            where sys.ParamCode == paramCode 
                            select new SysParamDto
                            {
                                paramDsc = sysdts.ParamDsc,
                                paramCode = sys.ParamCode,
                                paramKey = sysdts.ParamKey,
                                paramValue = sysdts.ParamValue
                            };

                return query.ToList();
            }
        }

        public override int deleteById(string id)
        {
            using (var ctx = DbContext()) {
                int i = ctx.Count<SysParameterDts>(f => f.ParamId == id);
                if (i > 0) {
                    throw new BussinessException("当前系统参数内有详细参数未删除,请您删除之后重新进行此操作！");
                }
            }
            return base.deleteById(id);
        }
        public IdsResult<string> RefreshCache(String paramCode)
        {


            using (var ctx = DbContext())
            {
                var query = from sys in ctx.SysParamter
                            join sysdts in ctx.SysParameterDts on sys.Id equals sysdts.ParamId
                            where sys.ParamCode == paramCode && sys.Status == 0 && sysdts.Status == 0
                            select new SysParameterAndDts
                            {
                                ParamName = sys.ParamName,
                                ParamCode = sys.ParamCode,
                                ParamKey = sysdts.ParamKey,
                                ParamValue = sysdts.ParamValue
                            };


                List<SysParameterAndDts> sysParameterAndDts = query.ToList();
                if (sysParameterAndDts == null || sysParameterAndDts.Count() == 0)
                    return IdsResult<string>.ok(false, "没有参数明细");
                sysParameterAndDts.ForEach(c=>{
                    RedisClient.SetHashFieldCache(IdsConstant.SYS_PARAMS_PREFIX_V2 + c.ParamCode, c.ParamKey, c.ParamValue);
                });
                return IdsResult<string>.ok(true);
            }


        }

        public override int save(SysParamter record, string?[] properites = null)
        {


            using (var ctx = DbContext()) {

                String currusername = CurrentUser.GetUserInfo()?.UserName;
                if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
                VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);
                int i = ctx.Count<SysParamter>(f => f.ParamCode == record.ParamCode);
                if (string.IsNullOrEmpty(record.Id) &&  i > 0) {
                    throw new BussinessException("已经存在相同的参数编码");
                }
                if(userOrgDepartmentView!=null)
                    record.OrgId = userOrgDepartmentView.OrgId;
            }

            return base.save(record, properites);
        }

        public override Page<SysParamter> List(Page<SysParamter> page, Expression<Func<SysParamter, bool>> predicate)
        {


            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (page.requestData == null)
                page.requestData = new SysParamter();
            if (userOrgDepartmentView != null && !string.IsNullOrEmpty(userOrgDepartmentView.OrgId))
            {
                page.requestData.OrgId = userOrgDepartmentView.OrgId;
            }
            if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
            {
                page.requestData.OrgId = null;
            }
            Expression<Func<SysParamter, bool>> where = null;
            if (!string.IsNullOrEmpty(page.requestData.ParamCode))
            {
                where = f => f.ParamCode == page.requestData.ParamCode;
            }
            if (where != null && !string.IsNullOrEmpty(page.requestData.ParamName))
            {

                where = where.And(f => f.ParamName == page.requestData.ParamName);
            }
            if (where != null && !string.IsNullOrEmpty(page.requestData.OrgId))
            {
                where = where.And(f => f.OrgId == page.requestData.OrgId);
            }

            return base.List(page, predicate);
        }

    }
}
