using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.IService
{
    [AutoInjection]
    public class RackTaskHisService : DbBaseService<RackTaskHis>, IRackTaskHisService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override IDSContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }
    }
}
