using ODCenter.Base;
using ODCenter.Models;
using System;
using System.Net;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class InstituteController : Controller
    {
        public ActionResult Index()
        {
            return View(DbProvider.Institutes.All());
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstituteInfo institute = DbProvider.Institutes.Find(id);
            if (institute == null)
            {
                return HttpNotFound();
            }
            return View(institute);
        }

        public ActionResult Create()
        {
            return View(new InstituteInfo());
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Type,Parent,Location,Longitude,Latitude,Summary")] InstituteInfo institute)
        {
            if (ModelState.IsValid)
            {
                DbProvider.Institutes.Create(institute,User.Identity.Name);
                return RedirectToAction("Index");
            }
            return View(institute);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstituteInfo institute = DbProvider.Institutes.Find(id);
            if (institute == null)
            {
                return HttpNotFound();
            }
            return View(institute);
        }

        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Type,Location,Longitude,Latitude,Summary")] InstituteInfo institute)
        {
            if (ModelState.IsValid)
            {
                DbProvider.Institutes.Update(institute,User.Identity.Name);

                return RedirectToAction("Index");
            }
            return View(institute);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstituteInfo institute = DbProvider.Institutes.Find(id);
            if (institute == null)
            {
                return HttpNotFound();
            }
            return View(institute);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DbProvider.Institutes.Delete(id, User.Identity.Name);
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
    }
}
