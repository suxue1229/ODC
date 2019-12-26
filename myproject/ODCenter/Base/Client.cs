using ODCenter.Models;
using PTR.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ODCenter.Base
{
    public class Client
    {
        public Guid Id { get; set; }
        public Int32? Index { get; set; }
        public Int32? Dtu { get; set; }
        public String Name { get; set; }
        public Guid Institute { get; set; }
        public Double? Longitude { get; set; }
        public Double? Latitude { get; set; }
        public SortedList<String, SensorEx> Sensors { get; set; }
        public DateTime LastActive { get; set; }
        public String opc_hostaddr;

        private String _configText;
        private String _configHash;

        public Client()
        {
            Sensors = new SortedList<String, SensorEx>();
        }
        public Client(ClientInfo client)
            : this()
        {
            this.Id = client.Id;
            this.Index = client.Index;
            this.Dtu = client.Dtu;
            this.Name = client.Name;
            this.Institute = client.Institute;
            this.Longitude = client.Longitude;
            this.Latitude = client.Latitude;
            this.LastActive = client.LastActive;
        }

        Boolean _cfginvalid = false;

        public void Invalidate()
        {
            this._cfginvalid = true;
        }

        public String ConfigText(SortedList<String, Client> opc_clients)
        {
            String url = new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Push", "Sensor", null,
                protocol: HttpContext.Current.Request.Url.Scheme);
            opc_hostaddr = url;
            return ConfigText(url, this._cfginvalid, opc_clients);
        }


        private String _hostaddr;
        //public String ConfigText(String url, Boolean invalid)
        //{
        //    if (invalid || String.IsNullOrWhiteSpace(this._configText) || _hostaddr != url)
        //    {
        //        _hostaddr = url;
        //        var json = new
        //        {
        //            host = url,
        //            id = this.Id.ToString("N"),
        //            institute = this.Institute.ToString("N"),
        //            index = this.Index,
        //            dtu = this.Dtu.ToString(),
        //            name = this.Name,
        //            sensors = (from sensor in this.Sensors.Values
        //                       select new
        //                       {
        //                           id = sensor.Sensor.Id.ToString("N"),
        //                           index = sensor.Index,
        //                           name = sensor.Sensor.Name,
        //                           host = sensor.Sensor.OPCHost,
        //                           server = sensor.Sensor.OPCServer,
        //                           item = sensor.Sensor.OPCItem,
        //                           interval = sensor.Sensor.Interval,
        //                           gain = sensor.Sensor.Gain,
        //                           offset = sensor.Sensor.Offset,
        //                           unit = sensor.Sensor.Unit
        //                       }),

        //        };
        //        this._configHash = null;
        //        this._configText = new JavaScriptSerializer().Serialize(json);
        //        Byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(this._configText));
        //        this._configHash = hash.ToString(true);
        //    }
        //    return this._configText;
        //}
        public String ConfigText(String url, Boolean invalid, SortedList<String, Client> opc_clients)
        {
            if (invalid || String.IsNullOrWhiteSpace(this._configText) || _hostaddr != url)
            {
                _hostaddr = url;
                var json = new
                {
                    config = (from client in opc_clients.Values
                              select new
                              {
                                  host = opc_hostaddr,
                                  id = client.Id.ToString("N"),
                                  institute = client.Institute.ToString("N"),
                                  index = client.Index,
                                  dtu = client.Dtu.ToString(),
                                  name = client.Name,
                                  sensors = (from sensor in client.Sensors.Values
                                             select new
                                             {
                                                 id = sensor.Sensor.Id.ToString("N"),
                                                 index = sensor.Index,
                                                 name = sensor.Sensor.Name,
                                                 host = sensor.Sensor.OPCHost,
                                                 server = sensor.Sensor.OPCServer,
                                                 item = sensor.Sensor.OPCItem,
                                                 interval = sensor.Sensor.Interval,
                                                 gain = sensor.Sensor.Gain,
                                                 offset = sensor.Sensor.Offset,
                                                 unit = sensor.Sensor.Unit
                                             }),
                              })

                };
                this._configHash = null;
                this._configText = new JavaScriptSerializer().Serialize(json);
                Byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(this._configText));
                this._configHash = hash.ToString(true);
            }
            return this._configText;
        }

        public String ConfigHash(SortedList<String, Client> opc_clients)
        {
            ConfigText(opc_clients);
            return this._configHash;
        }
    }

    public class SensorEx
    {
        public Sensor Sensor;
        public Int32? Index;

        public SensorEx(Sensor sensor, Int32? index = null)
        {
            Sensor = sensor;
            Index = index;
        }
    }
    

}