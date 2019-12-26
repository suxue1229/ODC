using ODCenter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    //能耗分析
    [Authorize]
    public class AnalysisController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "能耗分析";
            ViewData["controller"] = "Analysis";
            return View("Group");
        }

        public ActionResult Group(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                Group group = DbProvider.Groups[id.Value.ToString("N")];
                ViewBag.Title = "能耗分析 - "+group.Name;
                ViewData["controller"] = "Analysis";
                ViewData["group"] = group.Id.ToString("N");
                return View("Group");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Detail(Guid? id)
        {
            if (id.HasValue && DbProvider.Sensors.ContainsKey(id.Value.ToString("N")))
            {
                Sensor sensor = DbProvider.Sensors[id.Value.ToString("N")];
                ViewBag.Title = "能耗分析 - "+sensor.Name;
                ViewData["controller"] = "Analysis";
                ViewData["sensor"] = sensor.Id.ToString("N");
                return View("Detail");
            }
            return RedirectToAction("Index");
        }
    }
}