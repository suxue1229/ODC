using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ODCenter.Controllers
{
    [Authorize, RequireHttps]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Assert(LoginViewModel model)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (ModelState.IsValid)
            {
                    var user = await Account.Login(model.Email, model.Password, Request.UserHostAddress);
                    if (user != null)
                    {
                        json.Data = new { State = "Success", Errors = "" };
                        return json;
                    }
                    else
                    {
                        ModelState.AddModelError("", "用户名密码无效或访问受限。");
                    }
          
            }
            StringBuilder builder = new StringBuilder();
            foreach (ModelState state in ModelState.Values)
            {
                foreach (ModelError error in state.Errors)
                {
                    builder.AppendLine(error.ErrorMessage);
                }
            }
            json.Data = new
            {
                State = "Failed",
                Errors = builder.ToString()
            };
            return json;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await Account.Login(model.Email, model.Password, Request.UserHostAddress);
                if (user != null)
                {
                    if (!UserManager.IsEmailConfirmed(user.Id))
                    {
                        return View("Active", user);
                    }
                    else
                    {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "用户名密码无效或访问受限。");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Active([Bind(Include = "Email")]String Email)
        {
            if (await Account.Active(Email))
            {
                ViewBag.Title = "激活链接已发送";
                ViewData["body"] = String.Format("账户激活链接已发送。请<a href='{0}'>单击此处</a>查阅你的电子邮件并激活账户。", MailDomain(Email));
                return View("Result");
            }
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await Account.Register(model);
                if (result.Succeeded)
                {
                    //await SignInAsync(user, isPersistent: false);

                    ViewBag.Title = "注册成功";
                    ViewData["body"] = String.Format("感谢你的注册。请<a href='{0}' style='font-weight:bold'>单击此处</a>进入邮箱来确认你的账户，或<a href='{1}' style='font-weight:bold'>单击此处</a>返回首页。", MailDomain(model.Email), Url.Action("Index", "Home"));
                    return View("Result");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public String MailDomain(String email)
        {
            String[] m = email.Split('@');
            if (m.Length == 2)
            {
                return "https://mail." + m[1];
            }
            return "https://" + email;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Confirm(string userId, string code)
        {
            if (userId == null || code == null)
            {
                ViewBag.Title = "账户激活失败";
                ViewData["body"] = String.Format("无法激活你的帐户。请确认激活链接是否有效。");
                return View("Result");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                ViewBag.Title = "账户已激活";
                ViewData["body"] = String.Format("感谢你确认并激活帐户。请 <a href='{0}'>{1}</a>。", Url.Action("Login", "Account"), "单击此处登录");
            }
            else
            {
                ViewBag.Title = "账户激活失败";
                StringBuilder builder = new StringBuilder();
                builder.Append("无法激活你的帐户。");
                foreach (String err in result.Errors)
                {
                    builder.Append(err);
                }
                ViewData["body"] = builder.ToString();
            }
            return View("Result");
        }

        [AllowAnonymous]
        public ActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Forgot(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await Account.Forget(model.Email))
                {
                    ModelState.AddModelError("", "用户不存在或未确认。");
                    return View();
                }

                ViewBag.Title = "已发送密码重置链接";
                ViewData["body"] = String.Format("请<a href='{0}'>单击此处</a>查阅你的电子邮件以重置密码。", MailDomain(model.Email));
                return View("Result");
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Reset(string code)
        {
            if (code == null)
            {
                return View("Error");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reset(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "找不到用户。");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Title = "重置密码确认";
                    ViewData["body"] = String.Format("你的密码已重置。请 <a href='{0}'>{1}</a>。", Url.Action("Login", "Account"), "单击此处登录");
                    return View("Result");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "你的密码已更改。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录名。"
                : message == ManageMessageId.Error ? "出现错误。"
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // 用户没有密码，因此将删除由于缺少 OldPassword 字段而导致的所有验证错误
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public async Task<ActionResult> Info()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                return View(new ManageUserInfoModel()
                {
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    NickName = user.NickName,
                    Company = user.Company,
                    Department = user.Department
                });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(ManageUserInfoModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = Account.Update(User.Identity.Name, model);
                if (result.Succeeded)
                {
                    ViewBag.StatusMessage = "账户信息已更新";
                    return View(model);
                }
                else
                {
                    AddErrors(result);
                }
            }
            ViewBag.StatusMessage = "账户信息更新失败";
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 如果用户已具有登录名，则使用此外部登录提供程序将该用户登录
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // 如果用户没有帐户，则提示该用户创建帐户
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // 请求重定向到外部登录提供程序，以链接当前用户的登录名
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);

                        // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                        // 发送包含此链接的电子邮件
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // SendEmail(user.Email, callbackUrl, "确认你的帐户", "请单击此链接确认你的帐户");

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing && Account.UserManager != null)
            //{
            //    UserManager.Dispose();
            //    UserManager = null;
            //}
            base.Dispose(disposing);
        }

        #region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent },
                await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // 有关发送邮件的信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #region 角色与机构授权
        public ActionResult Switch()
        {
            Guid? insid = Account.GetInstitute();
            ViewBag.Now = insid.HasValue ? DbProvider.Institutes[insid.Value.ToString("N")].Name : "Unknown";
            ViewBag.List = Account.GetInstitutes(User.Identity.Name, true).GetSelectList(insid.HasValue ? insid.Value : Guid.Empty);
            return View();
        }

        [HttpPost]
        public JsonResult Switch(Guid? institute)
        {
            var ins = Account.GetInstitutes(User.Identity.Name, true).GetSelectList(Guid.Empty);
            if (institute.HasValue && ins.FirstOrDefault(i => Guid.Parse(i.Value) == institute.Value) != null)
            {
                ApplicationUser user = UserManager.FindByName(User.Identity.Name);
                if (user != null)
                {
                    user.Institute = institute.Value;
                    UserManager.Update(user);
                    UserManager.UpdateSecurityStamp(user.Id.ToString());
                }
                ClaimsIdentity claims = (ClaimsIdentity)User.Identity;
                claims.TryRemoveClaim(claims.FindFirst(Const.Claim_Institute_Id));
                claims.AddClaim(new Claim(Const.Claim_Institute_Id, institute.Value.ToString("N")));
                claims.TryRemoveClaim(claims.FindFirst(Const.Claim_Institute_Name));
                claims.AddClaim(new Claim(Const.Claim_Institute_Name, DbProvider.Institutes != null ? DbProvider.Institutes[institute.Value.ToString("N")].Name : "Unknown"));
                AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsIdentity(claims), new AuthenticationProperties());
                return Json(new { result = "ok" });
            }
            return Json(new { result = "failed", message = "机构或权限错误" });
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Edit()
        {
            return View(UserManager.Users);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Group()
        {
            var db_user = new UserDbContext();
            return View(db_user.Groups);

        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Invite()
        {
            var db_user = new UserDbContext();
            return View(db_user.Invites);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Role(Guid? id)
        {
            if (id.HasValue)
            {
                return View(UserManager.FindById(id.Value.ToString()));
            }
            else
            {
                return RedirectToAction("Edit");
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Role([Bind(Include = "Id,Email")] ApplicationUser newuser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser oriuser = UserManager.FindById(newuser.Id.ToString());
                if (oriuser != null && oriuser.Email == newuser.Email)
                {
                    foreach (String role in UserRoles.All())
                    {
                        String val = Request.Form["role_" + role];
                        Boolean rolen = (val != null && (val.Contains("on") || val.Contains("true")));
                        if (rolen && !Roles.IsUserInRole(oriuser.UserName, role))
                        {
                            Roles.AddUserToRole(oriuser.UserName, role);
                        }
                        else if (!rolen && Roles.IsUserInRole(oriuser.UserName, role))
                        {
                            Roles.RemoveUserFromRole(oriuser.UserName, role);
                        }
                    }
                    UserManager.Update(oriuser);
                    UserManager.UpdateSecurityStamp(oriuser.Id.ToString());
                }
            }
            return RedirectToAction("Edit");
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Auth(Guid? id)
        {
            if (id.HasValue)
            {
                return View(UserManager.FindById(id.Value.ToString()));
            }
            else
            {
                return RedirectToAction("Edit");
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Auth([Bind(Include = "Id,Email,Institute")] ApplicationUser newuser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser oriuser = UserManager.FindById(newuser.Id.ToString());
                if (oriuser != null && oriuser.Email == newuser.Email)
                {
                    oriuser.Institute = newuser.Institute;
                    UserManager.Update(oriuser);
                    UserManager.UpdateSecurityStamp(oriuser.Id.ToString());
                }
            }
            return RedirectToAction("Edit");
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Institute(Guid? id)
        {
            if (id.HasValue)
            {
                return View(UserManager.FindById(id.Value.ToString()));
            }
            else
            {
                return RedirectToAction("Edit");
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public ActionResult Institute([Bind(Include = "Id,Email")] ApplicationUser newuser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser oriuser = UserManager.FindById(newuser.Id.ToString());
                if (oriuser != null && oriuser.Email == newuser.Email)
                {
                    List<Guid> inlst = new List<Guid>();
                    foreach (KeyValuePair<String, Institute> ins in DbProvider.Institutes)
                    {
                        String val = Request.Form[ins.Key];
                        if (val != null && (val.Contains("on") || val.Contains("true")))
                        {
                            inlst.Add(ins.Value.Id);
                        }
                    }
                    oriuser.Institutes = inlst.ToArray();
                }
            }
            return RedirectToAction("Edit");
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Clean(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Edit");
            }
            ApplicationUser user = UserManager.FindById(id.Value.ToString());
            if (user == null || user.EmailConfirmed)
            {
                return RedirectToAction("Edit");
            }
            return View(user);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clean([Bind(Include = "Id,Email")] ApplicationUser usr)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(usr.Id);
                if (user != null && user.Email == usr.Email && !user.EmailConfirmed)
                {
                    UserManager.Delete(user);
                }
            }
            return RedirectToAction("Edit");
        }

        #endregion
    }
}