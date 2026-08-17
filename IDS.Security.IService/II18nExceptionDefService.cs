using IDS.Base;
using IDS.Common;
using IDS.Security.Module;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface II18nExceptionDefService
    {
        IdsResult<string> Exception(string code, string ln);
        IdsResult<Dictionary<string, object>> Exception(string code);
        IdsResult<string> Refresh(string code);
        IdsResult<string> Refresh();
        int save(I18nExceptionDef record, string?[] properites = null);
        int delete(I18nExceptionDef record);
        int deleteById(string id);
        int update(I18nExceptionDef record, string?[] properites = null);
        Page<I18nExceptionDef> GetPage(string tableName, string where, string orderBy, int pageIndex, int pageSize);
        Page<I18nExceptionDef> List(Page<I18nExceptionDef> page, Expression<Func<I18nExceptionDef, bool>> predicate);
    }
}
