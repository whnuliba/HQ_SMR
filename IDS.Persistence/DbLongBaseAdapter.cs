using IDS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Persistence
{
    public abstract class DbLongBaseAdapter<T> where T : LongBaseEntity
    {
        public abstract IDbLongBaseService<T> Service();
        public int save(T record)
        {
            return Service().save(record);
        }
        public int delete(T record)
        {
            return Service().delete(record);
        }
        public int deleteById(long id)
        {
            return Service().deleteById(id);
        }
        public int update(T record)
        {
            return Service().update(record);
        }
        public virtual Page<T> GetPage(string tableName, string where, string orderBy, int pageIndex, int pageSize)
        {
            return Service().GetPage(tableName, where, orderBy, pageIndex, pageSize);
        }
        public virtual Page<T> GetPages(Page<T> page, Expression<Func<T, bool>> predicate=null)
        {
            return Service().List(page, predicate);
        }
        public T QueryById(long id)
        {
            return Service().QueryById(id);
        }
    }
}
