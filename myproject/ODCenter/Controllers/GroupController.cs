using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class GroupController : Controller
    {
        public ActionResult Index(Guid? id)
        {
            ViewData["institute"] = id;
            return View(DbProvider.Groups.All());
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = DbProvider.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        public ActionResult Create(Guid? id)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                return View();
            }
            return RedirectToAction("Index", new { id = id });
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Guid? id, [Bind(Include = "Name")] GroupInfo group)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                if (ModelState.IsValid)
                {
                    DbProvider.Groups.Create(group, id.Value, User.Identity.Name);
                    return RedirectToAction("Index", new { id = id });
                }
                return View(group);
            }
            else
            {
                return RedirectToAction("LogOff", "Account");
            }
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = DbProvider.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] GroupInfo newgroup)
        {
            if (ModelState.IsValid)
            {
                DbProvider.Groups.Update(newgroup, User.Identity.Name);
                return RedirectToAction("Index");
            }
            return View(newgroup);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = DbProvider.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var group = DbProvider.Groups.Find(id);
            DbProvider.Groups.Delete(id, User.Identity.Name);
            if (group != null)
            {
                return RedirectToAction("Index", new { id = group.Institute });
            }
            return RedirectToAction("Index");
        }

        [Route("Group/Device/List/{id?}")]
        public ActionResult DeviceList(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                Group group = DbProvider.Groups[id.Value.ToString("N")];
                ViewBag.Institute = group.Institute.ToString("N");
                return View(group.Devices.Values);
            }
            return RedirectToAction("Index");
        }

        [Route("Group/Device/Add/{id?}")]
        public ActionResult DeviceAdd(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                return View(DbProvider.Groups.DevicesToConnect(DbProvider.Groups[id.Value.ToString("N")].Institute, true));
            }
            return RedirectToAction("DeviceList", new { id = id });
        }

        [HttpPost, ActionName("DeviceAdd")]
        [ValidateAntiForgeryToken]
        [Route("Group/Device/Add/{id?}")]
        public ActionResult DeviceAddConfirm(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                foreach (String devstr in Request.Form.AllKeys)
                {
                    Guid devid = Guid.Empty;
                    if (Guid.TryParse(devstr, out devid) && DbProvider.Devices.ContainsKey(devid.ToString("N")))
                    {
                        DbProvider.Groups.ConnectDevice(id.Value, devid);
                    }
                }
            }
            return RedirectToAction("DeviceList", new { id = id });
        }

        [Route("Group/Device/Remove/{id?}")]
        public ActionResult DeviceRemove(Guid? id)
        {
            Guid devid = Guid.Empty;
            if (id.HasValue && Guid.TryParse(Request.QueryString["did"], out devid))
            {
                DbProvider.Groups.DisconnectDevice(id.Value, devid);
            }
            return RedirectToAction("DeviceList", new { id = id });
        }

        [Route("Group/Sensor/List/{id?}")]
        public ActionResult SensorList(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                Group group = DbProvider.Groups[id.Value.ToString("N")];
                ViewBag.Institute = group.Institute.ToString("N");
                ViewData["VisIds"] = group.Visibles;
                return View(group.Sensors.Values);
            }
            return RedirectToAction("Index");
        }

        [Route("Group/Sensor/Add/{id?}")]
        public ActionResult SensorAdd(Guid? id)
        {
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                return View(DbProvider.Groups.SensorsToConnect(DbProvider.Groups[id.Value.ToString("N")].Institute, true));
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [HttpPost, ActionName("SensorAdd")]
        [ValidateAntiForgeryToken]
        [Route("Group/Sensor/Add/{id?}")]
        public ActionResult SensorAddConfirm(Guid? id)
        {
            Boolean hiddenmode = !String.IsNullOrWhiteSpace(Request.Form["hiddenmode"]);
            if (id.HasValue && DbProvider.Groups.ContainsKey(id.Value.ToString("N")))
            {
                foreach (String senstr in Request.Form.AllKeys)
                {
                    Guid senid = Guid.Empty;
                    if (Guid.TryParse(senstr, out senid) && DbProvider.Sensors.ContainsKey(senid.ToString("N")))
                    {
                        DbProvider.Groups.ConnectSensor(id.Value, senid, !hiddenmode);
                    }
                }
            }
            return RedirectToAction("SensorList", id);
        }

        [Route("Group/Sensor/Show/{id?}")]
        public ActionResult SensorShow(Guid? id)
        {
            Guid senid = Guid.Empty;
            if (id.HasValue && Guid.TryParse(Request.QueryString["sid"], out senid))
            {
                DbProvider.Groups.ShowSensor(id.Value, senid);
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [Route("Group/Sensor/Hide/{id?}")]
        public ActionResult SensorHide(Guid? id)
        {
            Guid senid = Guid.Empty;
            if (id.HasValue && Guid.TryParse(Request.QueryString["sid"], out senid))
            {
                DbProvider.Groups.HideSensor(id.Value, senid);
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [Route("Group/Sensor/Remove/{id?}")]
        public ActionResult SensorRemove(Guid? id)
        {
            Guid senid = Guid.Empty;
            if (id.HasValue && Guid.TryParse(Request.QueryString["sid"], out senid))
            {
                DbProvider.Groups.DisconnectSensor(id.Value, senid);
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Custom Dispose Operation
            }
            base.Dispose(disposing);
        }
    }
}
