using IDS.Base.Utils;
using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using Microsoft.AspNetCore.Http;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class I18nExceptionDefService : II18nExceptionDefService
    {
        public virtual IDbContextFactory<AuthDbContext> DbContextFactory { get; set; }

        public  AuthDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }
        public IdsResult<string> Exception(string code, string ln)
        {
            //先查缓存，找不到到数据库找
            string msg = GlobalExceptionDictionary.GetExceptionDictionary(code, ln);
            if (msg != null)
                return IdsResult<string>.ok(msg);
            return IdsResult<string>.ok(false);
        }

        public IdsResult<Dictionary<string, Object>> Exception(string code)
        {
            //先查缓存，找不到到数据库找
            Dictionary<string, Object> msg = GlobalExceptionDictionary.GetExceptionDictionary(code);
            if (msg != null)
                return IdsResult<Dictionary<string, Object>>.ok(msg);

            using (var ctx = DbContext())
            {

                I18nExceptionDef i18nExceptionDef = ctx.I18nExceptionDef.Where(f => f.ExceptionCode == code).FirstOrDefault();
                if (i18nExceptionDef == null)
                    return IdsResult<Dictionary<string, Object>>.ok(false);


                try
                {
                    var json = JsonConvert.SerializeObject(i18nExceptionDef);
                    Dictionary<string, Object> map = JsonConvert.DeserializeObject<Dictionary<string, Object>>(json);
                    GlobalExceptionDictionary.SetExceptionDictionary(code, map);
                    return IdsResult<Dictionary<string, Object>>.ok(map);
                }
                catch (Exception e)
                {
                    return IdsResult<Dictionary<string, Object>>.ok(false);
                }
            }


        }

        public IdsResult<string> Refresh(string code)
        {
            using (var ctx = DbContext())
            {

                I18nExceptionDef i18nExceptionDef = ctx.I18nExceptionDef.Where(f => f.ExceptionCode == code).FirstOrDefault();
                if (i18nExceptionDef == null)
                    return IdsResult<string>.ok(false);
                try
                {
                    var json = JsonConvert.SerializeObject(i18nExceptionDef);
                    Dictionary<string, Object> map = JsonConvert.DeserializeObject<Dictionary<string, Object>>(json);
                    GlobalExceptionDictionary.SetExceptionDictionary(code, map);
                    return IdsResult<string>.ok();
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
        }

        public IdsResult<string> Refresh()
        {
            using (var ctx = DbContext())
            {
                List<I18nExceptionDef> i18nExceptionDefList = ctx.I18nExceptionDef.ToList();
                if (i18nExceptionDefList.Count() == 0)
                    return IdsResult<string>.ok(false);
                try
                {
                    var json = JsonConvert.SerializeObject(i18nExceptionDefList);
                    List<Dictionary<string, Object>> map = JsonConvert.DeserializeObject<List<Dictionary<string, Object>>>(json);
                    map.ForEach(item =>
                    {
                        if (item.ContainsKey("exceptionCode") && item["exceptionCode"] != null)
                            GlobalExceptionDictionary.SetExceptionDictionary(item["exceptionCode"].ToString(), item);
                    });
                }
                catch (Exception e)
                {
                    throw e;
                }
                return IdsResult<string>.ok(true);

            }

        }


        public virtual int delete(I18nExceptionDef record)
        {
            using (var ctx = DbContext())
            {
                return ctx.Delete<I18nExceptionDef>(f => f.Id == record.Id);
            }
        }

        public virtual int deleteById(string id)
        {
            using (var ctx = DbContext())
            {
                return ctx.Delete<I18nExceptionDef>(f => f.Id == id);
            }
        }

        public virtual int save(I18nExceptionDef record, string?[] properites = null)
        {
            if (!String.IsNullOrWhiteSpace(record.Id))
            {
                return update(record, properites);
            }
            using (var ctx = DbContext())
            {

                if (string.IsNullOrWhiteSpace(record.Id))
                    record.Id = BaseUtil.uuid();
                return ctx.Insert<I18nExceptionDef>(record);
            }
        }

        public virtual int update(I18nExceptionDef record, string?[] properites = null)
        {
            using (var ctx = DbContext())
            {
                return ctx.UpdateByPrimaryKeySelective<I18nExceptionDef>(record, properites);
            }
        }

        public virtual Page<I18nExceptionDef> GetPage(string tableName, string where, string orderBy, int pageIndex, int pageSize)
        {
            int totalRecord = 0;
            var entitys = new List<I18nExceptionDef>();
            using (var ctx = DbContext())
            {
                entitys = ctx.GetPagedList<I18nExceptionDef>(tableName, where, orderBy, pageIndex, pageSize, out totalRecord);
            }
            return new Page<I18nExceptionDef>
            {
                pageSize = pageSize,
                current = pageIndex,
                total = totalRecord,
                data = entitys,
            };
        }

        public virtual Page<I18nExceptionDef> List(Page<I18nExceptionDef> page, Expression<Func<I18nExceptionDef, bool>> predicate)
        {
            using (var ctx = DbContext())
            {
                var req = page.requestData;
                var data = ctx.Query<I18nExceptionDef>(predicate).Skip((page.current - 1) * page.pageSize).Take(page.pageSize).ToList();
                var count = ctx.Count<I18nExceptionDef>(predicate);
                Page<I18nExceptionDef> page1 = new Page<I18nExceptionDef>(count, data, page.pageSize, page.current);
                return page1;
            }
        }
    }
}
