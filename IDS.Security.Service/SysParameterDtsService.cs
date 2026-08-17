using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class SysParameterDtsService : SecBaseService<SysParameterDts, AuthDbContext>, ISysParameterDtsService
    {
        public IdsRedis RedisClient { get; set; }

        public override int deleteById(string id)
        {
            using (var ctx= DbContext()) {

                SysParameterDts sysParameterDts = ctx.SysParameterDts.Where(f=>f.Id==id).FirstOrDefault();
                var sysParameter = ctx.SysParamter.Where(f => f.Id == sysParameterDts.ParamId).FirstOrDefault();
                int i = base.deleteById(id);
                RedisClient.RemoveHashFieldCache(IdsConstant.SYS_PARAMS_PREFIX_V2 + sysParameter.ParamCode, sysParameterDts.ParamKey);
                return i;
            }
        }

        public List<SysParameterDts> QueryParamsByParamCode(String paramCode)
        {
            using (var ctx = DbContext())
            {
                var query = from dts in ctx.SysParameterDts
                            where (from sys in ctx.SysParamter where sys.ParamCode == paramCode select sys.Id).Contains(dts.ParamId)
                            select dts;
                return query.ToList();
            }
        }

        public SysParameterDts QueryParamsByParamCodeAndKey(string paramCode, string paramKey)
        {
            using (var ctx = DbContext())
            {
                var query = from dts in ctx.SysParameterDts
                            join sys in ctx.SysParamter on dts.ParamId equals sys.Id
                            where sys.ParamCode == paramCode && dts.ParamKey == paramKey
                            select dts;
                return query.FirstOrDefault();
            }
        }


        public int DeleteByParamId(string paramId)
        {
            using (var ctx = DbContext()) {
                return ctx.Delete<SysParameterDts>(f => f.ParamId == paramId);
            }
        }
        public override int save(SysParameterDts sysParameterDts, string?[] properites = null)
        {
            using (var ctx = DbContext()) {

                SysParamter sysParameter =ctx.SysParamter.Where(f=>f.Id == sysParameterDts.ParamId).FirstOrDefault();


                int count = ctx.Count<SysParameterDts>(f => f.ParamKey == sysParameterDts.ParamKey && f.ParamId == sysParameterDts.ParamId);

                if (string.IsNullOrEmpty(sysParameterDts.Id) &&  count > 0 && string.IsNullOrEmpty(sysParameterDts.Id))
                {
                    throw new BussinessException("当前Key已存在,请您重新输入！");
                }
                int i = base.save(sysParameterDts, properites);
                String val = sysParameterDts.ParamValue;
                if (i > 0)
                {
                    RedisClient.SetHashFieldCache(IdsConstant.SYS_PARAMS_PREFIX_V2 + sysParameter.ParamCode, sysParameterDts.ParamKey, val);
                }
                if (sysParameterDts.Status != null && sysParameterDts.Status == 1)
                    RedisClient.RemoveHashFieldCache(IdsConstant.SYS_PARAMS_PREFIX_V2 + sysParameter.ParamCode, sysParameterDts.ParamKey);

                return i;
            }
        }

        public override Page<SysParameterDts> List(Page<SysParameterDts> page, Expression<Func<SysParameterDts, bool>> predicate)
        {
            if (page.requestData == null)
                throw new BussinessException("查询菜单明细不能为空");
            predicate = f=>f.ParamId == page.requestData.ParamId;
            return base.List(page, predicate);
        }
    }
}
