using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ODCenter.Controllers.Api
{
    [Authorize, Route("Api/Data/{id?}")]
    public class DataController : ApiController
    {
        // GET: api/Data
        public IHttpActionResult Get()
        {
            return Get(Guid.Empty);
        }

        // GET: api/Data/{id}
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return this.Failed("data_invalid", ApiStatusCode.DataInvalid);
            }
            String type = Request.Query("type");
            Boolean mode_full = (!String.IsNullOrWhiteSpace(Request.Query("mode")) && String.Compare(Request.Query("mode"), "full", true) == 0);
            if (DbProvider.Institutes.ContainsKey(id.ToString("N")) &&
                (String.IsNullOrWhiteSpace(type) || String.Compare(type, "institute", true) == 0))
            {
                Institute ins = DbProvider.Institutes[id.ToString("N")];
                return this.Succeed(new
                {
                    Id = ins.Id,
                    Name = ins.Name,
                    Type = ins.Type,
                    Groups = (from grp in ins.Groups.Values.OrderBy(g => g.Name)
                              select new
                              {
                                  Id = grp.Id,
                                  Name = grp.Name,
                                  Count = mode_full ? grp.Sensors.Count : grp.Visibles.Count,
                                  Devices = (from dev in grp.Devices.Values.OrderBy(d => d.Name)
                                             select new
                                             {
                                                 Id = dev.Id,
                                                 Name = dev.Name,
                                                 Editable = (dev.Statsrc == Dev_Stat.Unlimited || dev.Statsrc == Dev_Stat.Manual),
                                                 Status = dev.Status,
                                                 Sensors = (from sen in dev.Sensors.Values.OrderBy(x => x.Name)
                                                            where (mode_full || grp.Visibles.Contains(sen.Id))
                                                            select new
                                                            {
                                                                Id = sen.Id,
                                                                Name = sen.Name,
                                                                Editable = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                                                Time = sen.Time,
                                                                Value = sen.Value.ToString("0.####"),
                                                                Unit = sen.Unit
                                                            })
                                             }),
                                  Sensors = (from sen in grp.Sensors.Values.Where(x => x.Device == null).OrderBy(x => x.Name)
                                             where (mode_full || grp.Visibles.Contains(sen.Id))
                                             select new
                                             {
                                                 Id = sen.Id,
                                                 Name = sen.Name,
                                                 Editable = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                                 Time = sen.Time,
                                                 Value = sen.Value.ToString("0.####"),
                                                 Unit = sen.Unit
                                             })
                              })

                });
            }
            if (DbProvider.Groups.ContainsKey(id.ToString("N")) &&
                (String.IsNullOrWhiteSpace(type) || String.Compare(type, "group", true) == 0))
            {
                Group grp = DbProvider.Groups[id.ToString("N")];
                return this.Succeed(new
                {
                    Id = grp.Id,
                    Name = grp.Name,
                    Count = mode_full ? grp.Sensors.Count : grp.Visibles.Count,
                    Devices = (from dev in grp.Devices.Values.OrderBy(d => d.Name)
                               select new
                               {
                                   Id = dev.Id,
                                   Name = dev.Name,
                                   Editable = (dev.Statsrc == Dev_Stat.Unlimited || dev.Statsrc == Dev_Stat.Manual),
                                   Status = dev.Status,
                                   Sensors = (from sen in dev.Sensors.Values.OrderBy(x => x.Name)
                                              where (mode_full || grp.Visibles.Contains(sen.Id))
                                              select new
                                              {
                                                  Id = sen.Id,
                                                  Name = sen.Name,
                                                  Editable = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                                  Time = sen.Time,
                                                  Value = sen.Value.ToString("0.####"),
                                                  Unit = sen.Unit
                                              })
                               }),
                    Sensors = (from sen in grp.Sensors.Values.Where(x => x.Device == null).OrderBy(x => x.Name)
                               where (mode_full || grp.Visibles.Contains(sen.Id))
                               select new
                               {
                                   Id = sen.Id,
                                   Name = sen.Name,
                                   Editable = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                   Time = sen.Time,
                                   Value = sen.Value.ToString("0.####"),
                                   Unit = sen.Unit
                               })
                });
            }
            if (DbProvider.Sensors.ContainsKey(id.ToString("N")) &&
                (String.IsNullOrWhiteSpace(type) || String.Compare(type, "sensor", true) == 0))
            {
                Sensor sen = DbProvider.Sensors[id.ToString("N")];
                return this.Succeed(new
                {
                    Id = sen.Id,
                    Name = sen.Name,
                    Editable = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                    Time = sen.Time,
                    Value = sen.Value.ToString("0.####"),
                    Unit = sen.Unit
                });
            }
            return this.Failed("data_not_found", ApiStatusCode.NotFound);
        }

        // POST: api/Data
        public IHttpActionResult Post([FromBody]string value)
        {
            return Ok();
        }

        // PUT: api/Data/5 - Not Allowed
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/Data/5 - Not Allowed
        public void Delete(int id)
        {

        }
    }
}
