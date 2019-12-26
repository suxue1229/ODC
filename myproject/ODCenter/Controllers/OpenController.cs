using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ODCenter.Controllers
{
    public class OpenController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Router");
        }

        public ActionResult Router()
        {
            return View();
        }

        [Route("Open/Router/Query/")]
        public JsonResult RouterQuery()
        {
            Guid id;
            Int32 query;
            if (!String.IsNullOrWhiteSpace(Request.Params["query"]) && Int32.TryParse(Request.Params["query"], out query))
            {
                if (query < 0 || query >= 10000)
                {
                    return Json(new { status = 2, message = "invalid id" });
                }
                try
                {
                    String url = ODCenter.Base.Router.Combine("/link/list");
                    JArray cons = JArray.Parse(ODCenter.Base.Router.HttpGet(url));
                    for (int i = cons.Count - 1; i >= 0; i--)
                    {
                        Int32 dtu = cons[i]["remote"]["dtu"].Value<Int32>();
                        if (dtu % 10000 != query)
                        {
                            cons.RemoveAt(i);
                        }
                    }
                    return Json(new { status = 0, result = new JavaScriptSerializer().DeserializeObject(cons.ToString()) });
                }
                catch { }
            }
            else if (!String.IsNullOrWhiteSpace(Request.Params["id"]) && Guid.TryParse(Request.Params["id"], out id))
            {
                String url = ODCenter.Base.Router.Combine(String.Format("/link/list/{0}", id.ToString("N")));
                String dat = ODCenter.Base.Router.HttpGet(url);
                if (!String.IsNullOrWhiteSpace(dat))
                {
                    return Json(new { status = 0, result = new JavaScriptSerializer().DeserializeObject(dat) });
                }
            }
            return Json(new { status = 1, message = "internal error" });
        }

        [Route("Open/Router/Detail/{id?}")]
        public ActionResult RouterDetail(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
            {
                return RedirectToAction("Router");
            }
            ViewData["id"] = id.Value.ToString("N");
            return View();
        }
    }
}