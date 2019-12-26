using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ODCenter.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Text;

namespace ODCenter.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Ins_Type : int
    {
        [Description("常规")]
        Normal = 0,
        [Description("站点")]
        Station = 10
    }

    [Table("institute_list")]
    public class InstituteInfo
    {
        [Key, Column("id"), DisplayName("机构编码")]
        public Guid Id { get; set; }

        [Required, Column("name", TypeName = "varchar"), DisplayName("机构名称"), MaxLength(100)]
        public String Name { get; set; }

        [Column("type"), DisplayName("机构类型")]
        public Ins_Type Type { get; set; }

        [Column("parent"), DisplayName("所属机构")]
        public Guid? Parent { get; set; }

        [Column("location", TypeName = "varchar"), DisplayName("地理位置"), MaxLength(255)]
        public String Location { get; set; }

        [Column("longitude"), DisplayName("经度")]
        public Double? Longitude { get; set; }

        [Column("latitude"), DisplayName("纬度")]
        public Double? Latitude { get; set; }

        [Column("address"), DisplayName("地址")]
        public String Address { get; set; }

        [Column("summary"), DisplayName("简介"), DataType(DataType.MultilineText)]
        public String Summary { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }

        public Boolean Equals(InstituteInfo info)
        {
            if (info == null)
            {
                return false;
            }
            return this.Name == info.Name && this.Type == info.Type &&
                this.Location == info.Location && this.Longitude == info.Longitude &&
                this.Latitude == info.Latitude && this.Summary == info.Summary && this.Address == info.Address;
        }
    }

    [Table("institute_loc")]
    public class InstituteLocation
    {
        [Key, Column("id", Order = 1), DisplayName("机构编码")]
        public Guid Id { get; set; }

        [Key, Column("time", Order = 2), DisplayName("记录时间")]
        public DateTime Time { get; set; }

        [Column("longitude"), DisplayName("经度")]
        public Double? Longitude { get; set; }

        [Column("latitude"), DisplayName("纬度")]
        public Double? Latitude { get; set; }

        [Column("address"), DisplayName("地址")]
        public String Address { get; set; }
    }

    [Table("institute_log")]
    public class InstituteLog
    {
        [Key, Column("id"), DisplayName("日志编号")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("日志时间")]
        public DateTime LogTime { get; set; }

        [Column("instituteid"), DisplayName("机构编码")]
        public Guid InstituteId { get; set; }

        [Column("logtype"), DisplayName("日志类型")]
        public Int32 LogType { get; set; }

        [Column("operation", TypeName = "varchar"), DisplayName("日志描述"), MaxLength(1000)]
        public String Operation { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(50)]
        public String Operator { get; set; }
    }

    public class InstituteDbContext : DbContext
    {
        public DbSet<InstituteInfo> Institutes { get; set; }
        public DbSet<InstituteLocation> Locations { get; set; }
        public DbSet<InstituteLog> Logs { get; set; }

        public InstituteDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("institute");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<InstituteDbContext, Migrations.Institute.Configuration>());
            base.OnModelCreating(modelBuilder);
        }

        public InstituteLog CreateLog(InstituteInfo institute)
        {
            InstituteLog log = new InstituteLog();
            log.LogTime = DateTime.Now;
            log.InstituteId = institute.Id;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Institute (Name={0}, Parent={1}, Location={2}, Summary={3})",
                institute.Name, institute.Parent, institute.Location, institute.Summary);
            log.Operator = institute.Modifier;
            return log;
        }

        public InstituteLog EditLog(InstituteInfo oriinstitute, InstituteInfo newinstitute)
        {
            if (oriinstitute.Id == newinstitute.Id)
            {
                InstituteLog log = new InstituteLog();
                log.LogTime = DateTime.Now;
                log.InstituteId = oriinstitute.Id;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Institute");
                if (oriinstitute.Name != newinstitute.Name)
                {
                    builder.AppendFormat(" Name={0}->{1}", oriinstitute.Name, newinstitute.Name);
                }
                if (oriinstitute.Location != newinstitute.Location)
                {
                    builder.AppendFormat(" Location={0}->{1}", oriinstitute.Location, newinstitute.Location);
                }
                if (oriinstitute.Summary != newinstitute.Summary)
                {
                    builder.AppendFormat(" Summary={0}->{1}", oriinstitute.Location, newinstitute.Location);
                }
                log.Operation = builder.ToString();
                log.Operator = oriinstitute.Modifier;
                return log;
            }
            return null;
        }

        public InstituteLog DeleteLog(InstituteInfo institute)
        {
            InstituteLog log = new InstituteLog();
            log.LogTime = DateTime.Now;
            log.InstituteId = institute.Id;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Institute (Name={0}, Parent={1}, Location={2}, Summary={3})",
                institute.Name, institute.Parent, institute.Location, institute.Summary);
            log.Operator = institute.Modifier;
            return log;
        }
    }
}