using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace ODClient
{
    public class OPCData
    {
        public String Id, Name, Unit;
        public String Host, Server, Item;
        public Int32 Handle, Interval = 1;
        public Double Gain = 1, Offset = 0;

        private Double _opcvalue;
        private Int32 _quality;
        private DateTime _timestamp;
        private Boolean _isvalid = false;

        public Double OPCValue
        {
            get
            {
                return this._opcvalue;
            }
        }

        public Double PhyValue
        {
            get
            {
                return this._opcvalue * Gain + Offset;
            }
        }

        public Int32 Quality
        {
            get
            {
                return this._quality;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return this._timestamp;
            }
        }

        public Boolean Isvalid
        {
            get
            {
                if(_isvalid)
                {
                    this._isvalid = false;
                    return true;
                }
                return false;
            }
        }

        public void SetOpcValue(Double value, DateTime time)
        {
            this._opcvalue = value;
            this._timestamp = time;
            this._isvalid = true;
        }

        public void SetOpcValue(Double value, Int32 quality, DateTime time)
        {
            this._opcvalue = value;
            this._quality = quality;
            this._timestamp = time;
            this._isvalid = true;
        }

        public static OPCData Parse(JToken json)
        {
            try
            {
                String host = json["host"].Value<String>();
                if (String.IsNullOrWhiteSpace(host) || String.Compare(host, "local", true) == 0 || String.Compare(host, "localhost") == 0)
                {
                    host = Dns.GetHostName();
                }
                return new OPCData()
                {
                    Id = json["id"].Value<String>(),
                    Name = json["name"].Value<String>(),
                    Host = host,
                    Server = json["server"].Value<String>(),
                    Item = json["item"].Value<String>(),
                    Interval = (json["interval"] != null) ? json["interval"].Value<Int32>() : 1,
                    Gain = (json["gain"] != null) ? json["gain"].Value<Double>() : 1,
                    Offset = (json["offset"] != null) ? json["offset"].Value<Double>() : 0,
                    Unit = (json["unit"] != null) ? json["unit"].Value<String>() : String.Empty
                };
            }
            catch { }
            return null;
        }
    }
}
