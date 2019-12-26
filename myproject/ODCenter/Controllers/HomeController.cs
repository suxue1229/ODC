using ODCenter.Base;
using System;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous, RequireHttps]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                if (Account.GetInstitutes(true).Length > 1)
                {
                    return RedirectToAction("Index", "Map");
                }
                Guid? insid = Account.GetInstitute();
                if (insid.HasValue)
                {
                    String image = "/Images/Institute/" + insid.Value.ToString("N") + ".jpg";
                    if (System.IO.File.Exists(Server.MapPath(image)))
                    {
                        ViewData["img"] = image;
                        return View("Splash");
                    }
                }
                return RedirectToAction("Index", "Monitor");
            }
            else
            {
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}