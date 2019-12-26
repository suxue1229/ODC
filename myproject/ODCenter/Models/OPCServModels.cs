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
  
    [Table("OPCServerServerInfo_list")]
    public class OPCServerInfo
    {
        [Key, Column("id"), DisplayName("服务器编码")]
        public Guid Id { get; set; }

        [Required, Column("IP"), DisplayName("IP")]
        public String IP { get; set; }

        [Required, Column("OPCServerName"), DisplayName("服务器")]
        public String OPCServerName { get; set; }

        [Key, Column("time", Order = 2), DisplayName("记录时间")]
        public DateTime Time { get; set; }

        public OPCServerInfo() {
            IP = "127.0.0.1";
            OPCServerName = "KEPware.KEPServerEx.V4";
        }

    }

    public class OPCServerDbContext : DbContext
    {
        public DbSet<OPCServerInfo> Institutes { get; set; }

        public OPCServerDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("OPCServer");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<OPCServerDbContext, Migrations.OPCServer.Configuration>());
            base.OnModelCreating(modelBuilder);
        }

       
    }
}