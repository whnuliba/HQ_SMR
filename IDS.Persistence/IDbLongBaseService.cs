using IDS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Persistence
{
    public interface IDbLongBaseService<T> where T : LongBaseEntity
    {
        int save(T record, string? [] properites = null);
        int delete(T record);
        int deleteById(long id);
        T QueryById(long id);
        int update(T record, string? [] properites = null);
        Page<T> GetPage(string tableName, string where, string orderBy, int pageIndex, int pageSize);
        Page<T> List(Page<T> page, Expression<Func<T, bool>> predicate);
        Task<Page<T>> ListAsync(Page<T> page, Expression<Func<T, bool>> predicate);
    }
}
