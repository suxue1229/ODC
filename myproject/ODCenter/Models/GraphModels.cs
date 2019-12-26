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
    [Table("graph_list")]
    public class GraphInfo
    {
        [Key, Column("id"), DisplayName("画面编码")]
        public Guid Id { get; set; }

        [Column("name", TypeName = "varchar"), DisplayName("画面名称"), MaxLength(100)]
        public String Name { get; set; }

        [Column("institute"), DisplayName("所属机构")]
        public Guid Institute { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }

        public virtual List<GraphTag> Tags { get; set; }

        public GraphInfo()
        {
            Tags = new List<GraphTag>();
        }
    }

    [Table("graph_tag")]
    public class GraphTag
    {
        [Key, Column("id"), DisplayName("标签编码")]
        public Guid Id { get; set; }

        [Column("name"), DisplayName("标签名称")]
        public String Name { get; set; }

        [Column("sensorid"), DisplayName("仪表编码")]
        public Guid SensorId { get; set; }

        [Column("horizontal"),DisplayName("水平偏移")]
        public Double PosX { get; set; }

        [Column("vertical"), DisplayName("垂直偏移")]
        public Double PosY { get; set; }

        [Column("scale"), DisplayName("字体比例")]
        public Double Scale { get; set; }

        [Column("color"), DisplayName("字体颜色")]
        public Int32 Color { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }
    }

    [Table("graph_log")]
    public class GraphLog
    {
        [Key, Column("id"), DisplayName("分组编码")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("分组编码")]
        public DateTime LogTime { get; set; }

        [Column("graphid"), DisplayName("分组编码")]
        public Guid GraphId { get; set; }

        [Column("logtype"), DisplayName("日志类型")]
        public Int32 LogType { get; set; }

        [Column("operation", TypeName = "varchar"), DisplayName("分组编码"), MaxLength(1000)]
        public String Operation { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("分组编码"), MaxLength(50)]
        public String Operator { get; set; }
    }

    public class GraphDbContext : DbContext
    {
        public DbSet<GraphInfo> Graphs { get; set; }
        public DbSet<GraphTag> Tags{get;set;}
        public DbSet<GraphLog> Logs { get; set; }

        public GraphDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("graph");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<GraphDbContext, Migrations.Graph.Configuration>());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GraphInfo>().HasMany(x => x.Tags).WithRequired().Map(x => x.MapKey("groupid"));
        }

        public GraphLog CreateLog(GraphInfo graph)
        {
            GraphLog log = new GraphLog();
            log.LogTime = DateTime.Now;
            log.GraphId = graph.Id;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Graph (Name={0}, Institute={1})", graph.Name, graph.Institute);
            log.Operator = graph.Modifier;
            return log;
        }

        public GraphLog EditLog(GraphInfo origraph, GraphInfo newgraph)
        {
            if (origraph.Id == newgraph.Id)
            {
                GraphLog log = new GraphLog();
                log.LogTime = DateTime.Now;
                log.GraphId = origraph.Id;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Sensor");
                if (origraph.Name != newgraph.Name)
                {
                    builder.AppendFormat(" Name={0}->{1}", origraph.Name, newgraph.Name);
                }
                log.Operation = builder.ToString();
                log.Operator = origraph.Modifier;
                return log;
            }
            return null;
        }

        public GraphLog DeleteLog(GraphInfo graph)
        {
            GraphLog log = new GraphLog();
            log.LogTime = DateTime.Now;
            log.GraphId = graph.Id;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Graph (Name={0}, Institute={1})", graph.Name, graph.Institute);
            log.Operator = graph.Modifier;
            return log;
        }
    }
}