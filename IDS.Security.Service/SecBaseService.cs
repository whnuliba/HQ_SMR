using IDS.Base;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Service
{
    public abstract class SecBaseService<T,IDbContext> : DbBaseService<T>, ISecBaseService<T> where T : BaseEntity where IDbContext : IDSContext
    {
        public virtual IDbContextFactory<IDbContext> DbContextFactory { get; set; }

        public override IDbContext DbContext() {
            return DbContextFactory.CreateDbContext();
        }

    }
}
