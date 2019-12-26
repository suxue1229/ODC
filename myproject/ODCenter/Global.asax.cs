using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using ODCenter.Base;
using ODCenter.Models;
using PTR.Lbs.Baidu;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ODCenter
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Timer jobTimer = null;
        Int32 curMin = DateTime.Now.Minute, curHour = DateTime.Now.Hour;
        DateTime sync_client = DateTime.Now;

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            String msg = String.Format("ODCenter v{0}.{1}.{2} build {3} start running...", ver.Major, ver.Minor, ver.Build, ver.Revision);
            Logger.EventSource = "ODCenter";
            Logger.LogMessage(msg);
            LbsConfig.AK = ConfigurationManager.AppSettings["lbs:ak"];
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    conn.Close();
                }
                catch { }
            }

            DbProvider.Load();

            if (Institute.Excludes == null)
            {
                Institute.Excludes = new List<Guid>();
            }
            else
            {
                Institute.Excludes.Clear();
            }
            foreach (String id in ConfigurationManager.AppSettings["institute:exclude"].Split(new[] { ',', ';' }))
            {
                Guid insid = Guid.Empty;
                if (Guid.TryParse(id, out insid))
                {
                    Institute.Excludes.Add(insid);
                }
            }
            Router.Manbase = ConfigurationManager.AppSettings["router:manbase"];

            jobTimer = new Timer(1000);
            jobTimer.Elapsed += JobRoutine;
            jobTimer.Start();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                Converters = new List<JsonConverter> { new GuidConverter()/*, new StringEnumConverter()*/ }
            };
        }

        protected void JobRoutine(object sender, ElapsedEventArgs e)
        {
            jobTimer.Stop();

            #region 每3分钟收集数据
#if DEBUG
            if (true)
#else
            if (DateTime.Now.Minute % 3 == 0 && curMin != DateTime.Now.Minute)
#endif
            {
                try
                {
                    foreach (Sensor sen in DbProvider.Sensors.Values)
                    {
                        sen.Record();
                    }
                    curMin = DateTime.Now.Minute;
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when collecting data.", ex);
                    HttpRuntime.UnloadAppDomain();
                }
                try
                {
                    if (DbProvider.Clients.Values.FirstOrDefault(c => c.LastActive > sync_client) != null)
                    {
                        using (var db_cli = new ClientDbContext())
                        {
                            foreach (ClientInfo client in db_cli.Clients)
                            {
                                if (!client.Enabled || !DbProvider.Clients.ContainsKey(client.Id.ToString("N")))
                                {
                                    continue;
                                }
                                Client cli = DbProvider.Clients[client.Id.ToString("N")];
                                if (cli.LastActive == client.LastActive)
                                {
                                    continue;
                                }
                                client.LastActive = cli.LastActive;
                                db_cli.Entry(client).State = EntityState.Modified;
                            }
                            db_cli.SaveChanges();
                        }
                        sync_client = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when saving client status.", ex);
                }
            }
            #endregion

            #region 每1小时存储数据
            if (curHour != DateTime.Now.Hour)
            {
                try
                {
                    DataTable dt = new DataTable("sensor_dat");
                    dt.Columns.AddRange(new DataColumn[]{
                            new DataColumn("id",typeof(Guid)),
                            new DataColumn("time"),
                            new DataColumn("min"),
                            new DataColumn("avg"),
                            new DataColumn("max"),
                            new DataColumn("val")
                        });
                    foreach (Sensor sen in DbProvider.Sensors.Values)
                    {
                        object[] objs = sen.Stats;
                        if (objs != null)
                        {
                            dt.Rows.Add(objs);
                        }
                    }
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                    {
                        Statistic.LogDbRequest("data");
                        Statistic.Historian.Start((UInt64)dt.Rows.Count);
                        using (TransactionScope scope = new TransactionScope())
                        {
                            conn.Open();
                            using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                            {
                                bulk.DestinationTableName = dt.TableName;
                                bulk.BatchSize = 500;
                                bulk.BulkCopyTimeout = 600;
                                for (int i = 0; i < dt.Columns.Count; i++)
                                {
                                    bulk.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                                }
                                bulk.WriteToServer(dt);
                                scope.Complete();
                            }
                            conn.Close();
                        }
                        Statistic.Historian.Stop();
                    }
                    curHour = DateTime.Now.Hour;
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when saving data into db.", ex);
                }
            }
            #endregion

            jobTimer.Start();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                Exception err = Server.GetLastError();//.GetBaseException();
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("事件类型: {0}\r\n", err.GetType().FullName);
                msg.AppendFormat("请求Url: {0}\r\n", Request.Url);
                msg.AppendFormat("客户端IP: {0}\r\n", Request.UserHostAddress);
                msg.AppendFormat("事件时间: {0}\r\n", DateTime.Now);
                msg.AppendFormat("事件类型: {0}\r\n", err.GetType());
                if (err.GetType() == typeof(HttpException))
                {
                    msg.AppendFormat("错误代码: {0}\r\n", ((HttpException)err).ErrorCode);
                    msg.AppendFormat("HTTP编码: {0}\r\n", ((HttpException)err).GetHttpCode());
                    msg.AppendFormat("事件代码: {0}\r\n", ((HttpException)err).WebEventCode);
                }
                msg.AppendFormat("事件消息: {0}\r\n", err.Message);
                msg.AppendFormat("事件堆栈: {0}\r\n", err.StackTrace);
                Logger.LogError(msg.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when processing application error.", ex);
            }
        }

        protected void Application_End()
        {
            if (jobTimer != null)
            {
                jobTimer.Stop();
            }
            Logger.LogMessage("ODCenter stop running...");
        }
    }

    class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(Guid);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((Guid)value).ToString("N"));
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            return serializer.Deserialize<Guid>(reader);
        }
    }

    public class RoleProvider : System.Web.Security.RoleProvider
    {
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new UserDbContext()));
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new UserDbContext()));
        private String _appname = "ODCenter";

        public override string ApplicationName
        {
            get
            {
                return _appname;
            }
            set
            {
                this._appname = value;
            }
        }

        public override bool RoleExists(string roleName)
        {
            return roleManager.FindByName(roleName) != null;
        }

        public override void CreateRole(string roleName)
        {
            if (!RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            IdentityRole role = roleManager.FindByName(roleName);
            if (role != null)
            {
                roleManager.Delete(role);
                return true;
            }
            return false;
        }
        public override string[] GetAllRoles()
        {
            List<String> roles = new List<string>();
            foreach (IdentityRole role in roleManager.Roles)
            {
                roles.Add(role.Name);
            }
            return roles.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            ApplicationUser user = userManager.FindByName(username);
            if (user != null)
            {
                return userManager.IsInRole(user.Id, roleName);
            }
            return false;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            List<String> users = new List<string>();
            IdentityRole role = roleManager.FindByName(roleName);
            if (role != null)
            {
                foreach (IdentityUserRole usr in role.Users)
                {
                    ApplicationUser user = userManager.FindById(usr.UserId);
                    if (user != null)
                    {
                        users.Add(user.UserName);
                    }
                }
            }
            return users.ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            List<String> users = new List<string>();
            IdentityRole role = roleManager.FindByName(roleName);
            if (role != null)
            {
                foreach (IdentityUserRole usr in role.Users)
                {
                    ApplicationUser user = userManager.FindById(usr.UserId);
                    if (user != null && user.UserName.Contains(usernameToMatch))
                    {
                        users.Add(user.UserName);
                    }
                }
            }
            return users.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            List<String> roles = new List<string>();
            try
            {
                ApplicationUser user = userManager.FindByName(username);
                if (user != null)
                {
                    foreach (String role in UserRoles.All())
                    {
                        if (userManager.IsInRole(user.Id, role))
                        {
                            roles.Add(role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when access user roles.", ex);
            }
            return roles.ToArray();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            //foreach (string role in roleNames)
            //{
            //    CreateRole(role);
            //}
            foreach (string username in usernames)
            {
                ApplicationUser user = userManager.FindByName(username);
                if (user != null)
                {
                    foreach (string role in roleNames)
                    {
                        if (!userManager.IsInRole(user.Id, role))
                        {
                            userManager.AddToRole(user.Id, role);
                        }
                    }
                }
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (string username in usernames)
            {
                ApplicationUser user = userManager.FindByName(username);
                if (user != null)
                {
                    foreach (string role in roleNames)
                    {
                        userManager.RemoveFromRole(user.Id, role);
                    }
                }
            }
        }
    }

    public class RemoveHeaderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            if (!HttpRuntime.UsingIntegratedPipeline) return;
            context.PreSendRequestHeaders += (sender, e) =>
            {
                var app = sender as HttpApplication;
                if (app != null && app.Context != null)
                {
                    app.Response.Headers.Remove("Server");
                    Statistic.LogWebRequest(app.Request.AppRelativeCurrentExecutionFilePath.ToLower().TrimStart(new[] { '~', '/' }));
                }
            };
        }

        public void Dispose()
        {

        }
    }
}
