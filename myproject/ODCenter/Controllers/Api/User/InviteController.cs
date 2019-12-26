using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ODCenter.Controllers.Api
{
    [Authorize(Roles = UserRoles.Admin), Route("Api/User/Invite/{id?}")]
    public class InviteController : ApiController
    {
        // GET: api/Invite
        public IHttpActionResult Get()
        {
            var db_user = new UserDbContext();
            return this.Succeed(db_user.Invites.OrderBy(i => i.Issued));
        }

        // GET: api/Invite/5
        public IHttpActionResult Get(String id)
        {
            var db_user = new UserDbContext();
            var invite = db_user.Invites.FirstOrDefault(i => i.Id == id);
            if (invite != null)
            {
                return this.Succeed(invite);
            }
            return this.Failed("invite_not_found", ApiStatusCode.NotFound);
        }

        // POST: api/Invite
        public IHttpActionResult Post(String id)
        {
            var db_user = new UserDbContext();
            var invite = db_user.Invites.FirstOrDefault(i => i.Id == id);
            if (invite != null)
            {
                var val = Request.Param("description");
                if (val != null)
                {
                    invite.Description = val;
                    db_user.Entry(invite).State = EntityState.Modified;
                    db_user.SaveChanges();
                }
                return this.Succeed(invite);
            }
            return this.Failed("invite_not_found", ApiStatusCode.NotFound);
        }

        // PUT: api/Invite/5
        public IHttpActionResult Put()
        {
            var db_user = new UserDbContext();
            var invite = new UserInvite()
            {
                Id = ExtHttp.RandStr(10),
                Issued = DateTime.Now,
                Expired = DateTime.Now.AddDays(7),
                Description = Request.Param("description")
            };
            db_user.Invites.Add(invite);
            db_user.SaveChanges();
            return this.Succeed(invite);
        }

        // DELETE: api/Invite/5
        public IHttpActionResult Delete(String id)
        {
            var db_user = new UserDbContext();
            var invite = db_user.Invites.FirstOrDefault(i => i.Id == id);
            if (invite != null)
            {
                db_user.Invites.Remove(invite);
                db_user.SaveChanges();
                return this.Succeed(String.Format("invite {0} has been removed.", invite.Id));
            }
            return this.Failed("invite_not_found", ApiStatusCode.NotFound);
        }
    }
}
