using ODCenter.Base;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    [Authorize]
    public class NavController : Controller
    {
        public JsonResult Index()
        {
            Guid? insid = Account.GetInstitute();
            if (insid.HasValue)
            {
                JsonResult json = new JsonResult();
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                json.Data = new object[]
                {
                    new
                    {
                        name="实时监控",
                        controller="Monitor",
                        icon="glyphicon-eye-open",
                        subs=(from grp in DbProvider.Institutes[insid.Value.ToString("N")].Groups.Values.OrderBy(x=>x.Name)
                              select new 
                              {
                                  id=grp.Id.ToString("N"),
                                  name=grp.Name,
                                  count=grp.Sensors.Count
                              })
                    },
                    new
                    {
                        name="能耗分析",
                        controller="Analysis",
                        icon="glyphicon-tasks",
                        subs=(from grp in DbProvider.Institutes[insid.Value.ToString("N")].Groups.Values.OrderBy(x=>x.Name)
                              select new 
                              {
                                  id=grp.Id.ToString("N"),
                                  name=grp.Name,
                                  count=grp.Sensors.Count
                              })
                    },
                    new
                    {
                        name="报表系统",
                        controller="Report",
                        icon="glyphicon-list-alt",
                        subs=new Object[0]
                        //(from grp in DbProvider.Institutes[insid.ToString("N")].Groups.Values.OrderBy(x=>x.Name)
                        //      select new 
                        //      {
                        //          id=grp.Id.ToString("N"),
                        //          name=grp.Name,
                        //          count=grp.Sensors.Count
                        //      })
                    },
                    new
                    {
                        name="设备管理",
                        controller="Manage",
                        icon="glyphicon-cog",
                        subs=(from grp in DbProvider.Institutes[insid.Value.ToString("N")].Groups.Values.OrderBy(x=>x.Name)
                              select new 
                              {
                                  id=grp.Id.ToString("N"),
                                  name=grp.Name,
                                  count=grp.Sensors.Count
                              })
                    }
                };
                return json;
            }
            return null;
        }
    }
}
