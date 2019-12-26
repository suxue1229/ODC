using Newtonsoft.Json;
using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize]
    public class MapController : Controller
    {
        public ActionResult Index(Guid? id)
        {
            return View();
        }

        public ContentResult List(Guid? id)
        {
            var inslst = Account.GetInstitutes(User.Identity.Name, true).GetSelectList(Guid.Empty);
            Object data = null;
            if (id.HasValue)
            {
                if (DbProvider.Institutes.ContainsKey(id.Value.ToString("N")) && 
                    inslst.FirstOrDefault(i => i.Value == id.Value.ToString("N")) != null)
                {
                    Institute ins = DbProvider.Institutes[id.Value.ToString("N")];
                    data = new
                    {
                        status = 0,
                        message = "ok",
                        results = new[] { 
                            new Institute(){ 
                                Id=ins.Id,
                                Type=ins.Type,
                                Name = ins.Name, 
                                Latitude=ins.Latitude,
                                Longitude=ins.Longitude,
                                Address=ins.Address,
                                 Clients=null,
                                  Groups=null,
                                   Reports=null
                            }}
                    };
                }
                else
                {
                    data = new
                    {
                        status = -1,
                        message = "institute not found",
                        result = new Institute[0]
                    };
                }
            }
            else
            {
                data = new
                {
                    status = 0,
                    message = "ok",
                    results = (from ins in DbProvider.Institutes.Values
                               where inslst.FirstOrDefault(i => i.Value == ins.Id.ToString("N")) != null
                               orderby ins.Name
                               select new Institute()
                               {
                                   Id = ins.Id,
                                   Name = ins.Name,
                                   Type = ins.Type,
                                   Latitude = ins.Latitude,
                                   Longitude = ins.Longitude,
                                   Address = ins.Address,
                                   Clients = null,
                                   Groups = null,
                                   Reports = null
                               })
                };
            }
            return this.Content(JsonConvert.SerializeObject(data), "application/json");
        }
    }
}