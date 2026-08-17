using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using IDS.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class I18nExceptionDefAdapter
    {
        public II18nExceptionDefService I18nExceptionDefService { get; set; }
        public IdsResult<string> Exception(string code, string ln)
        {
            return I18nExceptionDefService.Exception(code, ln);
        }
        public IdsResult<Dictionary<string, object>> Exception(string code)
        {
            return I18nExceptionDefService.Exception(code);
        }
        public IdsResult<string> Refresh(string code)
        {
            return I18nExceptionDefService.Refresh(code);
        }
        public IdsResult<string> Refresh()
        {
            return I18nExceptionDefService.Refresh();
        }
        public int save(I18nExceptionDef record, string?[] properites = null)
        {
            return I18nExceptionDefService.save(record, properites);
        }
        public int delete(I18nExceptionDef record)
        {
            return I18nExceptionDefService.delete(record);
        }
        public int deleteById(string id)
        {
            return I18nExceptionDefService.deleteById(id);
        }
        public int update(I18nExceptionDef record, string?[] properites = null)
        {
            return I18nExceptionDefService.update(record, properites);
        }
        public Page<I18nExceptionDef> List(Page<I18nExceptionDef> page, Expression<Func<I18nExceptionDef, bool>> predicate)
        {
            return I18nExceptionDefService.List(page, predicate);
        }
    }
}
