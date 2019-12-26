using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ODCenter.Models;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ODCenter.Base
{
    public class Account
    {
        public static ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public static ApplicationUser GetUserByName(String username)
        {
            using (var db_usr = new UserDbContext())
            {
                var user = db_usr.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    return user;
                }
            }
            return null;
        }

        public static ApplicationUser GetUserByEmail(String email)
        {
            using (var db_usr = new UserDbContext())
            {
                var user = db_usr.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    return user;
                }
            }
            return null;
        }

        public static async Task<IdentityResult> Register(RegisterViewModel model)
        {
            if (!Const.RegisterEnabled)
            {
                return new IdentityResult(new[] { "账号注册功能已被管理员禁止。" });
            }
            var db_user = new UserDbContext();
            var invite = db_user.Invites.FirstOrDefault(i => i.Id == model.InviteCode && i.Account == null);
            if (Const.InviteRequired)
            {
                if (String.IsNullOrWhiteSpace(model.InviteCode))
                {
                    return new IdentityResult(new[] { "请输入授权码。" });
                }
                if (invite == null)
                {
                    return new IdentityResult(new[] { "授权码无效。" });
                }
            }
            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                NickName = model.NickName,
                Company = model.Company,
                Department = model.Department,
                Register = DateTime.Now
            };
            List<Guid> inslist = new List<Guid>();
            foreach (String id in ConfigurationManager.AppSettings["institute:default"].Split(new[] { ',', ';' }))
            {
                Guid insid = Guid.Empty;
                if (Guid.TryParse(id, out insid) && DbProvider.Institutes.ContainsKey(insid.ToString("N")))
                {
                    inslist.Add(insid);
                }
            }
            if (inslist.Count > 0)
            {
                user.Institute = inslist[0];
                user.Institutes = inslist.ToArray();
            }
            var usr_manager = UserManager;
            IdentityResult result = await usr_manager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (Const.InviteRequired && invite != null)
                {
                    invite.Used = DateTime.Now;
                    invite.Account = user.Email;
                    db_user.Entry(invite).State = EntityState.Modified;
                    db_user.SaveChanges();
                }
                String code = await usr_manager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Confirm", "Account",
                    new { userId = user.Id, code = code },
                    protocol: HttpContext.Current.Request.Url.Scheme);
                StringBuilder body = new StringBuilder();
                body.Append("尊敬的用户：<br>");
                body.AppendFormat("<p style='text-indent: 2em'>感谢你于{0}在{1}进行账户注册。</p>",
                    DateTime.Now.ToString("yyyy年MM月dd日HH时mm分"), HttpContext.Current.Request.Url.Host);
                body.AppendFormat("<p style='text-indent: 2em'>如非本人操作，请忽略并删除此邮件，否则请通过<a href='{0}'>单击此处</a>来激活你的账户。</p>", callbackUrl);
                body.AppendFormat("<p style='text-indent: 2em'>如果你的浏览器无法正常跳转，请复制此链接<b>{0}</b>，并在浏览器中打开。</p>", callbackUrl);
                usr_manager.SendEmail(user.Id, "确认你的帐户", body.ToString());
            }
            return result;
        }

        public static async Task<Boolean> Active(String email)
        {
            var usr_manager = UserManager;
            var user = usr_manager.FindByEmail(email);
            if (user != null && !usr_manager.IsEmailConfirmed(user.Id))
            {
                string code = usr_manager.GenerateEmailConfirmationToken(user.Id);
                var callbackUrl = new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Confirm", "Account",
                    new { userId = user.Id, code = code },
                    protocol: HttpContext.Current.Request.Url.Scheme);
                StringBuilder body = new StringBuilder();
                body.Append("尊敬的用户：<br>");
                body.AppendFormat("<p style='text-indent: 2em'>你于{0}在{1}上申请了重新账户激活功能。</p>", DateTime.Now.ToString("yyyy年MM月dd日HH时mm分"), HttpContext.Current.Request.Url.Host);
                body.AppendFormat("<p style='text-indent: 2em'>如非本人操作，请忽略并删除此邮件，否则请通过<a href='{0}'>单击此处</a>来激活你的账户。</p>", callbackUrl);
                body.AppendFormat("<p style='text-indent: 2em'>如果你的浏览器无法正常跳转，请复制此链接<b>{0}</b>，并在浏览器中打开。</p>", callbackUrl);
                await usr_manager.SendEmailAsync(user.Id, "激活你的帐户", body.ToString());
                return true;
            }
            return false;
        }

        public static async Task<ApplicationUser> Login(String email, String password, String client)
        {
            var user_manager = UserManager;
            using (var db_user = new UserDbContext())
            {
                var user = await user_manager.FindAsync(email, password);
                if (user != null)
                {
                    if (!user_manager.IsEmailConfirmed(user.Id))
                    {
                        return user;
                    }
                    var acc = db_user.Users.FirstOrDefault(u => u.Email == email);
                    if (acc != null)
                    {
                        if (!acc.TotalFail.HasValue)
                        {
                            acc.TotalFail = 0;
                        }
                        if (acc.LastFail.HasValue && acc.LastFail.Value.Date != DateTime.Now.Date)
                        {
                            acc.TotalFail = 0;
                        }
                        Int32 limit = 0;
                        if (Int32.TryParse(ConfigurationManager.AppSettings["account:maxfail"], out limit) && acc.TotalFail >= limit)
                        {
                            user = null;
                            acc.LastFail = DateTime.Now;
                            acc.TotalFail++;
                            db_user.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                            db_user.Access.Add(new UserAccess()
                            {
                                Id = acc.Id,
                                Remote = client,
                                Time = DateTime.Now,
                                Grant = false,
                                Remark = "exceed limit"
                            });
                        }
                        else
                        {
                            acc.LastLogin = DateTime.Now;
                            db_user.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                            db_user.Access.Add(new UserAccess()
                            {
                                Id = acc.Id,
                                Remote = client,
                                Time = DateTime.Now,
                                Grant = true,
                                Remark = "normal"
                            });
                        }
                        await db_user.SaveChangesAsync();
                    }
                    return user;
                }
                else
                {
                    var acc = db_user.Users.FirstOrDefault(u => u.Email == email);
                    if (acc != null)
                    {
                        if (!acc.TotalFail.HasValue)
                        {
                            acc.TotalFail = 0;
                        }
                        if (acc.LastFail.HasValue && acc.LastFail.Value.Date != DateTime.Now.Date)
                        {
                            acc.TotalFail = 1;
                        }
                        else
                        {
                            acc.TotalFail++;
                        }
                        acc.LastFail = DateTime.Now;
                        db_user.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                        db_user.Access.Add(new UserAccess()
                        {
                            Id = acc.Id,
                            Remote = client,
                            Time = DateTime.Now,
                            Grant = false,
                            Remark = "invalid password"
                        });
                        await db_user.SaveChangesAsync();
                    }
                    Console.WriteLine("user is null");
                    return null;
                }
            }
        }

        public static IdentityResult Update(String username, ManageUserInfoModel model)
        {
            if (!Const.AccountEdit)
            {
                return new IdentityResult(new[] { "账户编辑功能已被管理员禁止。" });
            }
            using (var db_user = new UserDbContext())
            {
                var user = db_user.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return new IdentityResult(new[] { "账户或账户信息无效。" });
                }
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.NickName = model.NickName;
                user.Company = model.Company;
                user.Department = model.Department;
                db_user.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db_user.SaveChanges();
            }
            return IdentityResult.Success;
        }

        public static async Task<Boolean> Forget(String email)
        {
            var usr_manager = UserManager;
            var user = await usr_manager.FindByNameAsync(email);
            if (user == null || !(await usr_manager.IsEmailConfirmedAsync(user.Id)))
            {
                return false;
            }
            string code = await usr_manager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Reset", "Account",
                new { userId = user.Id, code = code },
                protocol: HttpContext.Current.Request.Url.Scheme);
            StringBuilder body = new StringBuilder();
            body.Append("尊敬的用户：<br>");
            body.AppendFormat("<p style='text-indent: 2em'>你于{0}在{1}上申请了密码重置功能。</p>",
                DateTime.Now.ToString("yyyy年MM月dd日HH时mm分"), HttpContext.Current.Request.Url.Host);
            body.AppendFormat("<p style='text-indent: 2em'>如非本人操作，请忽略并删除此邮件，否则请通过<a href='{0}'>单击此处</a>来重置你的密码。</p>", callbackUrl);
            body.AppendFormat("<p style='text-indent: 2em'>如果你的浏览器无法正常跳转，请复制此链接<b>{0}</b>，并在浏览器中打开。</p>", callbackUrl);
            await usr_manager.SendEmailAsync(user.Id, "重置密码", body.ToString());
            return true;
        }

        public static Boolean AddToken(UserToken token)
        {
            using (var db_user = new UserDbContext())
            {
                var old = db_user.Tokens.FirstOrDefault(t => t.Id == token.Id);
                if (old != null)
                {
                    db_user.Tokens.Remove(token);
                }
                db_user.Tokens.Add(token);
                return db_user.SaveChanges() > 0;
            }
        }

        public static async Task<Boolean> AddTokenAsync(UserToken token)
        {
            using (var db_user = new UserDbContext())
            {
                var old = db_user.Tokens.FirstOrDefault(t => t.Id == token.Id);
                if (old != null)
                {
                    db_user.Tokens.Remove(token);
                }
                db_user.Tokens.Add(token);
                return await db_user.SaveChangesAsync() > 0;
            }
        }

        public static UserToken RemoveToken(String id)
        {
            using (var db_user = new UserDbContext())
            {
                UserToken token = db_user.Tokens.Find(id);
                if (token != null)
                {
                    db_user.Tokens.Remove(token);
                    db_user.SaveChanges();
                    return token;
                }
                return null;
            }
        }

        public static async Task<UserToken> RemoveTokenAsync(String id)
        {
            using (var db_user = new UserDbContext())
            {
                UserToken token = await db_user.Tokens.FindAsync(id);
                if (token != null)
                {
                    db_user.Tokens.Remove(token);
                    await db_user.SaveChangesAsync();
                    return token;
                }
                return null;
            }
        }

        public static Guid? GetInstitute()
        {
            try
            {
                Claim claim = ((ClaimsIdentity)HttpContext.Current.User.Identity).FindFirst(Const.Claim_Institute_Id);
                Guid insid = Guid.Empty;
                if (claim != null && Guid.TryParse(claim.Value, out insid))
                {
                    return insid;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when get institute for user.", ex);
            }
            return null;
        }

        public static Guid[] GetInstitutes(Boolean extend = false)
        {
            if (HttpContext.Current != null)
            {
                return GetInstitutes(HttpContext.Current.User.Identity.Name, extend);
            }
            return new Guid[0];
        }

        public static Guid[] GetInstitutes(String usrname, Boolean extend = false)
        {
            if (extend)
            {
                List<Guid> lst = new List<Guid>();
                if (Roles.IsUserInRole(usrname, UserRoles.Admin) || Roles.IsUserInRole(usrname, UserRoles.Global))
                {
                    foreach (Institute ind in DbProvider.Institutes.Values)
                    {
                        if (!lst.Contains(ind.Id))
                        {
                            lst.Add(ind.Id);
                        }
                    }
                    if (!Roles.IsUserInRole(usrname, UserRoles.Admin) && lst.Count > 1)
                    {
                        foreach (Guid id in Institute.Excludes)
                        {
                            if (lst.Contains(id))
                            {
                                lst.Remove(id);
                            }
                        }
                    }
                }
                else
                {
                    ApplicationUser user = Account.GetUserByName(usrname);
                    if (user.Institute.HasValue)
                    {
                        lst.Add(user.Institute.Value);
                    }
                    foreach (Guid id in user.Institutes)
                    {
                        if (!lst.Contains(id))
                        {
                            lst.Add(id);
                        }
                    }
                }
                return lst.ToArray();
            }
            else
            {
                using (var db_usr = new UserDbContext())
                {
                    return (from ins in db_usr.Conns.Where(x => x.UserName == usrname) select ins.Institute).ToArray();
                }
            }
        }

        public static void SetInstitutes(String usrname, Guid[] ins)
        {
            using (var db_usr = new UserDbContext())
            {
                List<Guid> list = new List<Guid>(ins);
                foreach (Guid id in list.ToArray())
                {
                    if (!DbProvider.Institutes.ContainsKey(id.ToString("N")))
                    {
                        list.Remove(id);
                    }
                }
                foreach (UserConn con in db_usr.Conns.Where(x => x.UserName == usrname))
                {
                    if (list.Contains(con.Institute))
                    {
                        list.Remove(con.Institute);
                    }
                    else
                    {
                        db_usr.Entry(con).State = System.Data.Entity.EntityState.Deleted;
                    }
                }
                foreach (Guid id in list)
                {
                    db_usr.Conns.Add(new UserConn(usrname, id));
                }
                db_usr.SaveChanges();
            }
        }

        public static Boolean ConnUpdate(String user, List<Guid> lst)
        {
            using (var db_usr = new UserDbContext())
            {
                foreach (UserConn con in db_usr.Conns.Where(x => x.UserName == user))
                {
                    if (lst.Contains(con.Institute))
                    {
                        lst.Remove(con.Institute);
                    }
                    else
                    {
                        db_usr.Conns.Remove(con);
                    }
                }
                foreach (Guid ins in lst)
                {
                    db_usr.Conns.Add(new UserConn(user, ins));
                }
                db_usr.SaveChanges();
            }
            return true;
        }

        public static Boolean ConnAdd(String user, Guid ins)
        {
            using (var db_usr = new UserDbContext())
            {
                if (db_usr.Conns.Find(user, ins) == null)
                {
                    db_usr.Conns.Add(new UserConn(user, ins));
                    db_usr.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public static Boolean ConnRemove(String user, Guid ins)
        {
            using (var db_usr = new UserDbContext())
            {
                UserConn con = db_usr.Conns.Find(user, ins);
                if (con != null)
                {
                    db_usr.Conns.Remove(con);
                    db_usr.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }

    public class UserRoles
    {
        public const String Admin = "administrator", Engineer = "engineer", Operator = "operator", Guest = "guest", Global = "global";
        public const String EngineerAbove = Admin + "," + Engineer;
        public const String OperatorAbove = EngineerAbove + "," + Operator;
        public const String GuestAbove = OperatorAbove + "," + Guest;

        public static String[] All()
        {
            return new String[]{
                Admin, Engineer, Operator, Guest, Global
            };
        }
    }
}