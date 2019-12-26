using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Net;
using System.Web.Mvc;
using System.Linq;

namespace ODCenter.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ClientController : Controller
    {
        public ActionResult Index(Guid? id)
        {
            ViewData["institute"] = id;
            return View(DbProvider.Clients.All());
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = DbProvider.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        public ActionResult Create(Guid? id)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                ViewData["ins_id"] = id.Value.ToString("N");
                return View();
            }
            return RedirectToAction("Index", new { id = id });
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Guid? id, [Bind(Include = "Name,Index,Dtu,Longitude,Latitude")] ClientInfo client)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                if (ModelState.IsValid)
                {
                    DbProvider.Clients.Create(client, id.Value, User.Identity.Name);
                    return RedirectToAction("Index", new { id = id });
                }
                return View(client);
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
            var client = DbProvider.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Index,Dtu,Longitude,Latitude")] ClientInfo newclient)
        {
            if (ModelState.IsValid)
            {
                DbProvider.Clients.Update(newclient, User.Identity.Name);
                return RedirectToAction("Index");
            }
            return View(newclient);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = DbProvider.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var client = DbProvider.Clients.Find(id);
            DbProvider.Clients.Delete(id, User.Identity.Name);
            if (client != null)
            {
                return RedirectToAction("Index", new { id = client.Institute });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Client/Sensor/List/{id?}")]
        public ActionResult SensorList(Guid? id)
        {
            if (id.HasValue && DbProvider.Clients.ContainsKey(id.Value.ToString("N")))
            {
                Client client = DbProvider.Clients[id.Value.ToString("N")];
                ViewBag.Institute = client.Institute.ToString("N");
                return View(client.Sensors.Values);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Client/Sensor/List/{id?}")]
        public ActionResult SensorListEdit(Guid? id)
        {
            if (id.HasValue && DbProvider.Clients.ContainsKey(id.Value.ToString("N")))
            {
                Guid sid = Guid.Empty;
                Int32 sidx = 0;
                if (Guid.TryParse(Request.Form["sensor"], out sid))
                {
                    DbProvider.Clients.ConnectSensor(id.Value, sid, Int32.TryParse(Request.Form["index"], out sidx) ? (Int32?)sidx : null);
                }
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [Route("Client/Sensor/Add/{id?}")]
        public ActionResult SensorAdd(Guid? id)
        {
            if (id.HasValue && DbProvider.Clients.ContainsKey(id.Value.ToString("N")))
            {
                return View(DbProvider.Clients.SensorsToConnect(DbProvider.Clients[id.Value.ToString("N")].Institute, true));
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [HttpPost, ActionName("SensorAdd")]
        [ValidateAntiForgeryToken]
        [Route("Client/Sensor/Add/{id?}")]
        public ActionResult SensorAddConfirm(Guid? id)
        {
            if (id.HasValue && DbProvider.Clients.ContainsKey(id.Value.ToString("N")))
            {
                foreach (String senstr in Request.Form.AllKeys)
                {
                    Guid senid = Guid.Empty;
                    if (Guid.TryParse(senstr, out senid) && DbProvider.Sensors.ContainsKey(senid.ToString("N")))
                    {
                        DbProvider.Clients.ConnectSensor(id.Value, senid);
                    }
                }
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [Route("Client/Sensor/Remove/{id?}")]
        public ActionResult SensorRemove(Guid? id)
        {
            Guid senid = Guid.Empty;
            if (id.HasValue && Guid.TryParse(Request.QueryString["sid"], out senid))
            {
                DbProvider.Clients.DisconnectSensor(id.Value, senid);
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
