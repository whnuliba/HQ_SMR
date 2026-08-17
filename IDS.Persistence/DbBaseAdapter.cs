using IDS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Persistence
{
    public abstract class DbBaseAdapter<T> where T : BaseEntity
    {
        public abstract IDbBaseService<T> Service();
        public int save(T record)
        {
            return Service().save(record);
        }
        public int delete(T record)
        {
            return Service().delete(record);
        }
        public int deleteById(string id)
        {
            return Service().deleteById(id);
        }

        public T QueryById(string id)
        {
            return Service().QueryById(id);
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
    }
}
