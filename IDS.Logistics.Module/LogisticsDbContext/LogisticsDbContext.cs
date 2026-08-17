using System;
using System.Collections.Generic;
using IDS.Logistics.Module.Entities;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IDS.Logistics.Module;

public partial class LogisticsDbContext : IDSContext
{
    private readonly string _dbConnectionString;
    private readonly string _dbType;
    public LogisticsDbContext(DbContextOptions options, IConfiguration configuration = null)
        : base(options)
    {
        configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var dbKey = configuration.GetSection("dbinfo:key").Value;
        _dbConnectionString = configuration.GetConnectionString(dbKey);
        var db = configuration.GetSection("dbinfo:type").Value;
        _dbType = db == null ? "MySql" : db.ToString();
    }
    public virtual DbSet<CwLogisticsBusinessTimer> CwLogisticsBusinessTimer { get; set; }

    public virtual DbSet<CwLogisticsCarrierInfo> CwLogisticsCarrierInfo { get; set; }

    public virtual DbSet<CwLogisticsLine> CwLogisticsLine { get; set; }

    public virtual DbSet<CwLogisticsLineGroup> CwLogisticsLineGroup { get; set; }

    public virtual DbSet<CwLogisticsLineGroupDts> CwLogisticsLineGroupDts { get; set; }

    public virtual DbSet<CwLogisticsLineType> CwLogisticsLineType { get; set; }

    public virtual DbSet<CwLogisticsLocationType> CwLogisticsLocationType { get; set; }

    public virtual DbSet<CwLogisticsLogicRack> CwLogisticsLogicRack { get; set; }

    public virtual DbSet<CwLogisticsLogicRackDts> CwLogisticsLogicRackDts { get; set; }

    public virtual DbSet<CwLogisticsMaterial> CwLogisticsMaterial { get; set; }

    public virtual DbSet<CwLogisticsMaterialInfo> CwLogisticsMaterialInfo { get; set; }

    public virtual DbSet<CwLogisticsMonitor> CwLogisticsMonitor { get; set; }

    public virtual DbSet<CwLogisticsPlcDefined> CwLogisticsPlcDefined { get; set; }

    public virtual DbSet<CwLogisticsProcessState> CwLogisticsProcessState { get; set; }

    public virtual DbSet<CwLogisticsRack> CwLogisticsRack { get; set; }

    public virtual DbSet<CwLogisticsRackState> CwLogisticsRackState { get; set; }

    public virtual DbSet<CwLogisticsRoad> CwLogisticsRoad { get; set; }

    public virtual DbSet<CwLogisticsRoadway> CwLogisticsRoadway { get; set; }

    public virtual DbSet<CwLogisticsScanState> CwLogisticsScanState { get; set; }

    public virtual DbSet<CwLogisticsSubtaskInfo> CwLogisticsSubtaskInfo { get; set; }

    public virtual DbSet<CwLogisticsSysParameter> CwLogisticsSysParameter { get; set; }

    public virtual DbSet<CwLogisticsSysParameterItem> CwLogisticsSysParameterItem { get; set; }

    public virtual DbSet<CwLogisticsTaskInfo> CwLogisticsTaskInfo { get; set; }

    public virtual DbSet<CwLogisticsTaskOption> CwLogisticsTaskOption { get; set; }

    public virtual DbSet<CwLogisticsTaskRoad> CwLogisticsTaskRoad { get; set; }

    public virtual DbSet<CwLogisticsTaskTimer> CwLogisticsTaskTimer { get; set; }

    public virtual DbSet<CwLogisticsTooling> CwLogisticsTooling { get; set; }

    public virtual DbSet<CwLogisticsTrayInfo> CwLogisticsTrayInfo { get; set; }

    public virtual DbSet<CwLogisticsWare> CwLogisticsWare { get; set; }

    public virtual DbSet<CwLogisticsWareType> CwLogisticsWareType { get; set; }




    public string DBConnectionString;
    public string DbType;


