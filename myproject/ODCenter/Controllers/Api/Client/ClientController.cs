using ODCenter.Base;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using System.Collections.Generic;

namespace ODCenter.Controllers.Api
{
    [Authorize(Roles = UserRoles.Admin), Route("Api/Client/{id?}/{method?}")]
    public class ClientController : ApiController
    {
        // GET: api/Client
        public IHttpActionResult Get()
        {
            Client cli = null;
            Int32 plcid, dtuid;
            if (Int32.TryParse(Request.Query("plcid"), out plcid))
            {
                cli = DbProvider.Clients.Find(plcid);
            }
            if (cli != null && cli.Dtu.HasValue && !String.IsNullOrWhiteSpace(Request.Query("dtuid")))
            {
                if (!(Int32.TryParse(Request.Query("dtuid"), out dtuid) && cli.Dtu == dtuid))
                {
                    cli = null;
                }
            }
            if (cli != null)
            {
                return Get(cli.Id);
            }
            return this.Succeed((from client in DbProvider.Clients.Values
                                 orderby client.Name
                                 select new
                                 {
                                     Id = client.Id,
                                     Name = client.Name,
                                     Institute = client.Institute,
                                     PLCID = client.Index,
                                     DTUID = client.Dtu,
                                     Longitude = client.Longitude,
                                     Latitude = client.Latitude,
                                     LastActive = client.LastActive,
                                 }));
        }

        // GET: api/Client/5
        public IHttpActionResult Get(Guid id)
        {
            Client client = null;
            if ((client = DbProvider.Clients[id.ToString("N")]) != null)
            {
                return this.Succeed(new
                {
                    Id = client.Id,
                    Name = client.Name,
                    Institute = client.Institute,
                    PLCID = client.Index,
                    DTUID = client.Dtu,
                    Longitude = client.Longitude,
                    Latitude = client.Latitude,
                    LastActive = client.LastActive,
                    Sensors = (from sen in client.Sensors.Values
                               select new
                               {
                                   Id = sen.Sensor.Id,
                                   Name = sen.Sensor.Name,
                                   Index = sen.Index,
                                   Host = sen.Sensor.OPCHost,
                                   Server = sen.Sensor.OPCServer,
                                   Item = sen.Sensor.OPCItem,
                                   Interval = sen.Sensor.Interval,
                                   Gain = sen.Sensor.Gain,
                                   Offset = sen.Sensor.Offset,
                                   Unit = sen.Sensor.Unit
                               })
                });
            }
            return this.Failed("client_not_found", ApiStatusCode.NotFound);
        }

        public IHttpActionResult Get(Guid id, String method)
              
        {
            SortedList<String, Client> opc_clients = DbProvider.OPCClients();
            Client client = null;
            if ((client = DbProvider.Clients[id.ToString("N")]) != null)
            {
                switch (method.ToLower())
                {
                    case "status":
                        return this.Succeed(new
                        {
                            Config = client.ConfigHash(opc_clients),
                            Client = Update.ClientHash,
                            Update = Update.UpdateHash
                        });
                    case "config":
                        return new ResponseMessageResult(new HttpResponseMessage()
                        {
                            Content = new StringContent(client.ConfigText(opc_clients), Encoding.UTF8, "application/json")
                        });
                    default:
                        return this.Failed("method_not_support", ApiStatusCode.NotFound);
                }
            }
            return this.Failed("client_not_found", ApiStatusCode.NotFound);
        }

        // POST: api/Client
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Client/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Client/5
        public void Delete(int id)
        {
        }
    }
}
