using ODCenter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class RouterController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Conn(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                ViewData["id"] = id.Value.ToString("N");
                return View();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Detail(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                ViewData["id"] = id.Value.ToString("N");
                return View();
            }
            return RedirectToAction("Index");
        }

        public ContentResult Link(Guid? id)
        {
            String url = Router.Combine(String.Format("/link/list/{0}", id.HasValue && id.Value != Guid.Empty ? id.Value.ToString("N") : ""));
            return new ContentResult() { Content = Router.HttpGet(url), ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
        }

        public ContentResult Close(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
            {
                return new ContentResult() { Content = String.Empty };
            }
            String url = Router.Combine(String.Format("/link/close/{0}", id.Value.ToString("N")));
            return new ContentResult() { Content = Router.HttpGet(url), ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
        }

        public ContentResult Data(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty || Request.Form.Count == 0)
            {
                return new ContentResult() { Content = String.Empty };
            }
            String url = Router.Combine(String.Format("/data/set/{0}?{1}", id.Value.ToString("N"), Router.ParamString(Request.Form)));
            return new ContentResult() { Content = Router.HttpGet(url), ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
        }

        public ContentResult Stat(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty || Request.Form.Count == 0)
            {
                return new ContentResult() { Content = String.Empty };
            }
            String url = Router.Combine(String.Format("/status/set/{0}?{1}", id.Value.ToString("N"), Router.ParamString(Request.Form)));
            return new ContentResult() { Content = Router.HttpGet(url), ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
        }

        public ContentResult Func(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty || Request.Form.Count == 0)
            {
                return new ContentResult() { Content = String.Empty };
            }
            String url = Router.Combine(String.Format("/function/set/{0}?{1}", id.Value.ToString("N"), Router.ParamString(Request.Form)));
            return new ContentResult() { Content = Router.HttpGet(url), ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
        }
    }
}