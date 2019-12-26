using ODCenter.Base;
using System;
using System.Linq;
using System.Web.Http;

namespace ODCenter.Controllers.Api
{
    [Authorize(Roles = UserRoles.Admin), Route("Api/Status/{id?}")]
    public class StatusController : ApiController
    {
        // GET: api/Status
        public IHttpActionResult Get()
        {
            return Get(null);
        }

        // GET: api/Status/{id}
        public IHttpActionResult Get(String id)
        {
            return this.Succeed(new
            {
                Service = new
                {
                    Startup = Statistic.Service.Startup,
                    Duration = Statistic.Service.Duration.ToString(@"dd\.hh\:mm\:ss")
                },
                Requests = new
                {
                    Web = new
                    {
                        Total = (from req in Statistic.WebRequests where !req.Key.StartsWith("api") select req.Value).Sum(),
                        Detail = (from request in Statistic.WebRequests
                                  where !request.Key.StartsWith("api")
                                  orderby request.Value descending
                                  select new
                                  {
                                      Url = request.Key,
                                      Count = request.Value
                                  })
                    },
                    Api = new
                    {
                        Total = (from req in Statistic.WebRequests where req.Key.StartsWith("api") select req.Value).Sum(),
                        Detail = (from request in Statistic.WebRequests
                                  where request.Key.StartsWith("api")
                                  orderby request.Value descending
                                  select new
                                  {
                                      Url = request.Key,
                                      Count = request.Value
                                  })
                    },
                    Database = new
                    {
                        Total = (from req in Statistic.DbRequests select req.Value).Sum(),
                        Detail = (from request in Statistic.DbRequests
                                  orderby request.Value descending
                                  select new
                                  {
                                      Name = request.Key,
                                      Count = request.Value
                                  })
                    }
                },
                Historian = (from rec in Statistic.Historian
                             orderby rec.Time descending
                             select new
                             {
                                 Time = rec.Time,
                                 Count = rec.Count,
                                 Elapsed = rec.Elapsed
                             })
            });
        }

        // POST: api/Status
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Status/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Status/5
        public void Delete(int id)
        {
        }
    }
}
