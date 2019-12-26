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
    [Table("report_list")]
    public class ReportInfo
    {
        [Key, Column("id"), DisplayName("报表编号")]
        public Guid Id { get; set; }

        [Required, Column("name", TypeName = "varchar"), DisplayName("报表名称"), MaxLength(100)]
        public String Name { get; set; }

        [Column("institute"), DisplayName("所属机构")]
        public Guid Institute { get; set; }

        [Column("pageinfo", TypeName = "varchar(MAX)"), DisplayName("页面设置")]
        public String PageInfo { get; set; }

        [Column("datasource", TypeName = "varchar(MAX)"), DisplayName("数据源")]
        public String DataSource { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }
    }

    [Table("report_log")]
    public class ReportLog
    {
        [Key, Column("id"), DisplayName("日志编号")]
        public Int32 LogId { get; set; }

        [Column("time"), DisplayName("日志时间")]
        public DateTime LogTime { get; set; }

        [Required, Column("reportid"), DisplayName("报表编号")]
        public Guid ReportId { get; set; }

        [Column("logtype"), DisplayName("日志类型")]
        public Int32 LogType { get; set; }

        [Column("operation", TypeName = "varchar"), DisplayName("日志描述")]
        public String Operation { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(50)]
        public String Operator { get; set; }
    }

    public class ReportDbContext : DbContext
    {
        public DbSet<ReportInfo> Reports { get; set; }
        public DbSet<ReportLog> Logs { get; set; }

        public ReportDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("report");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ReportDbContext, Migrations.Report.Configuration>());
            base.OnModelCreating(modelBuilder);
        }

        public ReportLog CreateLog(ReportInfo report)
        {
            ReportLog log = new ReportLog();
            log.LogTime = DateTime.Now;
            log.ReportId = report.Id;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Report (Name={0}, Institute={1}, PageInfo={2}, Variable={3})",
                report.Name, report.Institute, report.PageInfo, report.DataSource);
            log.Operator = report.Modifier;
            return log;
        }

        public ReportLog EditLog(ReportInfo orireport, ReportInfo newreport)
        {
            if (orireport.Id == newreport.Id)
            {
                ReportLog log = new ReportLog();
                log.LogTime = DateTime.Now;
                log.ReportId = orireport.Id;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Report");
                if (orireport.Name != newreport.Name)
                {
                    builder.AppendFormat(" Name={0}->{1}", orireport.Name, newreport.Name);
                }
                if (orireport.PageInfo != newreport.PageInfo)
                {
                    builder.AppendFormat(" PageInfo={0}->{1}", orireport.PageInfo, newreport.PageInfo);
                }
                if (orireport.DataSource != newreport.DataSource)
                {
                    builder.AppendFormat(" Variable={0}->{1}", orireport.DataSource, newreport.DataSource);
                }
                log.Operation = builder.ToString();
                log.Operator = orireport.Modifier;
                return log;
            }
            return null;
        }

        public ReportLog RenderLog(ReportInfo report, String format,String user)
        {
            ReportLog log = new ReportLog();
            log.LogTime = DateTime.Now;
            log.ReportId = report.Id;
            log.LogType = OperType.Render;
            log.Operation = String.Format("Render Report (Name={0}, Institute={1}, PageInfo={2}, Variable={3}, Format={4})",
                report.Name, report.Institute, report.PageInfo, report.DataSource, format);
            log.Operator = user;
            return log;
        }

        public ReportLog DeleteLog(ReportInfo report)
        {
            ReportLog log = new ReportLog();
            log.LogTime = DateTime.Now;
            log.ReportId = report.Id;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Report (Name={0}, Institute={1}, PageInfo={2}, Variable={3})",
                report.Name, report.Institute, report.PageInfo, report.DataSource);
            log.Operator = report.Modifier;
            return log;
        }
    }

    public class OperType
    {
        public const Int32 Create = 100;
        public const Int32 Modify = 200;
        public const Int32 Delete = 300;
        public const Int32 Render = 400;
    }
}