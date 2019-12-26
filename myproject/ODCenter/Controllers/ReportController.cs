using Microsoft.Reporting.WebForms;
using ODCenter.Base;
using ODCenter.Models;
using PTR.Logging;
using PTR.Reporting;
using PTR.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    //报表系统
    [Authorize]
    public class ReportController : Controller
    {
        private ReportDbContext db_rpt = new ReportDbContext();
        private String rpt_path = "/Resource/Report/";

        public ActionResult Index()
        {
            Guid? insid = Account.GetInstitute();
            if (insid.HasValue)
            {
                ViewBag.Title = "报表系统 - " + DbProvider.Institutes[insid.Value.ToString("N")].Name;
                ViewData["controller"] = "Report";
                return View(DbProvider.Institutes[insid.Value.ToString("N")].Reports.OrderBy(x => x.Value));
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Detail(Guid? id)
        {
            if (id.HasValue && DbProvider.Sensors.ContainsKey(id.Value.ToString("N")))
            {
                Sensor sensor = DbProvider.Sensors[id.Value.ToString("N")];
                ViewBag.Title = "报表系统 - " + sensor.Name;
                ViewData["controller"] = "Report";
                ViewData["sensor"] = sensor.Id.ToString("N");
                return View("Detail");
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = UserRoles.EngineerAbove)]
        public FileResult Download(Guid? id)
        {
            if (id.HasValue)
            {
                ReportInfo report = db_rpt.Reports.Find(id.Value);
                String filepath = Server.MapPath(String.Format(rpt_path + "{0}.rdlc", id.Value.ToString("N")));
                if (report != null && System.IO.File.Exists(filepath))
                {
                    String filename = report.Name + ".rdlc";
                    if (DbProvider.Institutes.ContainsKey(report.Institute.ToString("N")))
                    {
                        filename = DbProvider.Institutes[report.Institute.ToString("N")].Name + "-" + filename;
                    }
                    return File(filepath, "application/x-msdownload", filename);
                }
            }
            return null;
        }

        [Authorize(Roles = UserRoles.EngineerAbove)]
        public JsonResult Upload(Guid? id)
        {
            try
            {
                if (id.HasValue)
                {
                    if (!System.IO.Directory.Exists(Server.MapPath(rpt_path)))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath(rpt_path));
                    }
                    String filepath = Server.MapPath(String.Format(rpt_path + "{0}.rdlc", id.Value.ToString("N")));
                    String lastpath = Server.MapPath(String.Format(rpt_path + "{0}_old.rdlc", id.Value.ToString("N")));
                    HttpPostedFileBase file = Request.Files["upload"];
                    if (file == null)
                    {
                        return Json(new { result = false, msg = "No file is submitted." });
                    }
                    Int32 sizelimit = 1024 ^ 2;
                    Int32.TryParse(ConfigurationManager.AppSettings["report:maxsize"], out sizelimit);
                    if (file.ContentLength > sizelimit)
                    {
                        return Json(new { result = false, msg = "File Oversize." });
                    }
                    try
                    {
                        if (System.IO.File.Exists(lastpath))
                        {
                            System.IO.File.Delete(lastpath);
                        }
                        if (System.IO.File.Exists(filepath))
                        {
                            System.IO.File.Move(filepath, lastpath);
                        }
                    }
                    catch (Exception fex)
                    {
                        Logger.LogError("Error occurred when manage template files.", fex);
                    }
                    file.SaveAs(filepath);
                    return Json(new
                    {
                        result = true,
                        msg = "Upload Complete.",
                        file = System.IO.File.Exists(filepath) ?
                            String.Format("Upload at {0:yyyy-MM-dd HH:mm:ss}.", System.IO.File.GetLastWriteTime(filepath)) : "Not uploaded."
                    });
                }
                return Json(new { result = false, msg = "Unknown Report ID." });
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when upload report template.", ex);
                return Json(new { result = false, msg = "Unknown Error." });
            }
        }

        [Authorize(Roles = UserRoles.OperatorAbove)]
        public FileContentResult PDF(Guid? id)
        {
            if (id.HasValue)
            {
                return Generate(id.Value, OutputType.PDF, User.Identity.Name);
            }
            return null;
        }

        [Authorize(Roles = UserRoles.EngineerAbove)]
        public FileContentResult Excel(Guid? id)
        {
            if (id.HasValue)
            {
                return Generate(id.Value, OutputType.EXCEL, User.Identity.Name);
            }
            return null;
        }

        private FileContentResult Generate(Guid reportId, OutputType reportType, String userName)
        {
            try
            {
                String template = Server.MapPath(String.Format(rpt_path + "{0}.rdlc", reportId.ToString("N")));
                if (!System.IO.File.Exists(template))
                {
                    throw new Exception("报告模板不存在。");
                }
                ReportInfo report = db_rpt.Reports.Find(reportId);
                if (report == null)
                {
                    throw new Exception("报表不存在。");
                }
                else
                {
                    String mimeType, encoding, fileExtension;
                    String fileName = String.Format("{0}_{1}", report.Name, DateTime.Now.ToString("yyyyMMdd"));
                    PageInfo pageInfo = String.IsNullOrWhiteSpace(report.PageInfo) ? new PageInfo() : PageInfo.Parse(report.PageInfo);
                    pageInfo.OutputFormat = reportType;
                    List<ReportDataSource> datas = new List<ReportDataSource>();
                    DataSource source = DataSource.Parse(report.DataSource);
                    if (source == null)
                    {
                        throw new Exception("报表数据源配置错误。");
                    }
                    DateTime? rptTime = null;
                    try
                    {
                        String par = Request.Params["time"].Trim();
                        if (!String.IsNullOrWhiteSpace(par) && par.Length == 12)
                        {
                            rptTime = DateTime.Parse(par.Substring(0, 4) + "-" + par.Substring(4, 2) + "-" +
                                par.Substring(6, 2) + " " + par.Substring(8, 2) + ":" + par.Substring(10, 2));
                        }
                    }
                    catch { }
                    if (rptTime.HasValue)
                    {
                        foreach (DataItem itm in source.Items)
                        {
                            if (!itm.Time.HasValue)
                            {
                                itm.Time = rptTime.Value;
                            }
                        }
                    }
                    foreach (DataTable dt in Sensor.Track(source))
                    {
                        datas.Add(new ReportDataSource(dt.TableName, dt));
                    }
                    Dictionary<String, String> prop = new Dictionary<String, String>();
                    prop.Add("ReportName", report.Name);
                    prop.Add("ReportUser", userName);
                    byte[] result = PTR.Reporting.WebForms.Report.Generate(template, datas.ToArray(), prop, pageInfo,
                        out mimeType, out encoding, out fileExtension);
                    ReportLog log = db_rpt.RenderLog(report, reportType.ToString(), userName);
                    if (log != null)
                    {
                        db_rpt.Logs.Add(log);
                        db_rpt.SaveChanges();
                    }
                    return File(result, mimeType, String.Format("{0}.{1}", fileName, fileExtension));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when generating report.", ex);
                throw new Exception("报表数据源或模板配置错误。");
            }
        }

        public ActionResult List(Guid? id)
        {
            if (id.HasValue)
            {
                ViewData["institute"] = id;
                return View(db_rpt.Reports.Where(x => x.Institute == id.Value).ToList());
            }
            else
            {
                return View(db_rpt.Reports.ToList());
            }
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportInfo reportInfo = db_rpt.Reports.Find(id);
            if (reportInfo == null)
            {
                return HttpNotFound();
            }
            return View(reportInfo);
        }

        public ActionResult Create(Guid? id)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                return View();
            }
            return RedirectToAction("List", new { id = id });
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Guid? id, [Bind(Include = "Name")] ReportInfo report)
        {
            if (id.HasValue && DbProvider.Institutes.ContainsKey(id.Value.ToString("N")))
            {
                if (ModelState.IsValid)
                {
                    report.Id = Guid.NewGuid();
                    report.Institute = id.Value;
                    report.PageInfo = String.Empty;
                    report.DataSource = String.Empty;
                    report.Enabled = true;
                    report.Modifier = User.Identity.Name;
                    ReportLog log = db_rpt.CreateLog(report);
                    if (log != null)
                    {
                        db_rpt.Logs.Add(log);
                    }
                    db_rpt.Reports.Add(report);
                    db_rpt.SaveChanges();
                    DbProvider.Institutes[report.Institute.ToString("N")].Reports.Add(report.Id.ToString("N"), report.Name);
                    return RedirectToAction("Edit", new { id = report.Id.ToString("N") });
                }
                return View(report);
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
            ReportInfo report = db_rpt.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            String filepath = Server.MapPath(String.Format(rpt_path + "{0}.rdlc", id.Value.ToString("N")));
            ViewData["file"] = System.IO.File.Exists(filepath) ?
                String.Format("Upload at {0:yyyy-MM-dd HH:mm:ss}.", System.IO.File.GetLastWriteTime(filepath)) : "Not uploaded.";

            return View(report);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,PageInfo,DataSource")] ReportInfo newreport)
        {
            if (ModelState.IsValid)
            {
                ReportInfo orireport = db_rpt.Reports.Find(newreport.Id);
                if (orireport != null && (orireport.Name != newreport.Name ||
                    orireport.PageInfo != newreport.PageInfo || orireport.DataSource != newreport.DataSource))
                {
                    orireport.Modifier = User.Identity.Name;
                    ReportLog log = db_rpt.EditLog(orireport, newreport);
                    if (log != null)
                    {
                        db_rpt.Logs.Add(log);
                    }
                    orireport.Name = newreport.Name;
                    orireport.PageInfo = newreport.PageInfo;
                    orireport.DataSource = newreport.DataSource;
                    db_rpt.Entry(orireport).State = EntityState.Modified;
                    db_rpt.SaveChanges();

                    Institute ins = DbProvider.Institutes[orireport.Institute.ToString("N")];
                    if (ins != null && ins.Reports.ContainsKey(orireport.Id.ToString("N")))
                    {
                        ins.Reports[orireport.Id.ToString("N")] = orireport.Name;
                    }
                }
                return RedirectToAction("List", new { id = orireport.Institute.ToString("N") });
            }
            return View(newreport);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportInfo reportInfo = db_rpt.Reports.Find(id);
            if (reportInfo == null)
            {
                return HttpNotFound();
            }
            return View(reportInfo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ReportInfo report = db_rpt.Reports.Find(id);
            if (report != null)
            {
                report.Enabled = false;
                report.Modifier = User.Identity.Name;

                Institute ins = DbProvider.Institutes[report.Institute.ToString("N")];
                if (ins != null && ins.Reports.ContainsKey(report.Id.ToString("N")))
                {
                    ins.Reports.Remove(report.Id.ToString("N"));
                }

                ReportLog log = db_rpt.DeleteLog(report);
                if (log != null)
                {
                    db_rpt.Logs.Add(log);
                }
                db_rpt.Entry(report).State = EntityState.Modified;
                db_rpt.SaveChanges();

                return RedirectToAction("List", new { id = report.Institute.ToString("N") });
            }
            return RedirectToAction("List");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db_rpt.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}