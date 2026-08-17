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
    public abstract class DbLongBaseService<T> : IDbLongBaseService<T> where T : LongBaseEntity
    {


        public abstract IDSContext DbContext(); 

        public virtual int delete(T record)
        {
            using (var ctx = DbContext())
            {
               return ctx.Delete<T>(f => f.Id == record.Id);
            }
        }

        public virtual int deleteById(long id)
        {
            using (var ctx = DbContext())
            {
                return ctx.Delete<T>(f => f.Id == id);
            }
        }

        public virtual int save(T record, string?[] properites = null)
        {
            if (record.Id!=null && record.Id!=0)
            {
                return update(record, properites);
            }
            using (var ctx = DbContext())
            {
        
                record.saveInit();
                if(record.Id==null|| record.Id==0)
                    record.Id =IdUtils.Id;
                return  ctx.Insert<T>(record);
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

        public virtual Page<T> List(Page<T> page, Expression<Func<T, bool>> predicate)
        {
            using (var ctx = DbContext())
            {
                var req = page.requestData;
                var data =  ctx.Query<T>(predicate).Skip((page.current-1)*page.pageSize).Take(page.pageSize).ToList();
                var count = ctx.Count<T>(predicate);
                Page<T> page1 = new Page<T>(count, data, page.pageSize, page.current);
                return page1;
            }
        }

        public async Task<Page<T>> ListAsync(Page<T> page, Expression<Func<T, bool>> predicate)
        {
            using (var ctx = DbContext())
            {
                var req = page.requestData;
                var data = ctx.Query<T>(predicate).Skip((page.current - 1) * page.pageSize).Take(page.pageSize).ToList();
                var count = ctx.Count<T>(predicate);
                Page<T> page1 = new Page<T>(count, data, page.pageSize, page.current);
                return page1;
            }
        }
        public T QueryById(long id)
        {
            using (var ctx = DbContext())
            {
                return ctx.Query<T>(f => f.Id == id).FirstOrDefault();
            }
        }
    }
}
