using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ODCenter.Base
{
    public class OPCServ
    {

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "IP")]
        public String IP { get; set; }

        [JsonProperty(PropertyName = "OPCServerName")]
        public String OPCServerName { get; set; }

       
        [JsonProperty(PropertyName = "clients", NullValueHandling = NullValueHandling.Ignore)]
        public SortedList<String, Client> Clients { get; set; }

        [JsonProperty(PropertyName = "groups", NullValueHandling = NullValueHandling.Ignore)]
        public SortedList<String, Group> Groups { get; set; }

        [JsonProperty(PropertyName = "reports", NullValueHandling = NullValueHandling.Ignore)]
        public SortedList<String, String> Reports { get; set; }

        public OPCServ()
        {
            Clients = new SortedList<String, Client>();
            Groups = new SortedList<String, Group>();
            Reports = new SortedList<String, String>();
        }

        public OPCServ(OPCServ institute)
            : this()
        {
            this.Id = institute.Id;
            this.IP = institute.IP;
            this.OPCServerName = institute.OPCServerName;
        }

      
    }
}