using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using ODCenter.Models;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ODCenter
{
    // 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<UserDbContext>()));
            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // 配置密码的验证逻辑
            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 6,
            //    RequireNonLetterOrDigit = true,
            //    RequireDigit = true,
            //    RequireLowercase = true,
            //    RequireUppercase = true,
            //};
            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLetter = true,
                RequireLowercase = false,
                RequireUppercase = false
            };
            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并在此处插入。
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "安全代码",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, String> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<UserDbContext>()));
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class PasswordValidator : IIdentityValidator<string>
    {
        public bool RequireDigit { get; set; }

        public int RequiredLength { get; set; }

        public bool RequireNonLetterOrDigit { get; set; }

        public bool RequireLetter { get; set; }

        public bool RequireLowercase { get; set; }

        public bool RequireUppercase { get; set; }

        public virtual bool IsDigit(char c)
        {
            return ((c >= '0') && (c <= '9'));
        }

        public virtual bool IsLetterOrDigit(char c)
        {
            if (!this.IsUpper(c) && !this.IsLower(c))
            {
                return this.IsDigit(c);
            }
            return true;
        }

        public virtual bool IsLetter(char c)
        {
            return IsLower(c) || IsUpper(c);
        }

        public virtual bool IsLower(char c)
        {
            return ((c >= 'a') && (c <= 'z'));
        }

        public virtual bool IsUpper(char c)
        {
            return ((c >= 'A') && (c <= 'Z'));
        }

        public virtual Task<IdentityResult> ValidateAsync(string item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            List<string> values = new List<string>();
            if (string.IsNullOrWhiteSpace(item) || (item.Length < this.RequiredLength))
            {
                values.Add(string.Format("密码必须至少为 {0} 个字符。", this.RequiredLength));
            }
            if (this.RequireNonLetterOrDigit && item.All<char>(new Func<char, bool>(this.IsLetterOrDigit)))
            {
                values.Add("密码必须至少包含一个非字母或数字字符。");
            }
            if (this.RequireDigit && item.All<char>(c => !this.IsDigit(c)))
            {
                values.Add("密码必须至少包含一个数字(\"0\"-\"9\")。");
            }
            if (this.RequireLowercase && item.All<char>(c => !this.IsLower(c)))
            {
                values.Add("密码必须至少包含一个小写字母(\"a\"-\"z\")。");
            }
            if (this.RequireUppercase && item.All<char>(c => !this.IsUpper(c)))
            {
                values.Add("密码必须至少包含一个大写字母(\"A\"-\"Z\")。");
            }
            if (this.RequireLetter && item.All<char>(c => !this.IsLetter(c)))
            {
                values.Add("密码必须至少包含一个大写字母(\"A\"-\"Z\")或小写字母(\"a\"-\"z\")。");
            }
            if (values.Count == 0)
            {
                return Task.FromResult<IdentityResult>(IdentityResult.Success);
            }
            return Task.FromResult<IdentityResult>(IdentityResult.Failed(new string[] { string.Join(" ", values) }));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["smtp:host"]);
                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtp:user"],
                    ConfigurationManager.AppSettings["smtp:pass"]);
                smtp.SendCompleted += SendComplete;
                smtp.SendAsync(new MailMessage(new MailAddress(ConfigurationManager.AppSettings["smtp:user"],
                    ConfigurationManager.AppSettings["smtp:name"]), new MailAddress(message.Destination))
                {
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = true
                }, message.Destination);
            }
            catch (Exception ex)
            {
                Logger.LogError("Fail to send email to " + message.Destination, ex);
            }
            return Task.FromResult(0);
        }

        private void SendComplete(Object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            String addr = (String)e.UserState;
            if (e.Cancelled) { }
            if (e.Error != null)
            {
                Logger.LogError("Fail to send email to " + addr, e.Error);
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 在此处插入短信服务可发送短信。
            return Task.FromResult(0);
        }
    }
}
