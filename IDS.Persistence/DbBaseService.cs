using IDS.Base;
using IDS.Base.Utils;
using IDS.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Persistence
{
    public abstract class DbBaseService<T> : IDbBaseService<T> where T : BaseEntity
    {


        public abstract IDSContext DbContext(); 

        public virtual int delete(T record)
        {
            using (var ctx = DbContext())
            {
               return ctx.Delete<T>(f => f.Id == record.Id);
            }
        }

        public virtual int deleteById(string id)
        {
            using (var ctx = DbContext())
            {
                return ctx.Delete<T>(f => f.Id == id);
            }
        }
        public virtual int save(T record, string?[] properites = null)
        {
            if (!String.IsNullOrWhiteSpace(record.Id))
            {
                using (var ctx = DbContext())
                {
                    record.updateInit();
                    if (properites != null && properites.Length > 0)
                        return ctx.Update<T>(record, properites);
                    ctx.Set<T>().Update(record);
                    return ctx.SaveChanges();
                }
            }

            using (var ctx = DbContext())
            {

                record.saveInit();
                if (string.IsNullOrWhiteSpace(record.Id))
                    record.Id =BaseUtil.uuid();
                return ctx.Insert<T>(record);
            }
        }

        public virtual int update(T record,string ?[] properites=null)
        {
            using (var ctx = DbContext())
            {
                record.updateInit();
                return ctx.UpdateByPrimaryKeySelective<T>(record, properites);
            }
        }

        public virtual Page<T> GetPage(string tableName, string where, string orderBy, int pageIndex, int pageSize)
        {
            int totalRecord = 0;
            var entitys = new List<T>();
            using (var ctx = DbContext())
            {
                entitys = ctx.GetPagedList<T>(tableName, where, orderBy, pageIndex, pageSize, out totalRecord);
            }
            return new Page<T>
            {
                pageSize = pageSize,
                current = pageIndex,
                total = totalRecord,
                data = entitys,
            };
        }

        public virtual Page<T> List(Page<T> page, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderby = null, Expression<Func<T, object>> orderBydesc = null)
        {
            using (var ctx = DbContext())
            {
                var req = page.requestData;
                var data = ctx.Query<T>(predicate);
                if (orderby != null)
                {
                    data = data.OrderBy(orderby);
                }
                if (orderBydesc != null)
                {
                    data = data.OrderByDescending(orderBydesc);
                }
                data = data.Skip((page.current - 1) * page.pageSize).Take(page.pageSize);
                var list = data.ToList();
                var count = ctx.Count<T>(predicate);
                Page<T> page1 = new Page<T>(count, list, page.pageSize, page.current);
                return page1;
            }
        }

        public async Task<Page<T>> ListAsync(Page<T> page, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderby=null, Expression<Func<T, object>> orderBydesc = null)
        {
            using (var ctx = DbContext())
            {
                var req = page.requestData;
                var data = ctx.Query<T>(predicate);
                if (orderby != null) {
                    data = data.OrderBy(orderby);
                }
                if (orderBydesc != null)
                {
                    data = data.OrderByDescending(orderBydesc);
                }
                data = data.Skip((page.current - 1) * page.pageSize).Take(page.pageSize);
                var list =  data.ToList();
                var count = ctx.Count<T>(predicate);
                Page<T> page1 = new Page<T>(count, list, page.pageSize, page.current);
                return page1;
            }
        }

        public T QueryById(string id)
        {
            using (var ctx = DbContext())
            {
                return ctx.Query<T>(f => f.Id == id).FirstOrDefault();
            }
        }
    }
}
