using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ODCenter.Base
{
    public class DevTrack
    {
        public Int32 Id { get; set; }
        public String Log { get; set; }
        public DateTime Time { get; set; }
        public String Operator { get; set; }
    }

    public class Device
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public Guid Institute { get; set; }
        public Dev_Stat Statsrc { get; set; }
        public String Status { get; set; }
        public SortedList<String, Sensor> Sensors { get; set; }

        public Device()
        {
            Sensors = new SortedList<String, Sensor>();
        }

        public Device(DeviceInfo device)
            : this()
        {
            this.Id = device.Id;
            this.Name = device.Name;
            this.Institute = device.Institute;
            this.Statsrc = device.Statsrc;
            this.Status = device.Status;
        }

        public void UpdateStatus()
        {
            if (this.Statsrc == Dev_Stat.Manual || this.Statsrc == Dev_Stat.None)
            {
                return;
            }
            String prefix = "";
            foreach (Sensor s in this.Sensors.Values.Where(s => s.DatType == Dat_Type.BOOL).OrderByDescending(s => s.SenType))
            {
                switch (s.SenType)
                {
                    case Sen_Type.Status_Error:
                        if ((s.Value != 0) ^ s.Inverse)
                        {
                            this.Status = "{red}故障";
                            return;
                        }
                        else
                        {
                            this.Status = "{green}正常";
                        }
                        break;
                    case Sen_Type.Status_Manual:
                        if ((s.Value != 0) ^ s.Inverse)
                        {
                            prefix = "手动";
                        }
                        else
                        {
                            prefix = "自动";
                        }
                        break;
                    case Sen_Type.Status_Run:
                        if ((s.Value != 0) ^ s.Inverse)
                        {
                            this.Status = String.Format("{{green}}{0}运行", prefix);
                        }
                        else
                        {
                            this.Status = String.Format("{{red}}{0}停止", prefix);
                        }
                        return;
                    case Sen_Type.Status_Open:
                        if ((s.Value != 0) ^ s.Inverse)
                        {
                            this.Status = String.Format("{{green}}{0}打开", prefix);
                        }
                        else
                        {
                            this.Status = String.Format("{{red}}{0}关闭", prefix);
                        }
                        return;
                }
            }
        }

        public IEnumerable<DevTrack> Track
        {
            get
            {
                using (var db_dev = new DeviceDbContext())
                {
                    return (from trk in db_dev.Tracks
                            where (trk.DeviceId == this.Id && trk.Enabled == true)
                            orderby trk.Id
                            select new DevTrack()
                            {
                                Id = trk.Id,
                                Time = trk.Time,
                                Log = trk.Description,
                                Operator = trk.Operator
                            });
                }
            }
        }

        public static HtmlString WebStatus(String text)
        {
            try
            {
                Match mat = new Regex(@"{([^}]*)}(.*)").Match(text);
                if (mat.Success)
                {
                    return new HtmlString(String.Format("<span style=\"color:{0};\">{1}</span>", mat.Groups[1].Value, mat.Groups[2].Value));
                }
            }
            catch { }
            return new HtmlString(text);
        }
    }
}