using IDS.HQ.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.Module
{
    public static class MySqlModelBuilder
    {
        public static void UseMySqlModelBuilder(this ModelBuilder modelBuilder) {

            modelBuilder
           .UseCollation("utf8mb4_general_ci")
           .HasCharSet("utf8mb4");

            modelBuilder.Entity<RackInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("RackInfo");

            });

            modelBuilder.Entity<RackTask>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("RackTask");

            });

            modelBuilder.Entity<Rack>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("Rack");

            });

            modelBuilder.Entity<RackCancelTask>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("RackCancelTask");

            });

            modelBuilder.Entity<RackTaskHis>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("RackTaskHis");

            });
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("UserInfo");

            });

        }
    }
}
