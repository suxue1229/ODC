using ODCenter.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace ODCenter.Controllers
{
    public class UpdateController : Controller
    {
        public SortedList<String, Client> opc_clients= DbProvider.OPCClients();
        public JsonResult Check(Guid? id)
        {
            JsonResult res = new JsonResult();
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            Client client = null;
            if (id.HasValue && (client = DbProvider.Clients[id.Value.ToString("N")]) != null)
            {
                res.Data = new
                {
                    config = client.ConfigHash(opc_clients),
                    client = Update.ClientHash,
                    update = Update.UpdateHash
                };
            }
            return res;
        }

        //public ContentResult Config(Guid? id)
        //{
        //    Client client = null;
        //    if (id.HasValue)
        //    {
        //        client = DbProvider.Clients[id.Value.ToString("N")];
        //    }
        //    else
        //    {
        //        Int32 plcid, dtuid;
        //        if (Int32.TryParse(Request.Form["plcid"], out plcid))
        //        {
        //            client = DbProvider.Clients.Find(plcid);
        //        }
        //        if (client != null && client.Dtu.HasValue && !String.IsNullOrWhiteSpace(Request.Form["dtuid"]))
        //        {
        //            if (!(Int32.TryParse(Request.Form["dtuid"], out dtuid) && client.Dtu == dtuid))
        //            {
        //                client = null;
        //            }
        //        }
        //    }
        //    if (client != null)
        //    {
        //        return this.Content(client.ConfigText(), "application/json");
        //    }
        //    return this.Content(String.Empty);
        //}
        public ContentResult Configs()
        {
            Client client = new Client();
            return this.Content(client.ConfigText(opc_clients), "application/json");
        }

        public FileStreamResult Client(Guid? id)
        {
            String file = Server.MapPath("/Update/Update.exe");
            if (id.HasValue && System.IO.File.Exists(file) && DbProvider.Clients.ContainsKey(id.Value.ToString("N")))
            {
                FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return new FileStreamResult(stream, "application/x-msdownload");
            }
            return null;
        }
    }
}