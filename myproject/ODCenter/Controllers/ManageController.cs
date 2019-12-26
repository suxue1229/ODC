using ODCenter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    //设备管理
    [Authorize]
    public class ManageController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "设备管理";
            ViewData["controller"] = "Manage";
            return View("Group");
        }

        public ActionResult Group(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                Group group = DbProvider.Groups[id.Value.ToString("N")];
                ViewBag.Title = "设备管理 - " + group.Name;
                ViewData["controller"] = "Manage";
                ViewData["group"] = group.Id.ToString("N");
                return View("Group");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Detail(Guid? id)
        {
            if (id.HasValue && DbProvider.Devices.ContainsKey(id.Value.ToString("N")))
            {
                Device device = DbProvider.Devices[id.Value.ToString("N")];
                ViewBag.Title = "设备管理 - " + device.Name;
                ViewData["controller"] = "Manage";
                ViewData["id"] = device.Id.ToString("N");
                return View("Detail");
            }
            return RedirectToAction("Index");
        }
    }
}