    //public FormationDbContext(DbContextOptions options, IConfiguration configuration = null) : base(options, configuration)
    //{
    //    // base();
    //    configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    //    var dbKey = configuration.GetSection("dbinfo:key").Value;
    //    DBConnectionString = configuration.GetConnectionString(dbKey);
    //    var db = configuration.GetSection("dbinfo:type").Value;
    //    DbType = db == null ? "MySql" : db.ToString();
    //}
    //public FormationDbContext(IConfiguration configuration = null)
    //{
    //    // base();
    //    configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    //    var dbKey = configuration.GetSection("dbinfo:key").Value;
    //    DBConnectionString = configuration.GetConnectionString(dbKey);
    //    var db = configuration.GetSection("dbinfo:type").Value;
    //    DbType = db == null ? "MySql" : db.ToString();
    //}
    //public AuthDbContext PcsContext()
    //{
    //    return DbContextFactory.CreateDbContext();
    //}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        switch (DbType.ToLower())
        {
            case "mysql":
                var serverVersion = ServerVersion.AutoDetect(DBConnectionString);  //mysql版本: {8.2.0-mysql}
                optionsBuilder.UseMySql(DBConnectionString, serverVersion);
                break;
            case "sqlserver":
                optionsBuilder.UseSqlServer(DBConnectionString);
                break;
            case "mssql":
                optionsBuilder.UseSqlServer(DBConnectionString);
                break;
            case "oracle":
                optionsBuilder.UseOracle(DBConnectionString);
                break;
            case "postgresql":
                optionsBuilder.UseNpgsql(DBConnectionString);
                break;
            default:
                serverVersion = ServerVersion.AutoDetect(DBConnectionString);  //mysql版本: {8.2.0-mysql}
                optionsBuilder.UseMySql(DBConnectionString, serverVersion);
                break;

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CwLogisticsBusinessTimer>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_BUSINESS_TIMER", tb => tb.HasComment("寻路任务表定时器"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Cron)
                .HasMaxLength(30)
                .HasColumnName("CRON");
            entity.Property(e => e.JobService)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("JOB_SERVICE");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.Mutex)
                .HasMaxLength(50)
                .HasColumnName("MUTEX");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Time).HasColumnName("TIME");
            entity.Property(e => e.UseState).HasColumnName("USE_STATE");
        });

        modelBuilder.Entity<CwLogisticsCarrierInfo>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_CARRIER_INFO", tb => tb.HasComment("载具信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CarrierCmd).HasColumnName("CARRIER_CMD");
            entity.Property(e => e.CarrierCode)
                .HasMaxLength(30)
                .HasColumnName("CARRIER_CODE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LoadState).HasColumnName("LOAD_STATE");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.LocationType).HasColumnName("LOCATION_TYPE");
            entity.Property(e => e.Marking)
                .HasMaxLength(30)
                .HasColumnName("MARKING");
            entity.Property(e => e.MaterialCode).HasColumnName("MATERIAL_CODE");
            entity.Property(e => e.MoveState).HasColumnName("MOVE_STATE");
            entity.Property(e => e.ProcessCode).HasColumnName("PROCESS_CODE");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.ToolingId).HasColumnName("TOOLING_ID");
        });

        modelBuilder.Entity<CwLogisticsLine>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_LINE", tb => tb.HasComment("货架信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.BoxCode)
                .HasMaxLength(30)
                .HasColumnName("BOX_CODE");
            entity.Property(e => e.CarrierType).HasColumnName("CARRIER_TYPE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Deep).HasColumnName("DEEP");
            entity.Property(e => e.Fork).HasColumnName("FORK");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LineDirect)
                .HasMaxLength(30)
                .HasColumnName("LINE_DIRECT");
            entity.Property(e => e.LineTypeId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LINE_TYPE_ID");
            entity.Property(e => e.LocationCmd1).HasColumnName("LOCATION_CMD1");
            entity.Property(e => e.LocationCmd2).HasColumnName("LOCATION_CMD2");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.RoadwayDirect).HasColumnName("ROADWAY_DIRECT");
            entity.Property(e => e.RoadwayId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ROADWAY_ID");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsLineGroup>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_LINE_GROUP", tb => tb.HasComment("线体组信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.BoxCode)
                .HasMaxLength(30)
                .HasColumnName("BOX_CODE");
            entity.Property(e => e.CarrierType).HasColumnName("CARRIER_TYPE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Deep).HasColumnName("DEEP");
            entity.Property(e => e.Fork).HasColumnName("FORK");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LineDirect)
                .HasMaxLength(30)
                .HasColumnName("LINE_DIRECT");
            entity.Property(e => e.LineTypeId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LINE_TYPE_ID");
            entity.Property(e => e.LocationCmd1).HasColumnName("LOCATION_CMD1");
            entity.Property(e => e.LocationCmd2).HasColumnName("LOCATION_CMD2");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.RoadwayDirect).HasColumnName("ROADWAY_DIRECT");
            entity.Property(e => e.RoadwayId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ROADWAY_ID");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsLineGroupDts>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_LINE_GROUP_DTS", tb => tb.HasComment("线体组明细"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.GroupId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("GROUP_ID");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.LocationId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LOCATION_ID");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsLineType>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_LINE_TYPE", tb => tb.HasComment("物流线类型信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LineTypeCode)
                .HasMaxLength(30)
                .HasColumnName("LINE_TYPE_CODE");
            entity.Property(e => e.LineTypeDescription)
                .HasMaxLength(100)
                .HasColumnName("LINE_TYPE_DESCRIPTION");
            entity.Property(e => e.LineTypeState).HasColumnName("LINE_TYPE_STATE");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsLocationType>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_LOCATION_TYPE");

            entity.HasIndex(e => e.Id, "AK_KEY_1_CW_LOGIS").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LocationDesciption)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LOCATION_DESCIPTION");
            entity.Property(e => e.LocationDesciptionEn)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LOCATION_DESCIPTION_EN");
            entity.Property(e => e.LocationName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LOCATION_NAME");
            entity.Property(e => e.LocationNameEn)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LOCATION_NAME_EN");
            entity.Property(e => e.LocationType).HasColumnName("LOCATION_TYPE");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsLogicRack>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_LOGIC_RACK", tb => tb.HasComment("逻辑货架信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.BoxCode)
                .HasMaxLength(30)
                .HasColumnName("BOX_CODE");
            entity.Property(e => e.CarrierType).HasColumnName("CARRIER_TYPE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Deep).HasColumnName("DEEP");
            entity.Property(e => e.Fork).HasColumnName("FORK");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LocationCmd1).HasColumnName("LOCATION_CMD1");
            entity.Property(e => e.LocationCmd2).HasColumnName("LOCATION_CMD2");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.LocatonDirect)
                .HasMaxLength(30)
                .HasColumnName("LOCATON_DIRECT");
            entity.Property(e => e.RackDirect).HasColumnName("RACK_DIRECT");
            entity.Property(e => e.RoadwayId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ROADWAY_ID");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsLogicRackDts>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_LOGIC_RACK_DTS", tb => tb.HasComment("逻辑货架明细信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.LogicId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LOGIC_ID");
            entity.Property(e => e.RackId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("RACK_ID");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CW_LOGISTICS_MATERIAL_DEFINED");

            entity.ToTable("CW_LOGISTICS_MATERIAL", tb => tb.HasComment("物料类型定义"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.MaterialCode).HasColumnName("MATERIAL_CODE");
            entity.Property(e => e.MaterialDesciption)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("MATERIAL_DESCIPTION");
            entity.Property(e => e.MaterialDesciptionEn)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("MATERIAL_DESCIPTION_EN");
            entity.Property(e => e.MaterialName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("MATERIAL_NAME");
            entity.Property(e => e.MaterialNameEn)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("MATERIAL_NAME_EN");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsMaterialInfo>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_MATERIAL_INFO", tb => tb.HasComment("物料信息明细"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Attribute1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE1");
            entity.Property(e => e.Attribute10)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE10");
            entity.Property(e => e.Attribute11)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE11");
            entity.Property(e => e.Attribute12)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE12");
            entity.Property(e => e.Attribute13)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE13");
            entity.Property(e => e.Attribute2)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE2");
            entity.Property(e => e.Attribute3)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE3");
            entity.Property(e => e.Attribute4)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE4");
            entity.Property(e => e.Attribute5)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE5");
            entity.Property(e => e.Attribute6)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE6");
            entity.Property(e => e.Attribute7)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE7");
            entity.Property(e => e.Attribute8)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE8");
            entity.Property(e => e.Attribute9)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ATTRIBUTE9");
            entity.Property(e => e.Barcode).HasColumnName("BARCODE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.IsDummy).HasColumnName("IS_DUMMY");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.MaterialPos).HasColumnName("MATERIAL_POS");
            entity.Property(e => e.MaterialState).HasColumnName("MATERIAL_STATE");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsMonitor>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_MONITOR", tb => tb.HasComment("监控图"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasComment("创建时间")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasComment("创建人")
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Diagram)
                .HasComment("GRAPH")
                .HasColumnName("DIAGRAM");
            entity.Property(e => e.LastModifyTime)
                .HasComment("修改时间")
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasComment("修改人")
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.MonitorCode)
                .HasComment("MONITOR_CODE")
                .HasColumnName("MONITOR_CODE");
            entity.Property(e => e.MonitorName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasComment("MONITOR_NAME")
                .HasColumnName("MONITOR_NAME");
            entity.Property(e => e.Parameters).HasColumnName("PARAMETERS");
            entity.Property(e => e.Status)
                .HasComment("状态")
                .HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsPlcDefined>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_PLC_DEFINED", tb => tb.HasComment("PLC位置定义"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.AreaCode).HasColumnName("AREA_CODE");
            entity.Property(e => e.AreaName)
                .HasMaxLength(30)
                .HasColumnName("AREA_NAME");
            entity.Property(e => e.BusinessType)
                .HasMaxLength(30)
                .HasColumnName("BUSINESS_TYPE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.DbNum).HasColumnName("DB_NUM");
            entity.Property(e => e.IpAddr)
                .HasMaxLength(16)
                .HasColumnName("IP_ADDR");
            entity.Property(e => e.IpPort).HasColumnName("IP_PORT");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LocationType).HasColumnName("LOCATION_TYPE");
            entity.Property(e => e.Offset).HasColumnName("OFFSET");
            entity.Property(e => e.Parameter).HasColumnName("PARAMETER");
            entity.Property(e => e.PlcVersion)
                .HasMaxLength(30)
                .HasColumnName("PLC_VERSION");
            entity.Property(e => e.Protocol)
                .HasMaxLength(30)
                .HasColumnName("PROTOCOL");
            entity.Property(e => e.StartAddress).HasColumnName("START_ADDRESS");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsProcessState>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_PROCESS_STATE", tb => tb.HasComment("物流工艺状态"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.ProcessCode).HasColumnName("PROCESS_CODE");
            entity.Property(e => e.ProcessName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("PROCESS_NAME");
            entity.Property(e => e.ProcessNameEn)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("PROCESS_NAME_EN");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsRack>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_RACK", tb => tb.HasComment("货架信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.BoxCode)
                .HasMaxLength(30)
                .HasColumnName("BOX_CODE");
            entity.Property(e => e.CarrierType).HasColumnName("CARRIER_TYPE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Deep).HasColumnName("DEEP");
            entity.Property(e => e.Fork).HasColumnName("FORK");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LocationCmd1).HasColumnName("LOCATION_CMD1");
            entity.Property(e => e.LocationCmd2).HasColumnName("LOCATION_CMD2");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.LocatonDirect)
                .HasMaxLength(30)
                .HasColumnName("LOCATON_DIRECT");
            entity.Property(e => e.RackDirect).HasColumnName("RACK_DIRECT");
            entity.Property(e => e.RoadwayId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ROADWAY_ID");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsRackState>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_RACK_STATE", tb => tb.HasComment("货架状态"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.AutoState).HasColumnName("AUTO_STATE");
            entity.Property(e => e.CloseState).HasColumnName("CLOSE_STATE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.FireState).HasColumnName("FIRE_STATE");
            entity.Property(e => e.FullLocked).HasColumnName("FULL_LOCKED");
            entity.Property(e => e.InTime)
                .HasColumnType("datetime")
                .HasColumnName("IN_TIME");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LoadState).HasColumnName("LOAD_STATE");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.PlanOutTime)
                .HasColumnType("datetime")
                .HasColumnName("PLAN_OUT_TIME");
            entity.Property(e => e.ProcessCode).HasColumnName("PROCESS_CODE");
            entity.Property(e => e.SampleITime)
                .HasColumnType("datetime")
                .HasColumnName("SAMPLE_I_TIME");
            entity.Property(e => e.SampleOTime)
                .HasColumnType("datetime")
                .HasColumnName("SAMPLE_O_TIME");
            entity.Property(e => e.SampleState).HasColumnName("SAMPLE_STATE");
            entity.Property(e => e.Sampler)
                .HasMaxLength(30)
                .HasColumnName("SAMPLER");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Temperature)
                .HasColumnType("decimal(8, 4)")
                .HasColumnName("TEMPERATURE");
            entity.Property(e => e.UseState).HasColumnName("USE_STATE");
        });

        modelBuilder.Entity<CwLogisticsRoad>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_ROAD", tb => tb.HasComment("物流路径"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.AfterCompleteService)
                .HasMaxLength(200)
                .HasColumnName("AFTER_COMPLETE_SERVICE");
            entity.Property(e => e.AfterCreateService)
                .HasMaxLength(200)
                .HasColumnName("AFTER_CREATE_SERVICE");
            entity.Property(e => e.AfterSendService)
                .HasMaxLength(200)
                .HasColumnName("AFTER_SEND_SERVICE");
            entity.Property(e => e.BeforeCompleteService)
                .HasMaxLength(200)
                .HasColumnName("BEFORE_COMPLETE_SERVICE");
            entity.Property(e => e.BeforeCreateService)
                .HasMaxLength(200)
                .HasColumnName("BEFORE_CREATE_SERVICE");
            entity.Property(e => e.BeforeSendService)
                .HasMaxLength(200)
                .HasColumnName("BEFORE_SEND_SERVICE");
            entity.Property(e => e.CarrierTypeService)
                .HasMaxLength(200)
                .HasColumnName("CARRIER_TYPE_SERVICE");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.FromAreaCode).HasColumnName("FROM_AREA_CODE");
            entity.Property(e => e.FromCondCode)
                .HasMaxLength(200)
                .HasColumnName("FROM_COND_CODE");
            entity.Property(e => e.FromLocationCmd).HasColumnName("FROM_LOCATION_CMD");
            entity.Property(e => e.FromLocationCode)
                .HasMaxLength(200)
                .HasColumnName("FROM_LOCATION_CODE");
            entity.Property(e => e.FromLocationType).HasColumnName("FROM_LOCATION_TYPE");
            entity.Property(e => e.FromMaterialCode).HasColumnName("FROM_MATERIAL_CODE");
            entity.Property(e => e.FromProcessCode).HasColumnName("FROM_PROCESS_CODE");
            entity.Property(e => e.FromScanCode).HasColumnName("FROM_SCAN_CODE");
            entity.Property(e => e.FromService)
                .HasMaxLength(200)
                .HasColumnName("FROM_SERVICE");
            entity.Property(e => e.FromState1).HasColumnName("FROM_STATE1");
            entity.Property(e => e.FromState2).HasColumnName("FROM_STATE2");
            entity.Property(e => e.FromState3).HasColumnName("FROM_STATE3");
            entity.Property(e => e.FromState4).HasColumnName("FROM_STATE4");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.TaskBalance).HasColumnName("TASK_BALANCE");
            entity.Property(e => e.ToAreaCode).HasColumnName("TO_AREA_CODE");
            entity.Property(e => e.ToCondCode)
                .HasMaxLength(200)
                .HasColumnName("TO_COND_CODE");
            entity.Property(e => e.ToLocationCmd).HasColumnName("TO_LOCATION_CMD");
            entity.Property(e => e.ToLocationCode)
                .HasMaxLength(200)
                .HasColumnName("TO_LOCATION_CODE");
            entity.Property(e => e.ToLocationType).HasColumnName("TO_LOCATION_TYPE");
            entity.Property(e => e.ToMaterialCode).HasColumnName("TO_MATERIAL_CODE");
            entity.Property(e => e.ToProcessCode).HasColumnName("TO_PROCESS_CODE");
            entity.Property(e => e.ToScanCode).HasColumnName("TO_SCAN_CODE");
            entity.Property(e => e.ToService)
                .HasMaxLength(200)
                .HasColumnName("TO_SERVICE");
            entity.Property(e => e.ToState1).HasColumnName("TO_STATE1");
            entity.Property(e => e.ToState2).HasColumnName("TO_STATE2");
            entity.Property(e => e.ToState3).HasColumnName("TO_STATE3");
            entity.Property(e => e.ToState4).HasColumnName("TO_STATE4");
        });

        modelBuilder.Entity<CwLogisticsRoadway>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_ROADWAY", tb => tb.HasComment("仓库巷道信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Administrator)
                .HasMaxLength(30)
                .HasColumnName("ADMINISTRATOR");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.RoadwayCode)
                .HasMaxLength(30)
                .HasColumnName("ROADWAY_CODE");
            entity.Property(e => e.RoadwayDescription)
                .HasMaxLength(100)
                .HasColumnName("ROADWAY_DESCRIPTION");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.WareId)
                .HasMaxLength(30)
                .HasColumnName("WARE_ID");
        });

        modelBuilder.Entity<CwLogisticsScanState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CW_LOGISTICS_SCAN_DEFINED");

            entity.ToTable("CW_LOGISTICS_SCAN_STATE", tb => tb.HasComment("扫码状态"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.ScanCode).HasColumnName("SCAN_CODE");
            entity.Property(e => e.ScanDesciption)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SCAN_DESCIPTION");
            entity.Property(e => e.ScanDesciptionEn)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SCAN_DESCIPTION_EN");
            entity.Property(e => e.ScanName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SCAN_NAME");
            entity.Property(e => e.ScanNameEn)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SCAN_NAME_EN");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsSubtaskInfo>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_SUBTASK_INFO", tb => tb.HasComment("任务表,用于堆垛机多差搬运状态"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CarrierCode)
                .HasMaxLength(30)
                .HasColumnName("CARRIER_CODE");
            entity.Property(e => e.CarrierId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CARRIER_ID");
            entity.Property(e => e.CompleteState).HasColumnName("COMPLETE_STATE");
            entity.Property(e => e.CompleteTime)
                .HasColumnType("datetime")
                .HasColumnName("COMPLETE_TIME");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Fork).HasColumnName("FORK");
            entity.Property(e => e.FromCmd1).HasColumnName("FROM_CMD1");
            entity.Property(e => e.FromCmd2).HasColumnName("FROM_CMD2");
            entity.Property(e => e.FromLoactionCode)
                .HasMaxLength(30)
                .HasColumnName("FROM_LOACTION_CODE");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.Priority).HasColumnName("PRIORITY");
            entity.Property(e => e.RoadId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ROAD_ID");
            entity.Property(e => e.SendState).HasColumnName("SEND_STATE");
            entity.Property(e => e.SendTime)
                .HasColumnType("datetime")
                .HasColumnName("SEND_TIME");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.TaskGroupState).HasColumnName("TASK_GROUP_STATE");
            entity.Property(e => e.TaskId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TASK_ID");
            entity.Property(e => e.TaskNumber).HasColumnName("TASK_NUMBER");
            entity.Property(e => e.TaskState).HasColumnName("TASK_STATE");
            entity.Property(e => e.ToCmd1).HasColumnName("TO_CMD1");
            entity.Property(e => e.ToCmd2).HasColumnName("TO_CMD2");
            entity.Property(e => e.ToLocationCode)
                .HasMaxLength(30)
                .HasColumnName("TO_LOCATION_CODE");
            entity.Property(e => e.TrakGroupId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TRAK_GROUP_ID");
        });

        modelBuilder.Entity<CwLogisticsSysParameter>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_SYS_PARAMETER", tb => tb.HasComment("调度参数"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.ParamCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("PARAM_CODE");
            entity.Property(e => e.ParamDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PARAM_DESCRIPTION");
            entity.Property(e => e.ParamDescriptionEn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PARAM_DESCRIPTION_EN");
            entity.Property(e => e.ParamName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PARAM_NAME");
            entity.Property(e => e.ParamNameEn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PARAM_NAME_EN");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsSysParameterItem>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_SYS_PARAMETER_ITEM", tb => tb.HasComment("调度参数"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ITEM_CODE");
            entity.Property(e => e.ItemDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITEM_DESCRIPTION");
            entity.Property(e => e.ItemDescriptionEn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITEM_DESCRIPTION_EN");
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITEM_NAME");
            entity.Property(e => e.ItemNameEn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITEM_NAME_EN");
            entity.Property(e => e.ItemValue)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITEM_VALUE");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.ParamId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("PARAM_ID");
            entity.Property(e => e.Status).HasColumnName("STATUS");
        });

        modelBuilder.Entity<CwLogisticsTaskInfo>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_TASK_INFO", tb => tb.HasComment("任务表"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CarrierCode)
                .HasMaxLength(30)
                .HasColumnName("CARRIER_CODE");
            entity.Property(e => e.CarrierId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CARRIER_ID");
            entity.Property(e => e.CompleteState).HasColumnName("COMPLETE_STATE");
            entity.Property(e => e.CompleteTime)
                .HasColumnType("datetime")
                .HasColumnName("COMPLETE_TIME");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Fork).HasColumnName("FORK");
            entity.Property(e => e.FromCmd1).HasColumnName("FROM_CMD1");
            entity.Property(e => e.FromCmd2).HasColumnName("FROM_CMD2");
            entity.Property(e => e.FromLoactionCode)
                .HasMaxLength(30)
                .HasColumnName("FROM_LOACTION_CODE");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.Priority).HasColumnName("PRIORITY");
            entity.Property(e => e.RoadId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ROAD_ID");
            entity.Property(e => e.SendState).HasColumnName("SEND_STATE");
            entity.Property(e => e.SendTime)
                .HasColumnType("datetime")
                .HasColumnName("SEND_TIME");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.TaskGroupState).HasColumnName("TASK_GROUP_STATE");
            entity.Property(e => e.TaskNumber).HasColumnName("TASK_NUMBER");
            entity.Property(e => e.TaskState).HasColumnName("TASK_STATE");
            entity.Property(e => e.ToCmd1).HasColumnName("TO_CMD1");
            entity.Property(e => e.ToCmd2).HasColumnName("TO_CMD2");
            entity.Property(e => e.ToLocationCode)
                .HasMaxLength(30)
                .HasColumnName("TO_LOCATION_CODE");
            entity.Property(e => e.TrakGroupId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TRAK_GROUP_ID");
        });

        modelBuilder.Entity<CwLogisticsTaskOption>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_TASK_OPTION", tb => tb.HasComment("任务选择传递"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.RoadIndex).HasColumnName("ROAD_INDEX");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("SERVICE_NAME");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.TimerId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TIMER_ID");

            entity.HasOne(d => d.Timer).WithMany(p => p.CwLogisticsTaskOption)
                .HasForeignKey(d => d.TimerId)
                .HasConstraintName("FK__CW_LOGIST__TIMER__628FA481");
        });

        modelBuilder.Entity<CwLogisticsTaskRoad>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_TASK_ROAD", tb => tb.HasComment("任务路径"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.RoadId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ROAD_ID");
            entity.Property(e => e.RoadIndex).HasColumnName("ROAD_INDEX");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.TimerId)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TIMER_ID");

            entity.HasOne(d => d.Timer).WithMany(p => p.CwLogisticsTaskRoad)
                .HasForeignKey(d => d.TimerId)
                .HasConstraintName("FK__CW_LOGIST__TIMER__6383C8BA");
        });

        modelBuilder.Entity<CwLogisticsTaskTimer>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_TASK_TIMER", tb => tb.HasComment("寻路任务表定时器"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.Cron)
                .HasMaxLength(30)
                .HasColumnName("CRON");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.Mutex)
                .HasMaxLength(50)
                .HasColumnName("MUTEX");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.TaskService)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TASK_SERVICE");
            entity.Property(e => e.Time).HasColumnName("TIME");
            entity.Property(e => e.UseState).HasColumnName("USE_STATE");
            entity.HasMany(d => d.CwLogisticsTaskRoad).WithOne(f => f.Timer).HasForeignKey(f=>f.TimerId);
            entity.HasMany(d => d.CwLogisticsTaskOption).WithOne(f => f.Timer).HasForeignKey(f => f.TimerId);

        });

        modelBuilder.Entity<CwLogisticsTooling>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_TOOLING", tb => tb.HasComment("工装载具定义"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.ToolingCmd).HasColumnName("TOOLING_CMD");
            entity.Property(e => e.ToolingCode)
                .HasMaxLength(30)
                .HasColumnName("TOOLING_CODE");
            entity.Property(e => e.ToolingDescription)
                .HasMaxLength(100)
                .HasColumnName("TOOLING_DESCRIPTION");
            entity.Property(e => e.UseState).HasColumnName("USE_STATE");
        });

        modelBuilder.Entity<CwLogisticsTrayInfo>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_TRAY_INFO", tb => tb.HasComment("托盘信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CarrierCmd).HasColumnName("CARRIER_CMD");
            entity.Property(e => e.CarrierId).HasColumnName("CARRIER_ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LoadState).HasColumnName("LOAD_STATE");
            entity.Property(e => e.LocationType).HasColumnName("LOCATION_TYPE");
            entity.Property(e => e.MaterialCode).HasColumnName("MATERIAL_CODE");
            entity.Property(e => e.MoveState).HasColumnName("MOVE_STATE");
            entity.Property(e => e.ProcessCode).HasColumnName("PROCESS_CODE");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.TrayCode)
                .HasMaxLength(30)
                .HasColumnName("TRAY_CODE");
            entity.Property(e => e.TrayIndex)
                .HasMaxLength(30)
                .HasColumnName("TRAY_INDEX");
        });

        modelBuilder.Entity<CwLogisticsWare>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CW_LOGISTICS_WARE_DEFINED");

            entity.ToTable("CW_LOGISTICS_WARE", tb => tb.HasComment("仓库定义信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Administrator)
                .HasMaxLength(30)
                .HasColumnName("ADMINISTRATOR");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(30)
                .HasColumnName("LOCATION_CODE");
            entity.Property(e => e.LocationDescription)
                .HasMaxLength(100)
                .HasColumnName("LOCATION_DESCRIPTION");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.WareTypeId)
                .HasMaxLength(30)
                .HasColumnName("WARE_TYPE_ID");
        });

        modelBuilder.Entity<CwLogisticsWareType>(entity =>
        {
            entity.ToTable("CW_LOGISTICS_WARE_TYPE", tb => tb.HasComment("仓库类型定义信息"));

            entity.Property(e => e.Id)
                .HasMaxLength(19)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CREATE_USER");
            entity.Property(e => e.LastModifyTime)
                .HasColumnType("datetime")
                .HasColumnName("LAST_MODIFY_TIME");
            entity.Property(e => e.LastModifyUser)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("LAST_MODIFY_USER");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.WareTypeCode)
                .HasMaxLength(30)
                .HasColumnName("WARE_TYPE_CODE");
            entity.Property(e => e.WareTypeDescription)
                .HasMaxLength(100)
                .HasColumnName("WARE_TYPE_DESCRIPTION");
            entity.Property(e => e.WareTypeState).HasColumnName("WARE_TYPE_STATE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
