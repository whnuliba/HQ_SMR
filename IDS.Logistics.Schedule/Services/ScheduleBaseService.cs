using IDS.Base;
using IDS.Fms.IService;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IDS.Logistics.Schedule.Services
{
  public abstract class ScheduleBaseService<T, IDbContext> : DbBaseService<T>, IScheduleBaseService<T> where T : BaseEntity where IDbContext : IDSContext
    {
        public virtual IDbContextFactory<IDbContext> DbContextFactory { get; set; }

        public override IDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public T DoQuery<T>(Func<IDbContext, T> func)
        {
            using (var ctx = DbContext())
            {
                return func.Invoke(ctx);
            }
        }

        public T DoTransaction<T>(Func<IDbContext, T> func)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {
                    var result = func.Invoke(ctx);
                    ts.Complete();
                    return result;
                }

            }
        }

        public void DoWork(Action<IDbContext> func)
        {
            using (var ctx = DbContext())
            {
                func.Invoke(ctx);
            }
        }
    }
}
