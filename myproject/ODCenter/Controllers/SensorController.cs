using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize]
    public class SensorController : Controller
    {
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue && !DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                return RedirectToAction("Index", new { id = (Guid?)null });
            }
            ViewData["institute"] = id;
            return View(DbProvider.Sensors.All());
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sensor = DbProvider.Sensors.Find(id);
            if (sensor == null)
            {
                return HttpNotFound();
            }
            return View(sensor);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Create(Guid? id)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                var sensor = TempData["sensor"] as SensorInfo;
                if (sensor == null)
                {
                    sensor = new SensorInfo()
                    {
                        OPCHost = "local",
                        Interval = 3,
                        Gain = 1,
                        Offset = 0
                    };
                }
                return View(sensor);
            }
            return RedirectToAction("Index", new { id = id });
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Copy(Guid? id)
        {
            var sensor = DbProvider.Sensors.Find(id);
            if (sensor != null)
            {
                TempData["sensor"] = new SensorInfo()
                {
                    Name = sensor.Name,
                    Device = sensor.Device,
                    SenType = sensor.SenType,
                    DatType = sensor.DatType,
                    Precise = sensor.Precise,
                    DatSource = sensor.DatSource,
                    OPCHost = sensor.OPCHost,
                    OPCServer = sensor.OPCServer,
                    OPCItem = sensor.OPCItem,
                    Interval = sensor.Interval,
                    Minimum = sensor.Minimum,
                    Maximum = sensor.Maximum,
                    Gain = sensor.Gain,
                    Offset = sensor.Offset,
                    Unit = sensor.Unit,
                    Order = sensor.Order.HasValue ? sensor.Order + 1 : null,
                    Important = sensor.Important
                };
                return RedirectToAction("Create", new { id = sensor.Institute.ToString("N") });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Create(Guid? id, [Bind(Include = "Name,Device,DatType,SenType,Inverse,Precise,DatSource,OPCHost,OPCServer,OPCItem,Interval,Minimum,Maximum,Gain,Offset,Unit,Order,Important")] SensorInfo sensor)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                if (ModelState.IsValid)
                {
                    DbProvider.Sensors.Create(sensor, id.Value, User.Identity.Name);
                    return RedirectToAction("Index", new { id = id });
                }
                return View(sensor);
            }
            else
            {
                return RedirectToAction("LogOff", "Account");
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Import(Guid? id)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                return View();
            }
            return RedirectToAction("Index", new { id = id });
        }

        [HttpPost, ActionName("Import")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult ImportConfirm(Guid? id)
        {
            String sentxt = Request.Form["Sentxt"];
            if (id.HasValue && !String.IsNullOrWhiteSpace(sentxt) && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                if (Request.Form["commit"] != "true")
                {
                    //处理导入文本
                    sentxt += "\n";
                    List<String> fails = new List<String>();
                    String opchost = "local", opcserver = "";
                    Int32 interval = 3;
                    Sen_Type _type = Sen_Type.Undefined;
                    Sen_Source _source = Sen_Source.Client;
                    Double minimum = 0, maximum = 0, gain = 1, offset = 0;
                    List<SensorInfo> sens = new List<SensorInfo>();
                    System.Text.RegularExpressions.Regex regline = new System.Text.RegularExpressions.Regex("([^\n]*)\n");
                    System.Text.RegularExpressions.Regex defline = new System.Text.RegularExpressions.Regex("^default.([^=]*)=(.*)$");
                    //1.Name, 2.Type, 3.Source, 4.OPCHost, 5.OPCServer, 6.OPCItem, 7.Interval, 8.Minimum, 9.Maximum, 10:Gain, 11.Offset, 12.Unit
                    System.Text.RegularExpressions.Regex senline = new System.Text.RegularExpressions.Regex("^([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*)$");
                    foreach (System.Text.RegularExpressions.Match mat in regline.Matches(sentxt))
                    {
                        String line = mat.Groups[1].Value.Trim();
                        if (line.Length == 0 || line.Substring(0, 2) == "//")
                        {
                            continue;
                        }
                        System.Text.RegularExpressions.Match m = defline.Match(line);
                        if (m.Success)
                        {
                            switch (m.Groups[1].Value.ToLower())
                            {
                                case "opchost":
                                    opchost = m.Groups[2].Value;
                                    break;
                                case "opcserver":
                                    opcserver = m.Groups[2].Value;
                                    break;
                                case "type":
                                    Enum.TryParse(m.Groups[2].Value, out _type);
                                    break;
                                case "source":
                                    Enum.TryParse(m.Groups[2].Value, out _source);
                                    break;
                                case "interval":
                                    Int32.TryParse(m.Groups[2].Value, out interval);
                                    break;
                                case "minimum":
                                    Double.TryParse(m.Groups[2].Value, out minimum);
                                    break;
                                case "maximum":
                                    Double.TryParse(m.Groups[2].Value, out maximum);
                                    break;
                                case "gain":
                                    Double.TryParse(m.Groups[2].Value, out gain);
                                    break;
                                case "offset":
                                    Double.TryParse(m.Groups[2].Value, out offset);
                                    break;
                            }
                            continue;
                        }
                        m = senline.Match(line);
                        if (m.Success)
                        {
                            SensorInfo sen = new SensorInfo();
                            sen.Name = m.Groups[1].Value.Trim();
                            Sen_Type stmp = Sen_Type.Undefined;
                            if (Enum.TryParse(m.Groups[2].Value.Trim(), out stmp))
                            {
                                sen.SenType = stmp;
                            }
                            else
                            {
                                sen.SenType = _type;
                            }
                            Sen_Source etmp = Sen_Source.Client;
                            if (Enum.TryParse(m.Groups[3].Value.Trim(), out etmp))
                            {
                                sen.DatSource = etmp;
                            }
                            else
                            {
                                sen.DatSource = _source;
                            }
                            sen.OPCHost = m.Groups[4].Value.Trim().Length > 0 ? m.Groups[4].Value.Trim() : opchost;
                            sen.OPCServer = m.Groups[5].Value.Trim().Length > 0 ? m.Groups[5].Value.Trim() : opcserver;
                            sen.OPCItem = m.Groups[6].Value.Trim();
                            Int32 itmp = 0;
                            if (Int32.TryParse(m.Groups[7].Value.Trim(), out itmp))
                            {
                                sen.Interval = itmp;
                            }
                            else
                            {
                                sen.Interval = interval;
                            }
                            Double dtmp = 0;
                            if (Double.TryParse(m.Groups[8].Value.Trim(), out dtmp))
                            {
                                sen.Minimum = dtmp;
                            }
                            else
                            {
                                sen.Minimum = minimum;
                            }
                            if (Double.TryParse(m.Groups[9].Value.Trim(), out dtmp))
                            {
                                sen.Maximum = dtmp;
                            }
                            else
                            {
                                sen.Maximum = maximum;
                            }
                            if (Double.TryParse(m.Groups[10].Value.Trim(), out dtmp))
                            {
                                sen.Gain = dtmp;
                            }
                            else
                            {
                                sen.Gain = gain;
                            }
                            if (Double.TryParse(m.Groups[11].Value.Trim(), out dtmp))
                            {
                                sen.Offset = dtmp;
                            }
                            else
                            {
                                sen.Offset = offset;
                            }
                            sen.Unit = m.Groups[12].Value.Trim();
                            if (verifySensor(sen))
                            {
                                sens.Add(sen);
                                continue;
                            }
                        }
                        fails.Add(line);
                    }
                    ViewData["fails"] = fails;
                    StringBuilder builder = new StringBuilder();
                    foreach (SensorInfo inf in sens)
                    {
                        builder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}\n", inf.Name,
                            inf.SenType, inf.DatSource, inf.OPCHost, inf.OPCServer, inf.OPCItem, inf.Interval,
                            inf.Minimum, inf.Maximum, inf.Gain, inf.Offset, inf.Unit);
                    }
                    ViewData["sentxt"] = builder.ToString();
                    return View("ImportList", sens);
                }
                else
                {
                    //保存导入内容
                    List<SensorInfo> sens = new List<SensorInfo>();
                    System.Text.RegularExpressions.Regex regline = new System.Text.RegularExpressions.Regex("([^\n]*)\n");
                    System.Text.RegularExpressions.Regex senline = new System.Text.RegularExpressions.Regex("^([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*),([^,]*)$");
                    foreach (System.Text.RegularExpressions.Match mat in regline.Matches(sentxt))
                    {
                        String line = mat.Groups[1].Value.Trim();
                        if (line.Length == 0)
                        {
                            continue;
                        }
                        System.Text.RegularExpressions.Match m = senline.Match(line);
                        Boolean error = false;
                        if (m.Success)
                        {
                            SensorInfo sen = new SensorInfo();
                            sen.Name = m.Groups[1].Value.Trim();
                            Sen_Type stmp = Sen_Type.Undefined;
                            if (Enum.TryParse(m.Groups[2].Value.Trim(), out stmp))
                            {
                                sen.SenType = stmp;
                            }
                            else
                            {
                                error = true;
                            }
                            Sen_Source etmp = Sen_Source.Client;
                            if (Enum.TryParse(m.Groups[3].Value.Trim(), out etmp))
                            {
                                sen.DatSource = etmp;
                            }
                            else
                            {
                                error = true;
                            }
                            sen.OPCHost = m.Groups[4].Value.Trim();
                            sen.OPCServer = m.Groups[5].Value.Trim();
                            sen.OPCItem = m.Groups[6].Value.Trim();
                            Int32 itmp = 0;
                            if (Int32.TryParse(m.Groups[7].Value.Trim(), out itmp))
                            {
                                sen.Interval = itmp;
                            }
                            else
                            {
                                error = true;
                            }
                            Double dtmp = 0;
                            if (Double.TryParse(m.Groups[8].Value.Trim(), out dtmp))
                            {
                                sen.Minimum = dtmp;
                            }
                            else
                            {
                                error = true;
                            }
                            if (Double.TryParse(m.Groups[9].Value.Trim(), out dtmp))
                            {
                                sen.Maximum = dtmp;
                            }
                            else
                            {
                                error = true;
                            }

                            if (Double.TryParse(m.Groups[10].Value.Trim(), out dtmp))
                            {
                                sen.Gain = dtmp;
                            }
                            else
                            {
                                error = true;
                            }
                            if (Double.TryParse(m.Groups[11].Value.Trim(), out dtmp))
                            {
                                sen.Offset = dtmp;
                            }
                            else
                            {
                                error = true;
                            }
                            sen.Unit = m.Groups[12].Value.Trim();
                            if (error)
                            {

                            }
                            if (verifySensor(sen))
                            {
                                sens.Add(sen);
                            }
                        }
                    }
                    foreach (SensorInfo inf in sens)
                    {
                        DbProvider.Sensors.Create(inf, id.Value, User.Identity.Name);
                    }
                    return RedirectToAction("Index", new { id = id });
                }
            }
            else
            {
                return RedirectToAction("Index", new { id = id });
            }
        }

        private Boolean verifySensor(SensorInfo info)
        {
            Boolean success = true;
            success &= (info.Name.Length > 0);
            success &= (info.OPCHost.Length > 0);
            success &= (info.OPCServer.Length > 0);
            success &= (info.OPCItem.Length > 0);
            success &= (info.Interval > 0);
            return success;
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sensor = DbProvider.Sensors.Find(id);
            if (sensor == null)
            {
                return HttpNotFound();
            }
            return View(sensor);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Edit([Bind(Include = "Id,Name,DatType,SenType,Inverse,Precise,DatSource,OPCHost,OPCServer,OPCItem,Interval,Minimum,Maximum,Gain,Offset,Unit,Order,Important")] SensorInfo newsensor)
        {
            if (ModelState.IsValid)
            {
                DbProvider.Sensors.Update(newsensor, User.Identity.Name);
                return RedirectToAction("Index", new { id = DbProvider.Sensors.ContainsKey(newsensor.Id.ToString("N")) ? DbProvider.Sensors[newsensor.Id.ToString("N")].Institute : Guid.Empty });
            }
            return View(newsensor);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sensor = DbProvider.Sensors.Find(id);
            if (sensor == null)
            {
                return HttpNotFound();
            }
            return View(sensor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var sensor = DbProvider.Sensors.Find(id);
            DbProvider.Sensors.Delete(id, User.Identity.Name);
            if (sensor != null)
            {
                return RedirectToAction("Index", new { id = sensor.Institute });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Custom Dispose Operation
            }
            base.Dispose(disposing);
        }

        #region Custom Method

        public JsonResult Group(Guid? id)
        {
            Boolean fullmode = false;
            Int32 offset = 0, limit = 10;
            if (Request.Cookies["mode"] != null && Request.Cookies["mode"].Value == "full")
            {
                fullmode = true;
            }
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
            Guid insid = Guid.Empty;
            if (!String.IsNullOrWhiteSpace(Request.Params["ins"]) && (User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Engineer)))
            {
                Guid.TryParse(Request.Params["ins"], out insid);
            }
            if (insid == Guid.Empty)
            {
                Guid? ins = Account.GetInstitute();
                if (ins.HasValue)
                {
                    insid = ins.Value;
                }
            }
            if (insid != Guid.Empty)
            {
                String groupid = id.HasValue ? id.Value.ToString("N") : null;
                JsonResult json = new JsonResult();
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                if (DbProvider.Institutes.ContainsKey(insid.ToString("N")))
                {
                    Institute ins = DbProvider.Institutes[insid.ToString("N")];
                    if (ins.Type == Ins_Type.Station)
                    {
                        json.Data = new
                        {
                            institute = ins.Id.ToString("N"),
                            type = ins.Type.ToString(),
                            time = DateTime.Now.ToString(),
                            groups = (from grp in ins.Groups.Values.OrderBy(x => x.Name)
                                      select new
                                      {
                                          id = grp.Id.ToString("N"),
                                          name = grp.Name,
                                          count = grp.Visibles.Count,
                                          devices = (from dev in grp.Devices.Values.OrderBy(x => x.Name)
                                                     select new
                                                     {
                                                         id = dev.Id.ToString("N"),
                                                         name = dev.Name,
                                                         edit = (dev.Statsrc == Dev_Stat.Unlimited || dev.Statsrc == Dev_Stat.Manual),
                                                         status = dev.Status,
                                                         sensors = (from sen in dev.Sensors.Values.OrderBy(x => x.Name)
                                                                    where (fullmode || grp.Visibles.Contains(sen.Id))
                                                                    select new
                                                                    {
                                                                        id = sen.Id.ToString("N"),
                                                                        name = sen.Name,
                                                                        edit = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                                                        time = sen.Time.ToString(),
                                                                        value = sen.Value.ToString("0.####"),
                                                                        unit = sen.Unit
                                                                    })
                                                     }),
                                          sensors = (from sen in grp.Sensors.Values.Where(x => x.Device == null).OrderBy(x => x.Name)
                                                     where (fullmode || grp.Visibles.Contains(sen.Id))
                                                     select new
                                                     {
                                                         id = sen.Id.ToString("N"),
                                                         name = sen.Name,
                                                         edit = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                                         time = sen.Time.ToString(),
                                                         value = sen.Value.ToString("0.####"),
                                                         unit = sen.Unit
                                                     })
                                      })
                        };
                    }
                    else if (groupid == null)
                    {
                        offset = 0;
                        json.Data = new
                        {
                            institute = ins.Id.ToString("N"),
                            type = ins.Type.ToString(),
                            time = DateTime.Now.ToString(),
                            groups = (from grp in ins.Groups.Values.OrderBy(x => x.Name)
                                      select new
                                      {
                                          id = grp.Id.ToString("N"),
                                          name = grp.Name,
                                          offset = offset,
                                          limit = limit,
                                          count = grp.Visibles.Count,
                                          sensors = (from sen in grp.Sensors.Values.OrderBy(x => x.Name)
                                                     where (fullmode || grp.Visibles.Contains(sen.Id))
                                                     select new
                                                     {
                                                         id = sen.Id.ToString("N"),
                                                         name = sen.Name,
                                                         edit = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                                         time = sen.Time.ToString(),
                                                         value = sen.Value.ToString("0.####"),
                                                         unit = sen.Unit
                                                     }).Skip(offset).Take(limit)
                                      })
                        };
                    }
                    else if (ins.Groups.ContainsKey(groupid))
                    {
                        Group grp = ins.Groups[groupid];
                        if (offset >= grp.Sensors.Count)
                        {
                            offset = (grp.Sensors.Count % limit == 0 ? grp.Sensors.Count / limit - 1 : grp.Sensors.Count / limit) * limit;
                        }
                        json.Data = new
                        {
                            institute = ins.Id.ToString("N"),
                            type = ins.Type.ToString(),
                            time = DateTime.Now.ToString(),
                            groups = new[]
                            {
                                new 
                                {
                                    id=grp.Id.ToString("N"),
                                    name=grp.Name,
                                    offset=offset,
                                    limit=limit,
                                    count=grp.Visibles.Count,
                                    sensors=(from sen in grp.Sensors.Values.OrderBy(x=>x.Name)
                                             where (fullmode||grp.Visibles.Contains(sen.Id))
                                             select new
                                             {
                                                 id=sen.Id.ToString("N"),
                                                 name=sen.Name,
                                                 edit = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                                                 time=sen.Time.ToString(),
                                                 value=sen.Value.ToString("0.####"),
                                                 unit=sen.Unit
                                             }).Skip(offset).Take(limit)
                                }
                            }
                        };
                    }
                }
                else
                {
                    json.Data = new
                    {
                        institute = insid.ToString("N") + " Not Found!"
                    };
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
            if (id.HasValue && DbProvider.Sensors.ContainsKey(id.Value.ToString("N")))
            {
                Sensor sen = DbProvider.Sensors[id.Value.ToString("N")];
                json.Data = new
                {
                    id = sen.Id.ToString("N"),
                    srvtime = DateTime.Now.ToString(),
                    name = sen.Name,
                    edit = (sen.Source == Sen_Source.Manual || sen.Source == Sen_Source.Auto),
                    time = sen.Time.ToString(),
                    value = sen.Value.ToString("0.####"),
                    unit = sen.Unit,
                    ins = new
                    {
                        id = sen.Institute.ToString("N"),
                        name = DbProvider.Institutes[sen.Institute.ToString("N")].Name,
                        type = DbProvider.Institutes[sen.Institute.ToString("N")].Type.ToString()
                    }
                };
            }
            return json;
        }

        public JsonResult Track(Guid? id)
        {
            Int32 level = 0;
            Int32.TryParse(Request.Params["level"], out level);
            level = Math.Min(5, Math.Max(level, 0));
            DateTime time = DateTime.Now;
            DateTime.TryParseExact(Request.Params["time"], "yyyyMMddHHmm", new CultureInfo("en-US"), DateTimeStyles.None, out time);
            if (time > DateTime.Now)
            {
                time = DateTime.Now;
            };
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id.HasValue && DbProvider.Sensors.ContainsKey(id.Value.ToString("N")))
            {
                Sensor sen = DbProvider.Sensors[id.Value.ToString("N")];
                json.Data = new
                {
                    level = level,
                    his = (from rec in sen.Track(level, time)
                           select new
                           {
                               value = rec.Value,
                               time = rec.Time.ToString()
                           })
                };
            }
            return json;
        }

        [AllowAnonymous]
        public JsonResult Push(Guid? id)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id.HasValue)
            {
                String clientid = id.Value.ToString("N");
                if (DbProvider.Clients.ContainsKey(clientid))
                {
                    Client client = DbProvider.Clients[clientid];
                    Double val;
                    if (Request.Form["longitude"] != null && Request.Form["latitude"] != null)
                    {
                        Double lon, lat;
                        if (Double.TryParse(Request.Form["longitude"], out lon) &&
                            Double.TryParse(Request.Form["latitude"], out lat) &&
                            (lon != 0 || lat != 0))
                        {
                            DbProvider.Clients.Move(client.Id, lon, lat);
                        }
                    }
                    foreach (String key in Request.Form.AllKeys)
                    {
                        if (client.Sensors.ContainsKey(key) && Double.TryParse(Request.Form[key], out val))
                        {
                            client.Sensors[key].Sensor.Value = val;
                        }
                    }
                    client.LastActive = DateTime.Now;
                }
            }
            return json;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.OperatorAbove)]
        public JsonResult Set(Guid? id)
        {
            Double val = 0;
            if (id.HasValue && Double.TryParse(Request.Params["value"], out val))
            {
                Sensor sen = DbProvider.Sensors[id.Value.ToString("N")];
                if (sen != null && sen.Source != Sen_Source.Client)
                {
                    sen.Value = val;
                    return Json(new { result = "ok", msg = "仪表数值已更新" });
                }
                return Json(new { result = "error", msg = "仪表不存在或不可手动设置数值" });
            }
            return Json(new { result = "error", msg = "未知参数错误" });
        }

        #endregion
    }
}
