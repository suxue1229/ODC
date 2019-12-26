using Newtonsoft.Json;
using ODCenter.Models;
using System;
using System.Collections.Generic;

namespace ODCenter.Base
{
    public class Institute
    {
        public static List<Guid> Excludes = new List<Guid>();

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public Ins_Type Type { get; set; }

        [JsonProperty(PropertyName = "location")]
        public String Location { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public Double? Longitude { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public Double? Latitude { get; set; }

        [JsonProperty(PropertyName = "address")]
        public String Address { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public String Summary { get; set; }

        [JsonProperty(PropertyName = "clients", NullValueHandling = NullValueHandling.Ignore)]
        public SortedList<String, Client> Clients { get; set; }

        [JsonProperty(PropertyName = "groups", NullValueHandling = NullValueHandling.Ignore)]
        public SortedList<String, Group> Groups { get; set; }

        [JsonProperty(PropertyName = "reports", NullValueHandling = NullValueHandling.Ignore)]
        public SortedList<String, String> Reports { get; set; }

        public Institute()
        {
            Clients = new SortedList<String, Client>();
            Groups = new SortedList<String, Group>();
            Reports = new SortedList<String, String>();
        }

        public Institute(InstituteInfo institute)
            : this()
        {
            this.Id = institute.Id;
            this.Name = institute.Name;
            this.Type = institute.Type;
            this.Location = institute.Location;
            this.Longitude = institute.Longitude;
            this.Latitude = institute.Latitude;
            this.Address = institute.Address;
            this.Summary = institute.Summary;
        }
    }
}