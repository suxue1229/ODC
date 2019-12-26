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
    [Table("group_list")]
    public class GroupInfo
    {
        [Key, Column("id"), DisplayName("分组编码")]
        public Guid Id { get; set; }

        [Column("name", TypeName = "varchar"), DisplayName("分组名称"), MaxLength(100)]
        public String Name { get; set; }

        [Column("institute"), DisplayName("所属机构")]
        public Guid Institute { get; set; }

        [Column("group"), DisplayName("所属分组")]
        public Guid Group { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }
    }

    [Table("group_log")]
    public class GroupLog
    {
        [Key, Column("id"), DisplayName("分组编码")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("分组编码")]
        public DateTime LogTime { get; set; }

        [Column("groupid"), DisplayName("分组编码")]
        public Guid GroupId { get; set; }

        [Column("logtype"), DisplayName("日志类型")]
        public Int32 LogType { get; set; }

        [Column("operation", TypeName = "varchar"), DisplayName("分组编码"), MaxLength(1000)]
        public String Operation { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("分组编码"), MaxLength(50)]
        public String Operator { get; set; }
    }

    public class GroupDbContext : DbContext
    {
        public DbSet<GroupInfo> Groups { get; set; }
        public DbSet<GroupLog> Logs { get; set; }

        public GroupDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("group");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<GroupDbContext, Migrations.Group.Configuration>());
            base.OnModelCreating(modelBuilder);
        }

        public GroupLog CreateLog(GroupInfo group)
        {
            GroupLog log = new GroupLog();
            log.LogTime = DateTime.Now;
            log.GroupId = group.Id;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Group (Name={0}, Institute={1})", group.Name, group.Institute);
            log.Operator = group.Modifier;
            return log;
        }

        public GroupLog EditLog(GroupInfo origroup, GroupInfo newgroup)
        {
            if (origroup.Id == newgroup.Id)
            {
                GroupLog log = new GroupLog();
                log.LogTime = DateTime.Now;
                log.GroupId = origroup.Id;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Sensor");
                if (origroup.Name != newgroup.Name)
                {
                    builder.AppendFormat(" Name={0}->{1}", origroup.Name, newgroup.Name);
                }
                log.Operation = builder.ToString();
                log.Operator = origroup.Modifier;
                return log;
            }
            return null;
        }

        public GroupLog DeleteLog(GroupInfo group)
        {
            GroupLog log = new GroupLog();
            log.LogTime = DateTime.Now;
            log.GroupId = group.Id;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Group (Name={0}, Institute={1})", group.Name, group.Institute);
            log.Operator = group.Modifier;
            return log;
        }
    }
}