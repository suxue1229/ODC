using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ODCenter.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace ODCenter.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Dat_Type : int
    {
        [Description("自动")]
        Auto = 0,
        [Description("布尔型")]
        BOOL = 10,
        [Description("短整型")]
        SINT = 20,
        [Description("整型")]
        INT = 30,
        [Description("长整型")]
        DINT = 40,
        [Description("浮点型")]
        REAL = 50,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Sen_Type : int
    {
        [Description("未定义")]
        Undefined = 0,

        [Description("运行信号")]
        Status_Run = 10,
        [Description("开关信号")]
        Status_Open = 11,

        [Description("手动模式")]
        Status_Manual = 20,

        [Description("故障信号")]
        Status_Error = 30,

        [Description("温度")]
        Temperature = 100,
        [Description("pH")]
        pH = 110,
        [Description("浓度")]
        Concentration = 120,
        [Description("浊度")]
        Turbidity = 130,


        [Description("电压")]
        Electric_Voltage = 200,
        [Description("电流")]
        Electric_Current = 210,
        [Description("频率")]
        Electric_Frequency = 220,
        [Description("功率")]
        Electric_Power = 230,
        [Description("电能")]
        Electric_Energy = 240,

        [Description("压力")]
        Pressure = 300,
        [Description("瞬时流量")]
        Flowrate = 310,
        [Description("累计流量")]
        Flowsum = 320,

        [Description("长度")]
        Length = 400
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Sen_Source : int
    {
        [Description("自动采集")]
        Client = 0,
        [Description("手动输入")]
        Manual = 10,
        [Description("任意方式")]
        Auto = 20,
    }

    [Table("sensor_list")]
    public class SensorInfo
    {
        [Key, Column("id"), DisplayName("仪表编码")]
        public Guid Id { get; set; }

        [Required, Column("name", TypeName = "varchar"), DisplayName("仪表名称"), MaxLength(100)]
        public String Name { get; set; }

        [Column("institute"), DisplayName("所属机构")]
        public Guid Institute { get; set; }

        [Column("device"), DisplayName("所属设备")]
        public Guid? Device { get; set; }

        [Column("dattype"), DisplayName("数据类型")]
        public Dat_Type DatType { get; set; }

        //压力、流量、温度、浊度、浓度(氨氮、溶氧、COD)、体积(累计流量)、高度(液位)、PH
        //电压、电流、功率、频率
        [Column("sentype"), DisplayName("仪表类型")]
        public Sen_Type SenType { get; set; }

        [Column("inverse"), DisplayName("信号反转")]
        public Boolean Inverse { get; set; }

        [Column("precise"), DisplayName("显示精度")]
        public Int32? Precise { get; set; }

        [Column("source"), DisplayName("数据来源")]
        public Sen_Source DatSource { get; set; }

        [Column("nosave"), DisplayName("数据挥发")]
        public Boolean Nosave { get; set; }

        [Column("opchost", TypeName = "varchar"), DisplayName("OPC主机"), MaxLength(100)]
        public String OPCHost { get; set; }

        [Column("opcserver", TypeName = "varchar"), DisplayName("OPC服务器"), MaxLength(100)]
        public String OPCServer { get; set; }

        [Column("opcitem", TypeName = "varchar"), DisplayName("OPC变量"), MaxLength(100)]
        public String OPCItem { get; set; }

        [Required, Column("interval"), DisplayName("更新间隔"), Range(1, 3600)]
        public Int32 Interval { get; set; }

        [Column("minimum"), DisplayName("数值下限")]
        public Double? Minimum { get; set; }

        [Column("maximum"), DisplayName("数值上限")]
        public Double? Maximum { get; set; }

        [Required, Column("gain"), DisplayName("数值增益")]
        public Double Gain { get; set; }

        [Required, Column("offset"), DisplayName("数值偏移")]
        public Double Offset { get; set; }

        [Column("unit", TypeName = "varchar"), DisplayName("数值单位"), MaxLength(50)]
        public String Unit { get; set; }

        [Column("order"), DisplayName("显示顺序")]
        public Int32? Order { get; set; }

        [Column("important"), DisplayName("关键数据")]
        public Boolean Important { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }

        public Boolean Equals(SensorInfo info)
        {
            if (info == null)
            {
                return false;
            }
            return this.Name == info.Name && this.DatType == info.DatType &&
                this.SenType == info.SenType && this.Inverse == info.Inverse &&
                 this.Precise == info.Precise && this.DatSource == info.DatSource &&
                 this.OPCHost == info.OPCHost && this.OPCServer == info.OPCServer &&
                 this.OPCItem == info.OPCItem && this.Interval == info.Interval &&
                this.Minimum == info.Minimum && this.Maximum == info.Maximum &&
                this.Gain == info.Gain && this.Offset == info.Offset &&
                this.Unit == info.Unit && this.Order == info.Order && this.Important == info.Important;
        }
    }

    [Table("sensor_con")]
    public class SensorConn
    {
        [Key, Column("groupid", Order = 1), DisplayName("分组编码")]
        public Guid GroupId { get; set; }

        [Key, Column("sensorid", Order = 2), DisplayName("仪表编码")]
        public Guid SensorId { get; set; }

        [Column("visible"), DisplayName("是否显示")]
        public Boolean Visible { get; set; }

        public SensorConn()
        {

        }

        public SensorConn(Guid groupid, Guid sensorid, Boolean visible)
        {
            this.GroupId = groupid;
            this.SensorId = sensorid;
            this.Visible = visible;
        }
    }

    [Table("sensor_log")]
    public class SensorLog
    {
        [Key, Column("id"), DisplayName("日志编号")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("日志时间")]
        public DateTime LogTime { get; set; }

        [Column("sensorid"), DisplayName("仪表编码")]
        public Guid SensorId { get; set; }

        [Column("logtype"), DisplayName("日志类型")]
        public Int32 LogType { get; set; }

        [Column("operation", TypeName = "varchar"), DisplayName("日志描述"), MaxLength(1000)]
        public String Operation { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(50)]
        public String Operator { get; set; }
    }

    public class SensorDbContext : DbContext
    {
        public DbSet<SensorInfo> Sensors { get; set; }
        public DbSet<SensorConn> Conns { get; set; }
        public DbSet<SensorLog> Logs { get; set; }

        public SensorDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("sensor");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SensorDbContext, Migrations.Sensor.Configuration>());
            base.OnModelCreating(modelBuilder);
        }

        public SensorLog CreateLog(SensorInfo sensor)
        {
            SensorLog log = new SensorLog();
            log.LogTime = DateTime.Now;
            log.SensorId = sensor.Id;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Sensor  (Name={0}, SenType={1}, DatType={2}, Precise={3}, DatSource={4}, Institute={5}, OPCHost={6}, OPCServer={7}, OPCItem={8}, Interval={9}, Gain={10}, Offset={11}, Unit={12}, Order={13}, Important={14})",
                sensor.Name, sensor.SenType, sensor.DatType, sensor.Precise, sensor.DatSource, sensor.Institute, sensor.OPCHost, sensor.OPCServer, sensor.OPCItem, sensor.Interval, sensor.Gain, sensor.Offset, sensor.Unit, sensor.Order, sensor.Important);
            log.Operator = sensor.Modifier;
            return log;
        }

        public SensorLog EditLog(SensorInfo orisensor, SensorInfo newsensor)
        {
            if (orisensor.Id == newsensor.Id)
            {
                SensorLog log = new SensorLog();
                log.LogTime = DateTime.Now;
                log.SensorId = orisensor.Id;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Sensor");
                if (orisensor.Name != newsensor.Name)
                {
                    builder.AppendFormat(" Name={0}->{1}", orisensor.Name, newsensor.Name);
                }
                if (orisensor.SenType != newsensor.SenType)
                {
                    builder.AppendFormat(" SenType={0}->{1}", orisensor.SenType, newsensor.SenType);
                }
                if (orisensor.DatType != newsensor.DatType)
                {
                    builder.AppendFormat(" DatType={0}->{1}", orisensor.DatType, newsensor.DatType);
                }
                if (orisensor.Precise != newsensor.Precise)
                {
                    builder.AppendFormat(" Precise={0}->{1}", orisensor.Precise, newsensor.Precise);
                }
                if (orisensor.DatSource != newsensor.DatSource)
                {
                    builder.AppendFormat(" DatSource={0}->{1}", orisensor.DatSource, newsensor.DatSource);
                }
                if (orisensor.OPCHost != newsensor.OPCHost)
                {
                    builder.AppendFormat(" OPCHost={0}->{1}", orisensor.OPCHost, newsensor.OPCHost);
                }
                if (orisensor.OPCServer != newsensor.OPCServer)
                {
                    builder.AppendFormat(" OPCServer={0}->{1}", orisensor.OPCServer, newsensor.OPCServer);
                }
                if (orisensor.OPCItem != newsensor.OPCItem)
                {
                    builder.AppendFormat(" OPCItem={0}->{1}", orisensor.OPCItem, newsensor.OPCItem);
                }
                if (orisensor.Interval != newsensor.Interval)
                {
                    builder.AppendFormat(" Interval={0}->{1}", orisensor.Interval, newsensor.Interval);
                }
                if (orisensor.Gain != newsensor.Gain)
                {
                    builder.AppendFormat(" Gain={0}->{1}", orisensor.Gain, newsensor.Gain);
                }
                if (orisensor.Offset != newsensor.Offset)
                {
                    builder.AppendFormat(" Offset={0}->{1}", orisensor.Offset, newsensor.Offset);
                }
                if (orisensor.Unit != newsensor.Unit)
                {
                    builder.AppendFormat(" Unit={0}->{1}", orisensor.Unit, newsensor.Unit);
                }
                if (orisensor.Order != newsensor.Order)
                {
                    builder.AppendFormat(" Order={0}->{1}", orisensor.Order, newsensor.Order);
                }
                if (orisensor.Important != newsensor.Important)
                {
                    builder.AppendFormat(" Important={0}->{1}", orisensor.Important, newsensor.Important);
                }
                log.Operation = builder.ToString();
                log.Operator = orisensor.Modifier;
                return log;
            }
            return null;
        }

        public SensorLog DeleteLog(SensorInfo sensor)
        {
            SensorLog log = new SensorLog();
            log.LogTime = DateTime.Now;
            log.SensorId = sensor.Id;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Sensor (Name={0}, SenType={1}, DatType={2}, Precise={3}, DatSource={4}, Institute={5}, OPCHost={6}, OPCServer={7}, OPCItem={8}, Interval={9}, Gain={10}, Offset={11}, Unit={12}, Order={13}, Important={14})",
                sensor.Name, sensor.SenType, sensor.DatType, sensor.Precise, sensor.DatSource, sensor.Institute, sensor.OPCHost, sensor.OPCServer, sensor.OPCItem, sensor.Interval, sensor.Gain, sensor.Offset, sensor.Unit, sensor.Order, sensor.Important);
            log.Operator = sensor.Modifier;
            return log;
        }
    }
}