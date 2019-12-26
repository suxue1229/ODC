using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using ODCenter.Base;
using ODCenter.Models;
using Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace ODCenter
{
    public partial class Startup
    {
        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // 将数据库上下文和用户管理器配置为对每个请求使用单个实例
            app.CreatePerOwinContext(UserDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // 使应用程序可以使用 Cookie 来存储已登录用户的信息
            // 并使用 Cookie 来临时存储有关使用第三方登录提供程序登录的用户的信息
            // 配置登录 Cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),
                    OnApplyRedirect = ctx =>
                    {
                        if (!ctx.Request.Path.StartsWithSegments(new PathString("/api")))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                },
                CookieName = "access_token"
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // 取消注释以下行可允许使用第三方登录提供程序登录
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/api/user/authorize"),
                Provider = new OAuthAuthorizationServerProvider()
                {
                    OnValidateClientAuthentication = async context =>
                    {
                        await Task.Run(() =>
                        {
                            context.Validated();
                        });
                    },
                    OnGrantResourceOwnerCredentials = async context =>
                    {
                        if (String.IsNullOrWhiteSpace(context.UserName) || String.IsNullOrWhiteSpace(context.Password))
                        {
                            context.Rejected();
                        }
                        else
                        {
                            var usr_manager = Account.UserManager;
                            var usr = await Account.Login(context.UserName, context.Password, context.Request.RemoteIpAddress);
                            if (usr != null)
                            {
                                if (!usr_manager.IsEmailConfirmed(usr.Id))
                                {
                                    context.SetError("账户未激活");
                                }
                                else
                                {
                                    var identity = await usr.GenerateUserIdentityAsync(usr_manager, context.Options.AuthenticationType);
                                    var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
                                    context.Validated(ticket);
                                }
                            }
                            else
                            {
                                context.SetError("用户名密码无效或访问受限");
                            }
                        }
                    },
                    OnGrantRefreshToken = async context =>
                    {
                        await Task.Run(() =>
                        {
                            if (context.Ticket == null || context.Ticket.Identity == null || !context.Ticket.Identity.IsAuthenticated)
                            {
                                context.SetError("invalid_grant");
                            }
                            else
                            {
                                context.Validated();
                            }
                        });
                    }
                },
                RefreshTokenProvider = new AuthenticationTokenProvider()
                {
                    OnCreate = context =>
                    {
                        var token = new UserToken()
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            Subject = context.Ticket.Identity.Name,
                            IssuedUtc = DateTime.UtcNow,
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                        };
                        context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                        context.Ticket.Properties.ExpiresUtc = token.IssuedUtc.AddDays(7);
                        token.Ticket = context.SerializeTicket();
                        Account.AddToken(token);
                        context.SetToken(token.Id);
                    },
                    OnCreateAsync = async context =>
                    {
                        var token = new UserToken()
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            Subject = context.Ticket.Identity.Name,
                            IssuedUtc = DateTime.UtcNow,
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                        };
                        context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                        context.Ticket.Properties.ExpiresUtc = token.IssuedUtc.AddDays(7);
                        token.Ticket = context.SerializeTicket();
                        await Account.AddTokenAsync(token);
                        context.SetToken(token.Id);
                    },
                    OnReceive = context =>
                    {
                        var token = Account.RemoveToken(context.Token);
                        if (token != null)
                        {
                            context.DeserializeTicket(token.Ticket);
                        }
                    },
                    OnReceiveAsync = async context =>
                    {
                        var token = await Account.RemoveTokenAsync(context.Token);
                        if (token != null)
                        {
                            context.DeserializeTicket(token.Ticket);
                        }
                    }
                },
                AuthenticationMode = AuthenticationMode.Active,
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
#if DEBUG
                AllowInsecureHttp = true,
#endif
            });
        }
    }
}