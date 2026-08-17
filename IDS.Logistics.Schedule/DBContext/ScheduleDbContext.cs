using System;
using System.Collections.Generic;
using System.Data;
using IDS.Common;
using IDS.Persistence;
using IDS.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IDS.Fms.Service;

public partial class ScheduleDbContext : IDSContext
{
    private readonly string? _dbConnectionString;
    private readonly string? _dbType;
    public ScheduleDbContext(DbContextOptions options, IConfiguration configuration = null)
        : base(options)
    {
         configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        string ds = configuration.GetSection("Quartz:quartz.jobStore.dataSource").Value ?? "myDS";
        _dbConnectionString = configuration.GetSection($"Quartz:quartz.dataSource.{ds}.connectionString").Value;
        string provider = configuration.GetSection($"Quartz:quartz.dataSource.{ds}.provider").Value;
        //_dbConnectionString = configuration.GetConnectionString(key);  
        var db = provider ?? configuration.GetSection("dbinfo:type").Value;
        _dbType = db == null ? "sqlserver" : db.ToLower();
    }


    //public ScheduleDbContext(IConfiguration configuration = null)
    //{
    //    configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    //    string ds = configuration.GetSection("Quartz:quartz.jobStore.dataSource").Value??"myDS";
    //    _dbConnectionString = configuration.GetSection($"Quartz:quartz.dataSource.{ds}.connectionString").Value;
    //    string provider = configuration.GetSection($"Quartz:quartz.dataSource.{ds}.provider").Value;
    //    //_dbConnectionString = configuration.GetConnectionString(key);  
    //    var db = provider??configuration.GetSection("dbinfo:type").Value;
    //    _dbType = db == null? "sqlserver" : db.ToLower();
    //}
    //public AuthDbContext PcsContext()
    //{
    //    return DbContextFactory.CreateDbContext();
    //}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        switch (_dbType.ToLower()) {
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
            case "oracle":
                optionsBuilder.UseOracle(_dbConnectionString);
                break;
            case "postgresql":
                optionsBuilder.UseNpgsql(_dbConnectionString);
                break;
            default:
                serverVersion = ServerVersion.AutoDetect(_dbConnectionString);  //mysql版本: {8.2.0-mysql}
                optionsBuilder.UseMySql(_dbConnectionString, serverVersion);
                break;

        }

        // optionsBuilder.UseMySql(_dbConnectionString);
    }

    public override void Dispose()
    {
        // this.Database?.CloseConnection();
        base.Dispose();
        GC.Collect();
    }

    public virtual DbSet<CwQrtzScheduleJob> CwQrtzScheduleJob { get; set; }

    public virtual DbSet<VCwQrtzScheduleJob> VCwQrtzScheduleJob { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        switch (_dbType.ToLower())
        {
            case "mysql":
                modelBuilder.ScheduleUseMySqlBuilder();
                break;
            case "mssql":
                modelBuilder.ScheduleUseMSSqlBuilder();
                break;
            case "sqlserver":
                modelBuilder.ScheduleUseMSSqlBuilder();
                break;
            default:
                modelBuilder.ScheduleUseMSSqlBuilder();
                break;
        }
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
