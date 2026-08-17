using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq.Expressions;
using IDS.Security.Module;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using IDS.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySqlConnector;
using IDS.Base;
using IDS.Persistence;

namespace IDS.Security.Service;

public partial class AuthDbContext : IDSContext
{
    private readonly string _dbConnectionString;
    private readonly string _dbType;
    public AuthDbContext(DbContextOptions options, IConfiguration configuration = null)
        : base(options)
    {
         configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var dbKey = configuration.GetSection("dbinfo:key").Value;
        _dbConnectionString = configuration.GetConnectionString(dbKey);
        var db = configuration.GetSection("dbinfo:type").Value;
        _dbType = db == null ? "MySql" : db.ToString();
    }

    //public AuthDbContext(IConfiguration configuration = null)
    //{
    //    configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    //    var dbKey = configuration.GetSection("dbinfo:key").Value;
    //    _dbConnectionString = configuration.GetConnectionString(dbKey);
    //    var db = configuration.GetSection("dbinfo:type").Value;
    //    _dbType = db == null ? "MySql" : db.ToString();
    //}
    //public AuthDbContext PcsContext()
    //{
    //    return DbContextFactory.CreateDbContext();
    //}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        switch (_dbType.ToLower())
        {
            case "mysql":
                var serverVersion = ServerVersion.AutoDetect(_dbConnectionString);  //mysql版本: {8.2.0-mysql}
                optionsBuilder.UseMySql(_dbConnectionString, serverVersion);
                break;
            case "sqlserver":
                optionsBuilder.UseSqlServer(_dbConnectionString);
                break;
            case "mssql":
                optionsBuilder.UseSqlServer(_dbConnectionString);
                break;
            default:
                serverVersion = ServerVersion.AutoDetect(_dbConnectionString);  //mysql版本: {8.2.0-mysql}
                optionsBuilder.UseMySql(_dbConnectionString, serverVersion);
                break;

        }
    }

    public override void Dispose()
    {
        // this.Database?.CloseConnection();
        base.Dispose();
        GC.Collect();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
    //}
    public virtual DbSet<AllowAuthorized> AllowAuthorized { get; set; }

    public virtual DbSet<BizInfo> BizInfo { get; set; }

    public virtual DbSet<BizInfoItem> BizInfoItem { get; set; }

    public virtual DbSet<BizMenuInfo> BizMenuInfo { get; set; }

    public virtual DbSet<Department> Department { get; set; }

    public virtual DbSet<DepartmentRole> DepartmentRole { get; set; }

    public virtual DbSet<DepartmentUser> DepartmentUser { get; set; }

    public virtual DbSet<FactoryInfo> FactoryInfo { get; set; }

    public virtual DbSet<FunctionInfo> FunctionInfo { get; set; }

    public virtual DbSet<I18nExceptionDef> I18nExceptionDef { get; set; }

    public virtual DbSet<InterfaceTicket> InterfaceTicket { get; set; }

    public virtual DbSet<JobInfo> JobInfo { get; set; }

    public virtual DbSet<JobRole> JobRole { get; set; }

    public virtual DbSet<MenuGrpInfo> MenuGrpInfo { get; set; }

    public virtual DbSet<MenuInfo> MenuInfo { get; set; }

    public virtual DbSet<MetuxRole> MetuxRole { get; set; }

    public virtual DbSet<Organization> Organization { get; set; }

    public virtual DbSet<RoleFactory> RoleFactory { get; set; }

    public virtual DbSet<RoleFunction> RoleFunction { get; set; }

    public virtual DbSet<RoleGroup> RoleGroup { get; set; }

    public virtual DbSet<RoleGroupItem> RoleGroupItem { get; set; }

    public virtual DbSet<RoleInfo> RoleInfo { get; set; }

    public virtual DbSet<SequenceGenerator> SequenceGenerator { get; set; }

    public virtual DbSet<SubRole> SubRole { get; set; }

    public virtual DbSet<SysMenuInfo> SysMenuInfo { get; set; }

    public virtual DbSet<SysParameterDts> SysParameterDts { get; set; }

    public virtual DbSet<SysParamter> SysParamter { get; set; }

    public virtual DbSet<UserGroup> UserGroup { get; set; }

    public virtual DbSet<UserGroupRole> UserGroupRole { get; set; }

    public virtual DbSet<UserGroupUser> UserGroupUser { get; set; }

    public virtual DbSet<UserInfo> UserInfo { get; set; }

    public virtual DbSet<UserRole> UserRole { get; set; }

    public virtual DbSet<VBizInfoItem> VBizInfoItem { get; set; }

    public virtual DbSet<VDepartmentUser> VDepartmentUser { get; set; }

    public virtual DbSet<VFunctionInfo> VFunctionInfo { get; set; }

    public virtual DbSet<VMenuInfo> VMenuInfo { get; set; }

    public virtual DbSet<VOrgMenuInfo> VOrgMenuInfo { get; set; }

    public virtual DbSet<VOrganization> VOrganization { get; set; }

    public virtual DbSet<VOrganizationUser> VOrganizationUser { get; set; }

    public virtual DbSet<VRoleAndGroup> VRoleAndGroup { get; set; }

    public virtual DbSet<VRoleFunction> VRoleFunction { get; set; }

    public virtual DbSet<VUserOrgDepartment> VUserOrgDepartment { get; set; }

    public virtual DbSet<VUserRole> VUserRole { get; set; }

    public virtual DbSet<VUserRoleFunction> VUserRoleFunction { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        switch (_dbType.ToLower())
        {
            case "mysql":
                modelBuilder.UseMySqlModelBuilder();
                break;
            case "sqlserver":
                modelBuilder.UseMSSqlModelBuilder();
                break;
            case "mssql":
                modelBuilder.UseMSSqlModelBuilder();
                break;
            default:
                modelBuilder.UseMySqlModelBuilder();
                break;
        }
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
