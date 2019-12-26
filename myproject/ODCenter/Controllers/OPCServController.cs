using ODCenter.Base;
using ODCenter.Models;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class OPCServController : Controller
    {
        public ActionResult Index()
        {
            return View(new OPCServerInfo());
        }

        public ActionResult Connect()
        {
            return View(new OPCServerInfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Connect([Bind(Include = "id,IP,OPCServerName")] OPCServerInfo institute)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Connect");
            }
            return View(institute);
        }


    }
}