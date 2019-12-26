using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize]
    public class DeviceController : Controller
    {
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Index(Guid? id)
        {
            ViewData["institute"] = id;
            return View(DbProvider.Devices.All());
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceInfo device = DbProvider.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Create(Guid? id)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                return View(new DeviceInfo()
                {
                    Status = "未定义"
                });
            }
            return RedirectToAction("Index", new { id = id });
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Create(Guid? id, [Bind(Include = "Name,Statsrc,Status")] DeviceInfo device)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                if (ModelState.IsValid)
                {
                    DbProvider.Devices.Create(device, id.Value, User.Identity.Name);
                    return RedirectToAction("Index", new { id = id });
                }
                return View(device);
            }
            else
            {
                return RedirectToAction("LogOff", "Account");
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceInfo device = DbProvider.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Edit([Bind(Include = "Id,Name,Statsrc,Status")] DeviceInfo newdevice)
        {
            if (ModelState.IsValid)
            {
                DbProvider.Devices.Update(newdevice, User.Identity.Name);
                return RedirectToAction("Index");
            }
            return View(newdevice);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceInfo device = DbProvider.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var device = DbProvider.Devices.Find(id);
            DbProvider.Devices.Delete(id, User.Identity.Name);
            if (device != null)
            {
                return RedirectToAction("Index", new { id = device.Institute });
            }
            return RedirectToAction("Index");
        }

        public ActionResult State(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceInfo device = DbProvider.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult State([Bind(Include = "Id,Name,Status")] DeviceInfo newdevice)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (ModelState.IsValid)
            {
                DeviceInfo oridevice = DbProvider.Devices.Find(newdevice.Id);
                if (oridevice == null)
                {
                    json.Data = new { result = "error", msg = "未找到相关设备，可能已被删除" };
                }
                else if (oridevice.Statsrc == Dev_Stat.Automatic || oridevice.Statsrc == Dev_Stat.None)
                {
                    json.Data = new { result = "error", msg = "不允许手动设置该设备的状态" };
                }
                else
                {
                    DbProvider.Devices.Update(newdevice, User.Identity.Name);
                    json.Data = new { result = "ok", msg = "" };
                }
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                foreach (ModelState state in ModelState.Values)
                {
                    foreach (ModelError error in state.Errors)
                    {
                        builder.AppendLine(error.ErrorMessage);
                    }
                }
                json.Data = new
                {
                    State = "Failed",
                    Errors = builder.ToString()
                };
            }
            return json;
        }

        #region Track Operation
        private DeviceDbContext db = new DeviceDbContext();

        [Route("Device/Track/Create/{id?}")]
        public ActionResult TrackCreate(Guid? id)
        {
            return View(new DeviceTrack()
            {
                Time = DateTime.Now,
                DeviceId = id.HasValue ? id.Value : Guid.Empty
            });
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Device/Track/Create/{id?}")]
        public JsonResult TrackCreate([Bind(Include = "HisTime,DeviceId,Description")] DeviceTrack deviceTrack)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (ModelState.IsValid)
            {
                deviceTrack.Enabled = true;
                deviceTrack.Operator = User.Identity.Name;
                db.Tracks.Add(deviceTrack);
                db.SaveChanges();
                json.Data = new { State = "Success", Errors = "" };
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                foreach (ModelState state in ModelState.Values)
                {
                    foreach (ModelError error in state.Errors)
                    {
                        builder.AppendLine(error.ErrorMessage);
                    }
                }
                json.Data = new
                {
                    State = "Failed",
                    Errors = builder.ToString()
                };
            }
            return json;
        }

        [Route("Device/Track/Edit/{id?}")]
        public ActionResult TrackEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceTrack deviceTrack = db.Tracks.Find(id);
            if (deviceTrack == null)
            {
                return HttpNotFound();
            }
            return View(deviceTrack);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Device/Track/Edit/{id?}")]
        public JsonResult TrackEdit([Bind(Include = "Id,HisTime,DeviceId,Description")] DeviceTrack deviceTrack)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (ModelState.IsValid)
            {
                DeviceTrack oritrack = db.Tracks.Find(deviceTrack.Id);
                if (oritrack == null || oritrack.Enabled == false)
                {
                    json.Data = new { State = "Failed", Errors = "未找到该记录，可能已被删除。" };
                }
                else
                {
                    oritrack.Description = deviceTrack.Description;
                    oritrack.Operator = User.Identity.Name;
                    db.Entry(oritrack).State = EntityState.Modified;
                    db.SaveChanges();
                    json.Data = new { State = "Success", Errors = "" };
                }
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                foreach (ModelState state in ModelState.Values)
                {
                    foreach (ModelError error in state.Errors)
                    {
                        builder.AppendLine(error.ErrorMessage);
                    }
                }
                json.Data = new
                {
                    State = "Failed",
                    Errors = builder.ToString()
                };
            }
            return json;
        }

        [Route("Device/Track/Delete/{id?}")]
        public ActionResult TrackDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceTrack deviceTrack = db.Tracks.Find(id);
            if (deviceTrack == null)
            {
                return HttpNotFound();
            }
            return View(deviceTrack);
        }

        [HttpPost, ActionName("TrackDelete")]
        [ValidateAntiForgeryToken]
        [Route("Device/Track/Delete/{id?}")]
        public JsonResult TrackDeleteConfirmed(int id)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            DeviceTrack deviceTrack = db.Tracks.Find(id);
            if (deviceTrack == null || deviceTrack.Enabled == false)
            {
                json.Data = new { State = "Failed", Errors = "未找到该记录，可能已被删除。" };
            }
            else
            {
                deviceTrack.Enabled = false;
                db.Entry(deviceTrack).State = EntityState.Modified;
                db.SaveChanges();
                json.Data = new { State = "Success", Errors = "" };
            }
            return json;
        }

        [Route("Device/Track/Fetch/{id?}")]
        public JsonResult TrackFetch(Guid? id)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id.HasValue && DbProvider.Devices.ContainsKey(id.Value.ToString("N")))
            {
                Device dev = DbProvider.Devices[id.Value.ToString("N")];
                json.Data = (from rec in dev.Track
                             select new
                             {
                                 id = rec.Id,
                                 log = rec.Log,
                                 time = rec.Time.ToString(),
                                 oper = rec.Operator
                             });
            }
            return json;
        }
        #endregion

        [Authorize(Roles = UserRoles.Admin)]
        [Route("Device/Sensor/List/{id?}")]
        public ActionResult SensorList(Guid? id)
        {
            if (id.HasValue && DbProvider.Devices.ContainsKey(id.Value.ToString("N")))
            {
                ViewData["GroupId"] = id.Value.ToString("N");
                return View(DbProvider.Devices[id.Value.ToString("N")].Sensors.Values);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = UserRoles.Admin)]
        [Route("Device/Sensor/Add/{id?}")]
        public ActionResult SensorAdd(Guid? id)
        {
            if (id.HasValue && DbProvider.Devices.ContainsKey(id.Value.ToString("N")))
            {
                return View(DbProvider.Devices.SensorsToConnect(DbProvider.Devices[id.Value.ToString("N")].Institute, true));
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("SensorAdd")]
        [ValidateAntiForgeryToken]
        [Route("Device/Sensor/Add/{id?}")]
        public ActionResult SensorAddConfirm(Guid? id)
        {
            if (id.HasValue && DbProvider.Devices.ContainsKey(id.Value.ToString("N")))
            {
                foreach (String senstr in Request.Form.AllKeys)
                {
                    Guid senid = Guid.Empty;
                    if (Guid.TryParse(senstr, out senid) && DbProvider.Sensors.ContainsKey(senid.ToString("N")))
                    {
                        DbProvider.Devices.ConnectSensor(id.Value, senid);
                    }
                }
            }
            return RedirectToAction("SensorList", new { id = id });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [Route("Device/Sensor/Remove/{id?}")]
        public ActionResult SensorRemove(Guid? id)
        {
            Guid senid = Guid.Empty;
            if (id.HasValue && Guid.TryParse(Request.QueryString["sid"], out senid))
            {
                DbProvider.Devices.DisconnectSensor(id.Value, senid);
            }
            return RedirectToAction("SensorList", id);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Custom Dispose Operation
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Json Methods

        public JsonResult Group(Guid? id)
        {
            Int32 offset = 0, limit = 10;
            if (!String.IsNullOrWhiteSpace(Request.Form["offset"]))
            {
                Int32.TryParse(Request.Form["offset"], out offset);
                offset = Math.Max(offset, 0);
            }
            if (!String.IsNullOrWhiteSpace(Request.Form["limit"]))
            {
                Int32.TryParse(Request.Form["limit"], out limit);
                limit = Math.Max(limit, 1);
            }
            Guid? insid = Account.GetInstitute();
            if (insid.HasValue)
            {
                String groupid = id.HasValue ? id.Value.ToString("N") : null;
                JsonResult json = new JsonResult();
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                if (DbProvider.Institutes.ContainsKey(insid.Value.ToString("N")))
                {
                    Institute ins = DbProvider.Institutes[insid.Value.ToString("N")];
                    if (groupid == null)
                    {
                        offset = 0;
                        json.Data = new
                        {
                            institute = ins.Id.ToString("N"),
                            time = DateTime.Now.ToString(),
                            groups = (from grp in ins.Groups.Values
                                      select new
                                      {
                                          id = grp.Id.ToString("N"),
                                          name = grp.Name,
                                          offset = offset,
                                          limit = limit,
                                          count = grp.Devices.Count,
                                          devices = (from dev in grp.Devices.Values.OrderBy(x => x.Name).Skip(offset).Take(limit)
                                                     select new
                                                     {
                                                         id = dev.Id.ToString("N"),
                                                         name = dev.Name,
                                                         edit = (dev.Statsrc == Dev_Stat.Manual || dev.Statsrc == Dev_Stat.Unlimited),
                                                         status = dev.Status
                                                     })
                                      })
                        };
                    }
                    else if (ins.Groups.ContainsKey(groupid))
                    {
                        Group grp = ins.Groups[groupid];
                        if (offset >= grp.Devices.Count)
                        {
                            offset = (grp.Devices.Count % limit == 0 ? grp.Devices.Count / limit - 1 : grp.Devices.Count / limit) * limit;
                        }
                        json.Data = new
                        {
                            institute = ins.Id.ToString("N"),
                            time = DateTime.Now.ToString(),
                            groups = new[]
	                        {
	                            new 
	                            {
	                                id = grp.Id.ToString("N"),
	                                name = grp.Name,
                                    offset=offset,
                                    limit=limit,
                                    count=grp.Devices.Count,
	                                devices = (from dev in grp.Devices.Values.OrderBy(x=>x.Name).Skip(offset).Take(limit)
	                                          select new
	                                          {
	                                              id = dev.Id.ToString("N"),
	                                              name = dev.Name,
                                                  edit = (dev.Statsrc == Dev_Stat.Manual || dev.Statsrc == Dev_Stat.Unlimited),
	                                              status = dev.Status
	                                          })
	                            }
	                        }
                        };
                    }
                }
                return json;
            }
            else
            {
                RedirectToAction("LogOff", "Account");
                return null;
            }
        }

        public JsonResult Detail(Guid? id)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id.HasValue && DbProvider.Devices.ContainsKey(id.Value.ToString("N")))
            {
                Device dev = DbProvider.Devices[id.Value.ToString("N")];
                json.Data = new
                {
                    id = dev.Id.ToString("N"),
                    srvtime = DateTime.Now.ToString(),
                    name = dev.Name,
                    edit = (dev.Statsrc == Dev_Stat.Unlimited || dev.Statsrc == Dev_Stat.Manual),
                    status = dev.Status,
                    ins = new
                    {
                        id = dev.Institute.ToString("N"),
                        name = DbProvider.Institutes[dev.Institute.ToString("N")].Name,
                        type = DbProvider.Institutes[dev.Institute.ToString("N")].Type.ToString()
                    },
                    sensors = (from sen in dev.Sensors.Values
                               select new
                               {
                                   id = sen.Id.ToString("N"),
                                   name = sen.Name,
                                   edit = (dev.Statsrc == Dev_Stat.Manual || dev.Statsrc == Dev_Stat.Unlimited),
                                   time = sen.Time.ToString(),
                                   value = sen.Value.ToString("0.0000"),
                                   unit = sen.Unit
                               })
                };
            }
            return json;
        }

        #endregion
    }
}
