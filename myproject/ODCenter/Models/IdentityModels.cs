using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ODCenter.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ODCenter.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public virtual UserGroup Group { get; set; }

        [DisplayName("名"), MaxLength(20)]
        public String FirstName { get; set; }

        [DisplayName("姓"), MaxLength(20)]
        public String LastName { get; set; }

        [DisplayName("昵称"), MaxLength(50)]
        public String NickName { get; set; }

        [DisplayName("单位"), MaxLength(50)]
        public String Company { get; set; }

        [DisplayName("部门"), MaxLength(50)]
        public String Department { get; set; }

        [DisplayName("注册时间")]
        public DateTime? Register { get; set; }

        [DisplayName("最近登陆")]
        public DateTime? LastLogin { get; set; }

        [DisplayName("登陆失败")]
        public DateTime? LastFail { get; set; }

        [DisplayName("失败计数")]
        public Int32? TotalFail { get; set; }

        [Column("institute")]
        public Guid? Institute { get; set; }

        public Guid[] Institutes
        {
            get
            {
                return Account.GetInstitutes(this.UserName);
            }
            set
            {
                Account.SetInstitutes(this.UserName, value);
            }
        }

        public void PopulateIdentity(UserManager<ApplicationUser> manager, ref ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim(Const.Claim_Institute_Id,
                this.Institute.HasValue ? this.Institute.Value.ToString("N") : Guid.Empty.ToString("N")));
            identity.AddClaim(new Claim(Const.Claim_Institute_Name,
                (this.Institute.HasValue && DbProvider.Institutes != null) ? DbProvider.Institutes[this.Institute.Value.ToString("N")].Name : "Unknown"));
            foreach (String role in UserRoles.All())
            {
                    if (manager.IsInRole(this.Id, role))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
 
            }
        }

        public ClaimsIdentity GenerateUserIdentity(UserManager<ApplicationUser> manager,
            String authtype = DefaultAuthenticationTypes.ApplicationCookie)
        {
            var identity = manager.CreateIdentity(this, authtype);
            PopulateIdentity(manager, ref identity);
            return identity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager,
            String authtype = DefaultAuthenticationTypes.ApplicationCookie)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var identity = await manager.CreateIdentityAsync(this, authtype);

            // 在此处添加自定义用户声明
            PopulateIdentity(manager, ref identity);

            return identity;
        }
    }

    [Table("user_group")]
    public class UserGroup
    {
        [Key, DisplayName("编号")]
        public Guid Id { get; set; }

        [Required, DisplayName("用户组"), MaxLength(50)]
        public String Name { get; set; }

        [DisplayName("描述"), MaxLength(1000)]
        public String Description { get; set; }

        [DisplayName("创建时间")]
        public DateTime Created { get; set; }

        [DisplayName("操作人员")]
        public String Operator { get; set; }

        //public virtual ICollection<ApplicationUser> Users { get; set; }
        //public virtual ICollection<ApplicationRole> Roles { get; set; }
    }

    [Table("user_con")]
    public class UserConn
    {
        [Key, Column("username", Order = 1), DisplayName("用户名")]
        public String UserName { get; set; }

        [Key, Column("institute", Order = 2), DisplayName("机构编码")]
        public Guid Institute { get; set; }

        public UserConn()
        {

        }

        public UserConn(String userid, Guid insid)
            : this()
        {
            this.UserName = userid;
            this.Institute = insid;
        }
    }

    [Table("user_invite")]
    public class UserInvite
    {
        [Key, Required, DisplayName("编号")]
        public String Id { get; set; }

        [DisplayName("注释")]
        public String Description { get; set; }

        [DisplayName("生成时间")]
        public DateTime Issued { get; set; }

        [DisplayName("过期时间")]
        public DateTime Expired { get; set; }

        [DisplayName("使用时间")]
        public DateTime? Used { get; set; }

        [DisplayName("激活账户")]
        public String Account { get; set; }
    }

    [Table("user_token")]
    public class UserToken
    {
        [Key, DisplayName("编号")]
        public String Id { get; set; }

        [DisplayName("主题")]
        public String Subject { get; set; }

        [DisplayName("生成时间")]
        public DateTime IssuedUtc { get; set; }

        [DisplayName("过期时间")]
        public DateTime ExpiresUtc { get; set; }

        [DisplayName("令牌")]
        public String Ticket { get; set; }
    }

    [Table("user_access")]
    public class UserAccess
    {
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        [Key, Column(Order = 2)]
        public DateTime Time { get; set; }

        public String Remote { get; set; }

        public Boolean Grant { get; set; }

        public String Remark { get; set; }
    }

    public class UserDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<UserGroup> Groups { get; set; }
        public DbSet<UserConn> Conns { get; set; }
        public DbSet<UserInvite> Invites { get; set; }
        public DbSet<UserToken> Tokens { get; set; }
        public DbSet<UserAccess> Access { get; set; }

        public UserDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Statistic.LogDbRequest("user");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<UserDbContext, Migrations.User.Configuration>());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().ToTable("user_accounts");
            modelBuilder.Entity<ApplicationUser>().ToTable("user_accounts");
            modelBuilder.Entity<IdentityRole>().ToTable("sys_roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("user_roles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("user_logins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("user_claims");
        }

        public static UserDbContext Create()
        {
            return new UserDbContext();
        }
    }
}