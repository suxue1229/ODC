using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using ODCenter.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ODCenter.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Dev_Stat : int
    {
        [Description("自动生成")]
        Automatic = 0,
        [Description("手动修改")]
        Manual = 10,
        [Description("不限制")]
        Unlimited = 20,
        [Description("禁止修改")]
        None = 30
    }

    [Table("device_list")]
    public class DeviceInfo
    {
        [Key, Column("id"), DisplayName("设备编码")]
        public Guid Id { get; set; }

        [Required, Column("name", TypeName = "varchar"), DisplayName("设备名称"), MaxLength(100)]
        public String Name { get; set; }

        [Column("institute"), DisplayName("所属机构")]
        public Guid Institute { get; set; }

        [Column("statsrc"), DisplayName("状态来源")]
        public Dev_Stat Statsrc { get; set; }

        [Column("status", TypeName = "varchar"), DisplayName("设备状态"), MaxLength(100)]
        public String Status { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }

        public Boolean Equals(DeviceInfo info)
        {
            if (info == null)
            {
                return false;
            }
            return this.Name == info.Name && this.Statsrc == info.Statsrc &&
                this.Status == info.Status;
        }
    }

    [Table("device_con")]
    public class DeviceConn
    {
        [Key, Column("groupid", Order = 1), DisplayName("分组编码")]
        public Guid GroupId { get; set; }

        [Key, Column("deviceid", Order = 2), DisplayName("设备编码")]
        public Guid DeviceId { get; set; }

        public DeviceConn()
        {

        }

        public DeviceConn(Guid groupid, Guid deviceid)
        {
            this.GroupId = groupid;
            this.DeviceId = deviceid;
        }
    }

    [Table("device_stat")]
    public class DeviceStat
    {
        [Key, Column("id"), DisplayName("记录编号")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("记录时间")]
        public DateTime Time { get; set; }

        [Column("deviceid"), DisplayName("设备编码")]
        public Guid DeviceId { get; set; }

        [Column("status"), DisplayName("设备状态")]
        public String Status { get; set; }

        [Column("source"), DisplayName("状态来源")]
        public Dev_Stat Source { get; set; }
    }

    [Table("device_trk")]
    public class DeviceTrack
    {
        [Key, Column("id"), DisplayName("记录编号")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("记录时间")]
        public DateTime Time { get; set; }

        [Column("deviceid"), DisplayName("设备编码")]
        public Guid DeviceId { get; set; }

        [Required, Column("description", TypeName = "varchar"), DisplayName("操作描述")]
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(50)]
        public String Operator { get; set; }
    }

    [Table("device_log")]
    public class DeviceLog
    {
        [Key, Column("id"), DisplayName("日志编号")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("日志时间")]
        public DateTime LogTime { get; set; }

        [Column("deviceid"), DisplayName("设备编码")]
        public Guid DeviceId { get; set; }

        [Column("logtype"), DisplayName("日志类型")]
        public Int32 LogType { get; set; }

        [Column("operation", TypeName = "varchar"), DisplayName("日志描述"), MaxLength(1000)]
        public String Operation { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(50)]
        public String Operator { get; set; }
    }

    public class DeviceDbContext : DbContext
    {
        public DbSet<DeviceInfo> Devices { get; set; }
        public DbSet<DeviceConn> Conns { get; set; }
        public DbSet<DeviceStat> Stats { get; set; }
        public DbSet<DeviceTrack> Tracks { get; set; }
        public DbSet<DeviceLog> Logs { get; set; }

        public DeviceDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("device");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DeviceDbContext, Migrations.Device.Configuration>());
            base.OnModelCreating(modelBuilder);
        }

        public DeviceLog CreateLog(DeviceInfo device)
        {
            DeviceLog log = new DeviceLog();
            log.LogTime = DateTime.Now;
            log.DeviceId = device.Id;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Device (Name={0}, Institute={1}, Statsrc={2}, Status={3})",
                device.Name, device.Institute, device.Statsrc.Description(), device.Status);
            log.Operator = device.Modifier;
            return log;
        }

        public DeviceLog CreateLog(DeviceTrack track)
        {
            DeviceLog log = new DeviceLog();
            log.LogTime = DateTime.Now;
            log.DeviceId = track.DeviceId;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Device Track (Description={0})", track.Description);
            log.Operator = track.Operator;
            return log;
        }

        public DeviceLog EditLog(DeviceInfo oridevice, DeviceInfo newdevice)
        {
            if (oridevice.Id == newdevice.Id)
            {
                DeviceLog log = new DeviceLog();
                log.LogTime = DateTime.Now;
                log.DeviceId = oridevice.Id;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Device");
                if (oridevice.Name != newdevice.Name)
                {
                    builder.AppendFormat(" Name={0}->{1}", oridevice.Name, newdevice.Name);
                }
                if (oridevice.Statsrc != newdevice.Statsrc)
                {
                    builder.AppendFormat(" Statsrc={0}->{1}", oridevice.Statsrc.Description(), newdevice.Statsrc.Description());
                }
                if (oridevice.Status != newdevice.Status)
                {
                    builder.AppendFormat(" Status={0}->{1}", oridevice.Status, newdevice.Status);
                }
                log.Operation = builder.ToString();
                log.Operator = oridevice.Modifier;
                return log;
            }
            return null;
        }

        public DeviceStat EditLog(DeviceInfo device, String status,Dev_Stat source)
        {
            return new DeviceStat()
            {
                Time = DateTime.Now,
                DeviceId = device.Id,
                Status = status,
                Source = source
            };
        }

        public DeviceLog EditLog(DeviceTrack oritrack, DeviceTrack newtrack)
        {
            if (oritrack.Id == newtrack.Id)
            {
                DeviceLog log = new DeviceLog();
                log.LogTime = DateTime.Now;
                log.DeviceId = oritrack.DeviceId;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Device Track");
                if (oritrack.Description != newtrack.Description)
                {
                    builder.AppendFormat(" Description={0}->{1}", oritrack.Description, newtrack.Description);
                }
                log.Operation = builder.ToString();
                log.Operator = oritrack.Operator;
                return log;
            }
            return null;
        }

        public DeviceLog DeleteLog(DeviceInfo device)
        {
            DeviceLog log = new DeviceLog();
            log.LogTime = DateTime.Now;
            log.DeviceId = device.Id;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Device (Name={0}, Institute={1})", device.Name, device.Institute);
            log.Operator = device.Modifier;
            return log;
        }

        public DeviceLog DeleteLog(DeviceTrack track)
        {
            DeviceLog log = new DeviceLog();
            log.LogTime = DateTime.Now;
            log.DeviceId = track.DeviceId;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Device Track (Description={0})", track.Description);
            log.Operator = track.Operator;
            return log;
        }
    }
}