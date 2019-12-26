using ODCenter.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace ODCenter.Models
{
    [Table("client_list")]
    public class ClientInfo
    {
        [Key, Column("id"), DisplayName("客户端编码")]
        public Guid Id { get; set; }

        [Column("index"), DisplayName("客户端编号")]
        public Int32? Index { get; set; }

        [Column("dtuid"), DisplayName("DTU编号")]
        public Int32? Dtu { get; set; }

        [Column("name", TypeName = "varchar"), DisplayName("客户端名称"), MaxLength(100)]
        public String Name { get; set; }

        [Column("institute"), DisplayName("所属机构")]
        public Guid Institute { get; set; }

        [Column("longitude"), DisplayName("经度")]
        public Double? Longitude { get; set; }

        [Column("latitude"), DisplayName("纬度")]
        public Double? Latitude { get; set; }

        [Column("lastactive"), DisplayName("最后活跃")]
        public DateTime LastActive { get; set; }

        [Column("enabled"), DisplayName("是否有效")]
        public Boolean Enabled { get; set; }

        [Column("lastmodified", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(100)]
        public String Modifier { get; set; }

        public Boolean Equals(ClientInfo info)
        {
            if (info == null)
            {
                return false;
            }
            return this.Name == info.Name && this.Index == info.Index &&
                this.Dtu == info.Dtu && this.Longitude == info.Longitude &&
                this.Latitude == info.Latitude;
        }
    }

    [Table("client_loc")]
    public class ClientLocation
    {
        [Key, Column("id", Order = 1), DisplayName("客户端编码")]
        public Guid Id { get; set; }

        [Key, Column("time", Order = 2), DisplayName("记录时间")]
        public DateTime Time { get; set; }

        [Column("longitude"), DisplayName("经度")]
        public Double? Longitude { get; set; }

        [Column("latitude"), DisplayName("纬度")]
        public Double? Latitude { get; set; }
    }

    [Table("client_con")]
    public class ClientConn
    {
        [Key, Column("clientid", Order = 1), DisplayName("客户端编码")]
        public Guid ClientId { get; set; }

        [Key, Column("sensorid", Order = 2), DisplayName("仪表编码")]
        public Guid SensorId { get; set; }

        [Column("index"), DisplayName("仪表编号")]
        public Int32? SensorIndex { get; set; }

        public ClientConn()
        {

        }

        public ClientConn(Guid clientid, Guid sensorid, Int32? sensorindex = null)
        {
            this.ClientId = clientid;
            this.SensorId = sensorid;
            this.SensorIndex = sensorindex;
        }
    }

    [Table("client_log")]
    public class ClientLog
    {
        [Key, Column("id"), DisplayName("日志编号")]
        public Int32 Id { get; set; }

        [Column("time"), DisplayName("日志时间")]
        public DateTime LogTime { get; set; }

        [Column("clientid"), DisplayName("客户端编码")]
        public Guid ClientId { get; set; }

        [Column("logtype"), DisplayName("日志类型")]
        public Int32 LogType { get; set; }

        [Column("operation", TypeName = "varchar"), DisplayName("日志描述"), MaxLength(1000)]
        public String Operation { get; set; }

        [Column("operator", TypeName = "varchar"), DisplayName("操作人员"), MaxLength(50)]
        public String Operator { get; set; }
    }

    public class ClientDbContext : DbContext
    {
        public DbSet<ClientInfo> Clients { get; set; }
        public DbSet<ClientLocation> Locations { get; set; }
        public DbSet<ClientConn> Conns { get; set; }
        public DbSet<ClientLog> Logs { get; set; }

        public ClientDbContext()
            : base("DefaultConnection")
        {
            Statistic.LogDbRequest("client");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ClientDbContext, Migrations.Client.Configuration>());
            base.OnModelCreating(modelBuilder);
        }

        public ClientLog CreateLog(ClientInfo client)
        {
            ClientLog log = new ClientLog();
            log.LogTime = DateTime.Now;
            log.ClientId = client.Id;
            log.LogType = OperType.Create;
            log.Operation = String.Format("Create New Client (Name={0}, Institute={1})", client.Name, client.Institute);
            log.Operator = client.Modifier;
            return log;
        }

        public ClientLog EditLog(ClientInfo oriclient, ClientInfo newclient)
        {
            if (oriclient.Id == newclient.Id)
            {
                ClientLog log = new ClientLog();
                log.LogTime = DateTime.Now;
                log.ClientId = oriclient.Id;
                log.LogType = OperType.Modify;
                StringBuilder builder = new StringBuilder();
                builder.Append("Edit Client");
                if (oriclient.Name != newclient.Name)
                {
                    builder.AppendFormat(" Name={0}->{1}", oriclient.Name, newclient.Name);
                }
                if (oriclient.Index != newclient.Index)
                {
                    builder.AppendFormat(" Index={0}->{1}", oriclient.Index, newclient.Index);
                }
                if (oriclient.Dtu != newclient.Dtu)
                {
                    builder.AppendFormat(" Dtu={0}->{1}", oriclient.Dtu, newclient.Dtu);
                }
                log.Operation = builder.ToString();
                log.Operator = oriclient.Modifier;
                return log;
            }
            return null;
        }

        public ClientLog DeleteLog(ClientInfo client)
        {
            ClientLog log = new ClientLog();
            log.LogTime = DateTime.Now;
            log.ClientId = client.Id;
            log.LogType = OperType.Delete;
            log.Operation = String.Format("Delete Client (Name={0}, Institute={1})", client.Name, client.Institute);
            log.Operator = client.Modifier;
            return log;
        }
    }
}