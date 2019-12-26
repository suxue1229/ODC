using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Security;

namespace ODCenter.Controllers.Api
{
    [Authorize, Route("Api/Institute/{id?}")]
    public class InstituteController : ApiController
    {
        // GET: api/Institute
        public IHttpActionResult Get()
        {
            return this.Succeed((from id in Account.GetInstitutes(Thread.CurrentPrincipal.Identity.Name, true)
                                 let institute = DbProvider.Institutes[id.ToString("N")]
                                 where DbProvider.Institutes.ContainsKey(id.ToString("N"))
                                 orderby institute.Name
                                 select new
                                 {
                                     Id = institute.Id,
                                     Name = institute.Name,
                                     Type = institute.Type,
                                     Location = institute.Location,
                                     Longitude = institute.Longitude,
                                     Latitude = institute.Latitude,
                                     Address = institute.Address,
                                     Summary = institute.Summary,
                                 }));
        }

        // GET: api/Institute/{id}
        public IHttpActionResult Get(Guid id)
        {
            if (Roles.IsUserInRole(UserRoles.Admin) ||
                Account.GetInstitutes(Thread.CurrentPrincipal.Identity.Name, true).Contains(id))
            {
                var institute = DbProvider.Institutes.Find(id);
                if (institute != null)
                {
                    return this.Succeed(new
                    {
                        Id = institute.Id,
                        Name = institute.Name,
                        Type = institute.Type,
                        Location = institute.Location,
                        Longitude = institute.Longitude,
                        Latitude = institute.Latitude,
                        Address = institute.Address,
                        Summary = institute.Summary,
                    });
                }
            }
            return this.Failed("institute_not_found", ApiStatusCode.NotFound);
        }

        // POST: api/Institute/{id}
        [Authorize(Roles = UserRoles.Admin)]
        public IHttpActionResult Post(Guid id, [FromBody]InstituteInfo info)
        {
            if (ModelState.IsValid)
            {
                info.Id = id;
                DbProvider.Institutes.Update(info, User.Identity.Name);
                return Get(id);
            }
            return this.Failed("institute_not_found", ApiStatusCode.NotFound);
        }

        // PUT: api/Institute
        [Authorize(Roles = UserRoles.Admin)]
        public IHttpActionResult Put([FromBody]InstituteInfo info)
        {
            if (ModelState.IsValid)
            {
                Guid institute_id = DbProvider.Institutes.Create(info, User.Identity.Name);
                return Get(institute_id);
            }
            return this.Failed("data_invalid", ApiStatusCode.DataInvalid);
        }

        // DELETE: api/Institute/{id}
        [Authorize(Roles = UserRoles.Admin)]
        public IHttpActionResult Delete(Guid id)
        {
            var institute = DbProvider.Institutes.Delete(id, User.Identity.Name);
            if (institute != null)
            {
                return this.Succeed("institute_deleted", new
                {
                    Id = institute.Id,
                    Name = institute.Name,
                    Type = institute.Type,
                    Location = institute.Location,
                    Longitude = institute.Longitude,
                    Latitude = institute.Latitude,
                    Address = institute.Address,
                    Summary = institute.Summary,
                });
            }
            return this.Failed("institute_not_found", ApiStatusCode.NotFound);
        }
    }
}
