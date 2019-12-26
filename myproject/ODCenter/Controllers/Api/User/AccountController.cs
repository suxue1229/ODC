using Microsoft.AspNet.Identity;
using ODCenter.Base;
using ODCenter.Models;
using PTR.Web.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace ODCenter.Controllers.Api
{
#if !DEBUG
    [RequireSsl]
#endif
    [Authorize, Route("Api/User/Account/")]
    public class AccountController : ApiController
    {
        // GET: api/Account
        public IHttpActionResult Get()
        {
            var user = Account.GetUserByEmail(Thread.CurrentPrincipal.Identity.Name);
            if (user != null)
            {
                return this.Succeed(new
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NickName = user.NickName,
                    RegisterDate = user.Register.HasValue ? user.Register.Value.ToString() : null,
                    LastLoginDate = user.LastLogin.HasValue ? user.LastLogin.Value.ToString() : null,
                    Company = user.Company,
                    Department = user.Department,
                    Authorizes = (from id in Account.GetInstitutes(user.UserName, true)
                                  let institute = DbProvider.Institutes[id.ToString("N")]
                                  where DbProvider.Institutes.ContainsKey(id.ToString("N"))
                                  orderby institute.Name
                                  select new
                                  {
                                      Id = institute.Id.ToString("N"),
                                      Name = institute.Name
                                  }),
                });
            }
            return this.Failed("user_not_found", ApiStatusCode.NotFound);
        }

        //GET: api/Account/{email}
        [Authorize(Roles = UserRoles.Admin)]
        public IHttpActionResult Get(String email)
        {
            var user = Account.GetUserByEmail(email);
            if (user != null)
            {
                return this.Succeed(new
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Confirmed = user.EmailConfirmed,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NickName = user.NickName,
                    RegisterDate = user.Register.HasValue ? user.Register.Value.ToString() : null,
                    LastLoginDate = user.LastLogin.HasValue ? user.LastLogin.Value.ToString() : null,
                    Company = user.Company,
                    Department = user.Department,
                    Institute = user.Institute.HasValue ? user.Institute.Value.ToString("N") : null,
                    Institutes = (from insid in Account.GetInstitutes(user.UserName)
                                  let institute = DbProvider.Institutes[insid.ToString("N")]
                                  where DbProvider.Institutes.ContainsKey(insid.ToString("N"))
                                  orderby institute.Name
                                  select new
                                  {
                                      Id = institute.Id.ToString("N"),
                                      Name = institute.Name
                                  }),
                    Authorizes = (from insid in Account.GetInstitutes(user.UserName, true)
                                  let institute = DbProvider.Institutes[insid.ToString("N")]
                                  where DbProvider.Institutes.ContainsKey(insid.ToString("N"))
                                  orderby institute.Name
                                  select new
                                  {
                                      Id = institute.Id.ToString("N"),
                                      Name = institute.Name
                                  }),
                    Roles = Roles.GetRolesForUser(user.UserName)
                });
            }
            return this.Failed("user_not_found", ApiStatusCode.NotFound);
        }

        // POST: api/Account
        public IHttpActionResult Post([FromBody]ManageUserInfoModel model)
        {
            return Post(Thread.CurrentPrincipal.Identity.Name, model);
        }

        //POST: api/Account/{email}
        [Authorize(Roles = UserRoles.Admin)]
        public IHttpActionResult Post(String email, [FromBody]ManageUserInfoModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = Account.Update(email, model);
                if (result.Succeeded)
                {
                    return this.Succeed("账户信息已更新");
                }
                else
                {
                    return this.Failed(result.Errors);
                }
            }
            return this.Failed(ModelState.GetErrors(), ApiStatusCode.NotFound);
        }

        // PUT: api/Account
        [AllowAnonymous]
        public async Task<IHttpActionResult> Put([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Account.Register(model);
                if (result.Succeeded)
                {
                    return this.Succeed("注册成功，请查收邮件以激活您的帐户。");
                }
                else
                {
                    return this.Failed(result.Errors);
                }
            }
            return this.Failed(ModelState.GetErrors(), ApiStatusCode.DataInvalid);
        }

        // DELETE: api/Account/{email}
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IHttpActionResult> Delete(String email)
        {
            var manager = Account.UserManager;
            var user = await manager.FindByEmailAsync(email);
            if (user != null)
            {
                IdentityResult result = await manager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return this.Succeed(String.Format("已删除账户{0}", email));
                }
                return this.Failed(result.Errors);
            }
            return this.Failed("user_not_found", ApiStatusCode.NotFound);
        }
    }
}
