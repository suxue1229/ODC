using ODCenter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    //实时监控
    [Authorize]
    public class MonitorController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "实时监控";
            ViewData["controller"] = "Monitor";
            Guid? insid = Account.GetInstitute();
            if (insid.HasValue)
            {
                ViewData["institute"] = insid.Value.ToString("N");
            }
            return View("Group");
        }

        public ActionResult Group(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                Group group = DbProvider.Groups[id.Value.ToString("N")];
                ViewBag.Title = "实时监控 - " + group.Name;
                ViewData["controller"] = "Monitor";
                ViewData["institute"] = group.Institute.ToString("N");
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
                ViewBag.Title = "实时监控 - " + sensor.Name;
                ViewData["controller"] = "Monitor";
                ViewData["id"] = sensor.Id.ToString("N");
                return View("Detail");
            }
            return RedirectToAction("Index");
        }
    }
}