using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using IDS.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using IDS.Base;
using IDS.Persistence;
using LinqToDB.Common;
using IDS.HQ.Module;

namespace IDS.HQ.Module;

public partial class RackDbContext : IDSContext
{
    private readonly string _dbConnectionString;
    private readonly string _dbType;
    public RackDbContext(DbContextOptions options, IConfiguration configuration = null)
        : base(options)
    {
         configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var dbKey = configuration.GetSection("dbinfo:key").Value;
        _dbConnectionString = configuration.GetConnectionString(dbKey);
        var db = configuration.GetSection("dbinfo:type").Value;
        _dbType = db == null ? "MySql" : db.ToString();
    }

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
    public virtual DbSet<RackTask> RackTask { get; set; }

    public virtual DbSet<RackInfo> RackInfo { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        switch (_dbType.ToLower())
        {
            case "mysql":
                modelBuilder.UseMySqlModelBuilder();
                break;
            default:
                modelBuilder.UseMySqlModelBuilder();
                break;
        }
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
