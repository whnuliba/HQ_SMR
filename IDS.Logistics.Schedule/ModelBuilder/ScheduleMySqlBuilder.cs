using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Schedule
{
    public static class ScheduleMySqlBuilder
    {
        public static void ScheduleUseMySqlBuilder(this ModelBuilder modelBuilder) {
            modelBuilder.Entity<CwQrtzScheduleJob>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CW_QRTZ___3214EC277AABCD57");

                entity.ToTable("CW_QRTZ_SCHEDULE_JOB");

                entity.Property(e => e.Id)
                    .HasMaxLength(19)
                    .IsFixedLength()
                    .HasColumnName("ID");
                entity.Property(e => e.BusinessCode)
                    .HasMaxLength(50)
                    .HasColumnName("BUSINESS_CODE");
                entity.Property(e => e.CreateTime).HasColumnName("CREATE_TIME");
                entity.Property(e => e.CreateUser)
                    .HasMaxLength(30)
                    .HasColumnName("CREATE_USER");
                entity.Property(e => e.Cron)
                    .HasMaxLength(30)
                    .HasColumnName("CRON");
                entity.Property(e => e.JobClass)
                    .HasMaxLength(255)
                    .HasColumnName("JOB_CLASS");
                entity.Property(e => e.LastModifyTime).HasColumnName("LAST_MODIFY_TIME");
                entity.Property(e => e.LastModifyUser)
                    .HasMaxLength(30)
                    .HasColumnName("LAST_MODIFY_USER");
                entity.Property(e => e.ScheduleCode)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_CODE");
                entity.Property(e => e.ScheduleGrpCode)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_GRP_CODE");
                entity.Property(e => e.ScheduleName)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_NAME");
                entity.Property(e => e.ScheduleType)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_TYPE");
                entity.Property(e => e.Status).HasColumnName("STATUS");
                entity.Property(e => e.Interval).HasColumnName("INTERVAL");
            });

            modelBuilder.Entity<VCwQrtzScheduleJob>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_CW_QRTZ_SCHEDULE_JOB");

                entity.Property(e => e.BusinessCode)
                    .HasMaxLength(50)
                    .HasColumnName("BUSINESS_CODE");
                entity.Property(e => e.CreateTime).HasColumnName("CREATE_TIME");
                entity.Property(e => e.CreateUser)
                    .HasMaxLength(30)
                    .HasColumnName("CREATE_USER");
                entity.Property(e => e.Cron)
                    .HasMaxLength(30)
                    .HasColumnName("CRON");
                entity.Property(e => e.Id)
                    .HasMaxLength(19)
                    .IsFixedLength()
                    .HasColumnName("ID");
                entity.Property(e => e.JobClass)
                    .HasMaxLength(255)
                    .HasColumnName("JOB_CLASS");
                entity.Property(e => e.LastModifyTime).HasColumnName("LAST_MODIFY_TIME");
                entity.Property(e => e.LastModifyUser)
                    .HasMaxLength(30)
                    .HasColumnName("LAST_MODIFY_USER");
                entity.Property(e => e.ScheduleCode)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_CODE");
                entity.Property(e => e.ScheduleGrpCode)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_GRP_CODE");
                entity.Property(e => e.ScheduleName)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_NAME");
                entity.Property(e => e.ScheduleType)
                    .HasMaxLength(30)
                    .HasColumnName("SCHEDULE_TYPE");
                entity.Property(e => e.Status).HasColumnName("STATUS");
                entity.Property(e => e.TriggerState).HasColumnName("TRIGGER_STATE").HasMaxLength(16);
                entity.Property(e => e.Interval).HasColumnName("INTERVAL");
                entity.Property(e => e.NextFireTime).HasColumnName("NEXT_FIRE_TIME");
                entity.Property(e => e.PreFireTime).HasColumnName("PREV_FIRE_TIME");
                entity.Property(e => e.StartTime).HasColumnName("START_TIME");
                entity.Property(e => e.EndTime).HasColumnName("END_TIME");


            });
        }
    }
}
