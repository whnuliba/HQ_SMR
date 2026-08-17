using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Module
{
    public static class MySqlModelBuilder
    {
        public static void UseMySqlModelBuilder(this ModelBuilder modelBuilder) {

            modelBuilder
           .UseCollation("utf8_general_ci")
           .HasCharSet("utf8");

            modelBuilder.Entity<AllowAuthorized>(entity =>
            {
                entity.HasKey(e => e.FuncId).HasName("PRIMARY");

                entity.ToTable("ALLOW_AUTHORIZED");

                entity.Property(e => e.FuncId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.AuthSate)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<BizInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("BIZ_INFO");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.BizCode).HasMaxLength(100);
                entity.Property(e => e.BizComment).HasMaxLength(500);
                entity.Property(e => e.BizName).HasMaxLength(100);
                entity.Property(e => e.BizType).HasMaxLength(10);
                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.LastModifyTime).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleCode).HasMaxLength(100);
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Scope).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<BizInfoItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("BIZ_INFO_ITEM");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.BizId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.FieldCode).HasMaxLength(100);
                entity.Property(e => e.FieldName).HasMaxLength(100);
                entity.Property(e => e.LastModifyTime).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<BizMenuInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("BIZ_MENU_INFO", tb => tb.HasComment("菜单表"));

                entity.HasIndex(e => e.MenuCode, "Idx_menu_code").IsUnique();

                entity.HasIndex(e => e.MenuGroup, "MenuGroup");

                entity.HasIndex(e => e.OrgId, "OrgId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Component).HasMaxLength(100);
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuCode).HasMaxLength(30);
                entity.Property(e => e.MenuGroup).HasMaxLength(30);
                entity.Property(e => e.MenuName).HasMaxLength(30);
                entity.Property(e => e.MenuNameEn).HasMaxLength(50);
                entity.Property(e => e.MenuRoute).HasMaxLength(255);
                entity.Property(e => e.MenuType).HasColumnType("int(11)");
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("'#'")
                    .IsFixedLength();
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Scope)
                    .HasMaxLength(1)
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.TextIcon).HasMaxLength(30);
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("DEPARTMENT");

                entity.HasIndex(e => e.DeptCode, "Idx_Dept_code").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.DeptCode).HasMaxLength(30);
                entity.Property(e => e.DeptGrade).HasColumnType("int(11)");
                entity.Property(e => e.DeptName).HasMaxLength(30);
                entity.Property(e => e.DeptType).HasColumnType("int(11)");
                entity.Property(e => e.JobDsc).HasMaxLength(255);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.Leader).HasMaxLength(32);
                entity.Property(e => e.LeaderId).HasMaxLength(32);
                entity.Property(e => e.LeaderName).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("'#'")
                    .IsFixedLength();
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<DepartmentRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("DEPARTMENT_ROLE");

                entity.HasIndex(e => e.DeptId, "DeptId");

                entity.HasIndex(e => e.RoleId, "RoleId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.DeptId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleType).HasColumnType("int(11)");

                entity.HasOne(d => d.Dept).WithMany(p => p.DepartmentRole)
                    .HasForeignKey(d => d.DeptId)
                    .HasConstraintName("DEPARTMENT_ROLE_ibfk_1");

                entity.HasOne(d => d.Role).WithMany(p => p.DepartmentRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("DEPARTMENT_ROLE_ibfk_2");
            });

            modelBuilder.Entity<DepartmentUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("DEPARTMENT_USER");

                entity.HasIndex(e => e.DeptId, "DeptId");

                entity.HasIndex(e => e.UserId, "UserId").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.DeptId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MainJob).HasColumnType("int(11)");
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.UserId)
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.HasOne(d => d.Dept).WithMany(p => p.DepartmentUser)
                    .HasForeignKey(d => d.DeptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DEPARTMENT_USER_ibfk_1");

                entity.HasOne(d => d.User).WithOne(p => p.DepartmentUser)
                    .HasForeignKey<DepartmentUser>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DEPARTMENT_USER_ibfk_2");
            });

            modelBuilder.Entity<FactoryInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("FACTORY_INFO");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.FactoryDesc).HasMaxLength(100);
                entity.Property(e => e.FactoryName).HasMaxLength(50);
                entity.Property(e => e.FactoryNo).HasMaxLength(30);
                entity.Property(e => e.LastModifyTime).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.Remark).HasMaxLength(100);
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
            });

            modelBuilder.Entity<FunctionInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("FUNCTION_INFO", tb => tb.HasComment("功能表"));

                entity.HasIndex(e => e.FuncCode, "Idx_Func_code").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Component)
                    .HasMaxLength(100)
                    .HasColumnName("component");
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.FuncCode).HasMaxLength(30);
                entity.Property(e => e.FuncName).HasMaxLength(50);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuGroup).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Platform).HasMaxLength(30);
                entity.Property(e => e.Scope)
                    .HasMaxLength(1)
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);
                entity.Property(e => e.UseState).HasColumnType("int(11)");
            });

            modelBuilder.Entity<I18nExceptionDef>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("I18N_EXCEPTION_DEF");

                entity.HasIndex(e => e.ExceptionCode, "ExceptionCode");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.AppCode).HasMaxLength(30);
                entity.Property(e => e.Ch).HasMaxLength(500);
                entity.Property(e => e.CreateTime).HasColumnType("timestamp");
                entity.Property(e => e.En).HasMaxLength(500);
                entity.Property(e => e.ExceptionCode).HasMaxLength(30);
                entity.Property(e => e.Fre).HasMaxLength(500);
                entity.Property(e => e.Ger).HasMaxLength(500);
                entity.Property(e => e.Japan).HasMaxLength(500);
                entity.Property(e => e.Korea).HasMaxLength(500);
                entity.Property(e => e.Ln1).HasMaxLength(500);
                entity.Property(e => e.Ln2).HasMaxLength(500);
                entity.Property(e => e.Ln3).HasMaxLength(500);
                entity.Property(e => e.Poland).HasMaxLength(500);
                entity.Property(e => e.Vn).HasMaxLength(500);
                entity.Property(e => e.Zh).HasMaxLength(500);
            });

            modelBuilder.Entity<InterfaceTicket>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("INTERFACE_TICKET");

                entity.HasIndex(e => e.BizCode, "InterfaceTicket_BizCode").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.BizCode).HasMaxLength(30);
                entity.Property(e => e.Code).HasMaxLength(30);
                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.InterfaceName).HasMaxLength(30);
                entity.Property(e => e.Ip)
                    .HasMaxLength(15)
                    .IsFixedLength();
                entity.Property(e => e.LastModifyTime).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.Ticket).HasMaxLength(256);
                entity.Property(e => e.Type).HasColumnType("int(11)");
            });

            modelBuilder.Entity<JobInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("JOB_INFO");

                entity.HasIndex(e => e.JobNo, "JobNo").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.JobDesc).HasMaxLength(100);
                entity.Property(e => e.JobName).HasMaxLength(30);
                entity.Property(e => e.JobNo).HasMaxLength(30);
                entity.Property(e => e.JobType).HasMaxLength(30);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Scope)
                    .HasMaxLength(1)
                    .HasDefaultValueSql("'1'");
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<JobRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("JOB_ROLE");

                entity.HasIndex(e => new { e.JobId, e.RoleId }, "JobId").IsUnique();

                entity.HasIndex(e => e.JobId, "JobId_2");

                entity.HasIndex(e => e.RoleId, "RoleId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.JobId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleType).HasColumnType("int(11)");

                entity.HasOne(d => d.Job).WithMany(p => p.JobRole)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("JOB_ROLE_ibfk_1");

                entity.HasOne(d => d.Role).WithMany(p => p.JobRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("JOB_ROLE_ibfk_2");
            });

            modelBuilder.Entity<MenuGrpInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("MENU_GRP_INFO");

                entity.HasIndex(e => e.GroupCode, "GroupCode").IsUnique();

                entity.HasIndex(e => e.OrgId, "OrgId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Desc).HasMaxLength(100);
                entity.Property(e => e.GroupCode).HasMaxLength(50);
                entity.Property(e => e.GroupName).HasMaxLength(50);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Platform).HasMaxLength(30);
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);

                entity.HasOne(d => d.Org).WithMany(p => p.MenuGrpInfo)
                    .HasForeignKey(d => d.OrgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MENU_GRP_INFO_ibfk_1");
            });

            modelBuilder.Entity<MenuInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("MENU_INFO", tb => tb.HasComment("菜单表"));

                entity.HasIndex(e => e.MenuCode, "Idx_menu_code").IsUnique();

                entity.HasIndex(e => e.MenuGroup, "MenuGroup");

                entity.HasIndex(e => e.OrgId, "OrgId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Component).HasMaxLength(100);
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuCode).HasMaxLength(30);
                entity.Property(e => e.MenuGroup).HasMaxLength(30);
                entity.Property(e => e.MenuName).HasMaxLength(30);
                entity.Property(e => e.MenuNameEn).HasMaxLength(50);
                entity.Property(e => e.MenuRoute).HasMaxLength(255);
                entity.Property(e => e.MenuType).HasColumnType("int(11)");
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("'#'")
                    .IsFixedLength();
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Scope)
                    .HasMaxLength(1)
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.TextIcon).HasMaxLength(30);
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);

                //entity.HasOne(d => d.MenuGroupNavigation).WithMany(p => p.MenuInfo)
                //    .HasPrincipalKey(p => p.GroupCode)
                //    .HasForeignKey(d => d.MenuGroup)
                //    .HasConstraintName("MENU_INFO_ibfk_2");

                //entity.HasOne(d => d.Org).WithMany(p => p.MenuInfo)
                //    .HasForeignKey(d => d.OrgId)
                //    .HasConstraintName("MENU_INFO_ibfk_1");
            });

            modelBuilder.Entity<MetuxRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("METUX_ROLE");

                entity.HasIndex(e => e.MutexRoleId, "MutexRoleId");

                entity.HasIndex(e => new { e.RoleId, e.MutexRoleId }, "RoleId").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.MutexRoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.HasOne(d => d.MutexRole).WithMany(p => p.MetuxRoleMutexRole)
                    .HasForeignKey(d => d.MutexRoleId)
                    .HasConstraintName("METUX_ROLE_ibfk_2");

                entity.HasOne(d => d.Role).WithMany(p => p.MetuxRoleRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("METUX_ROLE_ibfk_1");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ORGANIZATION");

                entity.HasIndex(e => e.OrgCode, "Idx_Org_code").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Grade).HasColumnType("int(11)");
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgCode).HasMaxLength(30);
                entity.Property(e => e.OrgName).HasMaxLength(30);
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'1'")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<RoleFactory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ROLE_FACTORY");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.FactoryId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
            });

            modelBuilder.Entity<RoleFunction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ROLE_FUNCTION");

                entity.HasIndex(e => e.FuncId, "FuncId");

                entity.HasIndex(e => e.RoleId, "RoleId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Add).HasColumnType("int(11)");
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Del).HasColumnType("int(11)");
                entity.Property(e => e.Edit).HasColumnType("int(11)");
                entity.Property(e => e.FuncId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Upd).HasColumnType("int(11)");

                entity.HasOne(d => d.Role).WithMany(p => p.RoleFunction)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("ROLE_FUNCTION_ibfk_1");
            });

            modelBuilder.Entity<RoleGroup>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ROLE_GROUP");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.GroupDesc).HasMaxLength(100);
                entity.Property(e => e.GroupName).HasMaxLength(30);
                entity.Property(e => e.GroupNo).HasMaxLength(30);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleMaxUser).HasColumnType("int(11)");
                entity.Property(e => e.RoleType)
                    .HasDefaultValueSql("'1'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Scope)
                    .HasMaxLength(1)
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.UseState).HasColumnType("int(11)");
            });

            modelBuilder.Entity<RoleGroupItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ROLE_GROUP_ITEM");

                entity.HasIndex(e => new { e.GroupId, e.RoleId }, "GroupId").IsUnique();

                entity.HasIndex(e => e.RoleId, "RoleId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.GroupId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.HasOne(d => d.Group).WithMany(p => p.RoleGroupItem)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ROLE_GROUP_ITEM_ibfk_1");

                entity.HasOne(d => d.Role).WithMany(p => p.RoleGroupItem)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ROLE_GROUP_ITEM_ibfk_2");
            });

            modelBuilder.Entity<RoleInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ROLE_INFO", tb => tb.HasComment("角色表"));

                entity.HasIndex(e => e.RoleCode, "Idx_Role_code").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleCode).HasMaxLength(30);
                entity.Property(e => e.RoleMaxUser)
                    .HasDefaultValueSql("'0'")
                    .HasComment("0表示无限制")
                    .HasColumnType("int(11)");
                entity.Property(e => e.RoleName).HasMaxLength(50);
                entity.Property(e => e.RoleType)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.UseState).HasColumnType("int(11)");
            });

            modelBuilder.Entity<SequenceGenerator>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("SEQUENCE_GENERATOR");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength()
                    .HasColumnName("ID");
                entity.Property(e => e.Classification)
                    .HasMaxLength(30)
                    .HasColumnName("CLASSIFICATION");
                entity.Property(e => e.CreateTime)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("timestamp")
                    .HasColumnName("CREATE_TIME");
                entity.Property(e => e.CreateUser)
                    .HasMaxLength(30)
                    .HasColumnName("CREATE_USER");
                entity.Property(e => e.Increase)
                    .HasColumnType("int(11)")
                    .HasColumnName("INCREASE");
                entity.Property(e => e.LastModifyTime)
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasColumnType("timestamp")
                    .HasColumnName("LAST_MODIFY_TIME");
                entity.Property(e => e.LastModifyUser)
                    .HasMaxLength(30)
                    .HasColumnName("LAST_MODIFY_USER");
                entity.Property(e => e.Prefix)
                    .HasMaxLength(30)
                    .HasColumnName("PREFIX");
                entity.Property(e => e.Status)
                    .HasColumnType("int(11)")
                    .HasColumnName("STATUS");
            });

            modelBuilder.Entity<SubRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("SUB_ROLE");

                entity.HasIndex(e => e.RoleId, "RoleId");

                entity.HasIndex(e => e.SubRoleId, "SubRoleId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.SubRoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.HasOne(d => d.Role).WithMany(p => p.SubRoleRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("SUB_ROLE_ibfk_1");

                entity.HasOne(d => d.SubRoleNavigation).WithMany(p => p.SubRoleSubRoleNavigation)
                    .HasForeignKey(d => d.SubRoleId)
                    .HasConstraintName("SUB_ROLE_ibfk_2");
            });

            modelBuilder.Entity<SysMenuInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("SYS_MENU_INFO", tb => tb.HasComment("菜单表"));

                entity.HasIndex(e => e.MenuCode, "Idx_menu_code").IsUnique();

                entity.HasIndex(e => e.MenuGroup, "MenuGroup");

                entity.HasIndex(e => e.OrgId, "OrgId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Component).HasMaxLength(100);
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuCode).HasMaxLength(30);
                entity.Property(e => e.MenuGroup).HasMaxLength(30);
                entity.Property(e => e.MenuName).HasMaxLength(30);
                entity.Property(e => e.MenuNameEn).HasMaxLength(50);
                entity.Property(e => e.MenuRoute).HasMaxLength(255);
                entity.Property(e => e.MenuType).HasColumnType("int(11)");
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("'#'")
                    .IsFixedLength();
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Scope)
                    .HasMaxLength(1)
                    .HasDefaultValueSql("'0'");
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.TextIcon).HasMaxLength(30);
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);
            });

            modelBuilder.Entity<SysParameterDts>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("SYS_PARAMETER_DTS");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.LastModifyTime).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.ParamDsc).HasMaxLength(100);
                entity.Property(e => e.ParamId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.ParamKey).HasMaxLength(50);
                entity.Property(e => e.ParamValue).HasMaxLength(100);
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<SysParamter>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("SYS_PARAMTER");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.LastModifyTime).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.ParamCode).HasMaxLength(50);
                entity.Property(e => e.ParamDsc).HasMaxLength(500);
                entity.Property(e => e.ParamName).HasMaxLength(50);
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("USER_GROUP");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.GroupDesc).HasMaxLength(100);
                entity.Property(e => e.GroupName).HasMaxLength(30);
                entity.Property(e => e.GroupNo).HasMaxLength(30);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<UserGroupRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("USER_GROUP_ROLE");

                entity.HasIndex(e => e.GroupId, "GroupId");

                entity.HasIndex(e => e.RoleId, "RoleId");

                entity.HasIndex(e => new { e.RoleId, e.GroupId }, "RoleId_2");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.GroupId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleType).HasColumnType("int(11)");

                entity.HasOne(d => d.Group).WithMany(p => p.UserGroupRole)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("USER_GROUP_ROLE_ibfk_1");
            });

            modelBuilder.Entity<UserGroupUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("USER_GROUP_USER");

                entity.HasIndex(e => new { e.GroupId, e.UserId }, "GroupId").IsUnique();

                entity.HasIndex(e => e.UserId, "UserId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.GroupId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.UserId)
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.HasOne(d => d.Group).WithMany(p => p.UserGroupUser)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("USER_GROUP_USER_ibfk_1");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("USER_INFO", tb => tb.HasComment("用户定义表"));

                entity.HasIndex(e => e.UserName, "Idx_Account_code").IsUnique();

                entity.HasIndex(e => e.OrgId, "OrgId");

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.JobId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.Leader).HasMaxLength(32);
                entity.Property(e => e.LeaderId).HasMaxLength(32);
                entity.Property(e => e.LeaderName).HasMaxLength(30);
                entity.Property(e => e.Mobile).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Password).HasMaxLength(200);
                entity.Property(e => e.RealName).HasMaxLength(30);
                entity.Property(e => e.Sex).HasColumnType("int(11)");
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.UseState).HasColumnType("int(11)");
                entity.Property(e => e.UserName).HasMaxLength(30);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("USER_ROLE", tb => tb.HasComment("用户角色表"));

                entity.HasIndex(e => e.RoleId, "RoleId");

                entity.HasIndex(e => e.UserId, "UserId");

                entity.HasIndex(e => new { e.UserId, e.RoleId }, "UserId_2").IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleType)
                    .HasDefaultValueSql("'0'")
                    .HasComment("0 角色 1 角色组")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.UserId)
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.HasOne(d => d.User).WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("USER_ROLE_ibfk_2");
            });

            modelBuilder.Entity<VBizInfoItem>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_BIZ_INFO_ITEM");

                entity.Property(e => e.BizCode).HasMaxLength(100);
                entity.Property(e => e.BizId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.BizName).HasMaxLength(100);
                entity.Property(e => e.BizStatus).HasColumnType("int(11)");
                entity.Property(e => e.BizType).HasMaxLength(10);
                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.FieldCode).HasMaxLength(100);
                entity.Property(e => e.FieldName).HasMaxLength(100);
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.LastModifyTime).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RealName).HasMaxLength(30);
                entity.Property(e => e.RoleCode).HasMaxLength(100);
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Scope).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.UserName).HasMaxLength(30);
            });

            modelBuilder.Entity<VDepartmentUser>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_DEPARTMENT_USER");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.DeptCode).HasMaxLength(30);
                entity.Property(e => e.DeptId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.DeptLeader).HasMaxLength(32);
                entity.Property(e => e.DeptLeaderId).HasMaxLength(32);
                entity.Property(e => e.DeptLeaderName).HasMaxLength(30);
                entity.Property(e => e.DeptName).HasMaxLength(30);
                entity.Property(e => e.DeptType)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.JobId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.Mobile).HasMaxLength(30);
                entity.Property(e => e.OrgCode).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.OrgName).HasMaxLength(30);
                entity.Property(e => e.Password).HasMaxLength(200);
                entity.Property(e => e.RealName).HasMaxLength(30);
                entity.Property(e => e.Sex).HasColumnType("int(11)");
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.UseState).HasColumnType("int(11)");
                entity.Property(e => e.UserLeader).HasMaxLength(32);
                entity.Property(e => e.UserLeaderId).HasMaxLength(32);
                entity.Property(e => e.UserLeaderName).HasMaxLength(30);
                entity.Property(e => e.UserName).HasMaxLength(30);
            });

            modelBuilder.Entity<VFunctionInfo>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_FUNCTION_INFO");

                entity.Property(e => e.Component).HasMaxLength(100);
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.FuncCode).HasMaxLength(50);
                entity.Property(e => e.FuncName).HasMaxLength(50);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuGroup).HasMaxLength(50);
                entity.Property(e => e.MenuNameEn).HasMaxLength(50);
                entity.Property(e => e.MenuRoute).HasMaxLength(255);
                entity.Property(e => e.MenuType).HasColumnType("int(11)");
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.TextIcon).HasMaxLength(30);
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);
            });

            modelBuilder.Entity<VMenuInfo>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_MENU_INFO");

                entity.Property(e => e.Component).HasMaxLength(100);
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuCode).HasMaxLength(30);
                entity.Property(e => e.MenuGroup).HasMaxLength(30);
                entity.Property(e => e.MenuName).HasMaxLength(30);
                entity.Property(e => e.MenuNameEn).HasMaxLength(50);
                entity.Property(e => e.MenuRoute).HasMaxLength(255);
                entity.Property(e => e.MenuType).HasColumnType("int(11)");
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.TextIcon).HasMaxLength(30);
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);
            });

            modelBuilder.Entity<VOrgMenuInfo>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_ORG_MENU_INFO");

                entity.Property(e => e.Component).HasMaxLength(100);
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuCode).HasMaxLength(50);
                entity.Property(e => e.MenuGroup).HasMaxLength(50);
                entity.Property(e => e.MenuName).HasMaxLength(50);
                entity.Property(e => e.MenuNameEn).HasMaxLength(50);
                entity.Property(e => e.MenuRoute).HasMaxLength(255);
                entity.Property(e => e.MenuType).HasColumnType("int(11)");
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.TextIcon).HasMaxLength(30);
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);
            });

            modelBuilder.Entity<VOrganization>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_ORGANIZATION");

                entity.Property(e => e.Code).HasMaxLength(30);
                entity.Property(e => e.CreateUser)
                    .HasMaxLength(30)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.DeptType).HasColumnType("int(11)");
                entity.Property(e => e.Grade).HasColumnType("int(11)");
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.JobDsc).HasMaxLength(255);
                entity.Property(e => e.Leader).HasMaxLength(32);
                entity.Property(e => e.LeaderId).HasMaxLength(32);
                entity.Property(e => e.LeaderName).HasMaxLength(30);
                entity.Property(e => e.Name).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<VOrganizationUser>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_ORGANIZATION_USER");

                entity.Property(e => e.Code).HasMaxLength(30);
                entity.Property(e => e.CreateUser)
                    .HasMaxLength(30)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.DeptType).HasColumnType("int(11)");
                entity.Property(e => e.Grade).HasColumnType("int(11)");
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.JobDsc).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
            });

            modelBuilder.Entity<VRoleAndGroup>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_ROLE_AND_GROUP");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleCode).HasMaxLength(30);
                entity.Property(e => e.RoleMaxUser).HasColumnType("int(11)");
                entity.Property(e => e.RoleName).HasMaxLength(50);
                entity.Property(e => e.RoleType).HasColumnType("int(11)");
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.UseState).HasColumnType("int(11)");
            });

            modelBuilder.Entity<VRoleFunction>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_ROLE_FUNCTION");

                entity.Property(e => e.Add).HasColumnType("int(11)");
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Del).HasColumnType("int(11)");
                entity.Property(e => e.Edit).HasColumnType("int(11)");
                entity.Property(e => e.FuncId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.RoleCode).HasMaxLength(30);
                entity.Property(e => e.RoleName).HasMaxLength(50);
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Upd).HasColumnType("int(11)");
            });

            modelBuilder.Entity<VUserOrgDepartment>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_USER_ORG_DEPARTMENT");

                entity.Property(e => e.DeptCode).HasMaxLength(30);
                entity.Property(e => e.DeptId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.DeptName).HasMaxLength(30);
                entity.Property(e => e.OrgCode).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.OrgName).HasMaxLength(30);
                entity.Property(e => e.RealName).HasMaxLength(30);
                entity.Property(e => e.UserId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.UserName).HasMaxLength(30);
                entity.Property(e => e.UserState)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)");
                entity.Property(e => e.UserUseState).HasColumnType("int(11)");
            });

            modelBuilder.Entity<VUserRole>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_USER_ROLE");

                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasColumnType("datetime");
                entity.Property(e => e.CreateUser)
                    .HasMaxLength(30)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.RealName).HasMaxLength(30);
                entity.Property(e => e.RoleCode).HasMaxLength(30);
                entity.Property(e => e.RoleMaxUser).HasColumnType("int(11)");
                entity.Property(e => e.RoleName).HasMaxLength(50);
                entity.Property(e => e.RoleType).HasColumnType("int(11)");
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.UseState).HasColumnType("int(11)");
                entity.Property(e => e.UserId)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.UserName).HasMaxLength(30);
                entity.Property(e => e.UserState).HasColumnType("int(11)");
                entity.Property(e => e.UserUseState).HasColumnType("int(11)");
            });

            modelBuilder.Entity<VUserRoleFunction>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("V_USER_ROLE_FUNCTION");

                entity.Property(e => e.Add).HasColumnType("int(11)");
                entity.Property(e => e.Component).HasMaxLength(100);
                entity.Property(e => e.CreateDate).HasColumnType("datetime");
                entity.Property(e => e.CreateUser).HasMaxLength(30);
                entity.Property(e => e.Del).HasColumnType("int(11)");
                entity.Property(e => e.Edit).HasColumnType("int(11)");
                entity.Property(e => e.FuncCode).HasMaxLength(50);
                entity.Property(e => e.FuncName).HasMaxLength(50);
                entity.Property(e => e.Href).HasMaxLength(255);
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.LastModifyDate).HasColumnType("datetime");
                entity.Property(e => e.LastModifyUser).HasMaxLength(30);
                entity.Property(e => e.MenuGroup).HasMaxLength(50);
                entity.Property(e => e.MenuNameEn).HasMaxLength(50);
                entity.Property(e => e.MenuRoute).HasMaxLength(255);
                entity.Property(e => e.MenuType).HasColumnType("int(11)");
                entity.Property(e => e.OrgId)
                    .HasMaxLength(32)
                    .IsFixedLength();
                entity.Property(e => e.Pid)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''");
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.RealName).HasMaxLength(30);
                entity.Property(e => e.RoleCode).HasMaxLength(30);
                entity.Property(e => e.RoleId)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.RoleName).HasMaxLength(50);
                entity.Property(e => e.RoleStatus).HasColumnType("int(11)");
                entity.Property(e => e.Scope).HasMaxLength(1);
                entity.Property(e => e.Sort).HasColumnType("int(11)");
                entity.Property(e => e.Status).HasColumnType("int(11)");
                entity.Property(e => e.TextIcon).HasMaxLength(30);
                entity.Property(e => e.Udf1).HasMaxLength(30);
                entity.Property(e => e.Udf2).HasMaxLength(30);
                entity.Property(e => e.Udf3).HasMaxLength(30);
                entity.Property(e => e.Udf4).HasMaxLength(30);
                entity.Property(e => e.Udf5).HasMaxLength(30);
                entity.Property(e => e.Udf6).HasMaxLength(30);
                entity.Property(e => e.Upd).HasColumnType("int(11)");
                entity.Property(e => e.UserId)
                    .HasMaxLength(32)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
                entity.Property(e => e.UserName).HasMaxLength(30);
                entity.Property(e => e.UserState).HasColumnType("int(11)");
                entity.Property(e => e.UserUseState).HasColumnType("int(11)");
            });

        }
    }
}
