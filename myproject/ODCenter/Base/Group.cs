using ODCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODCenter.Base
{
    public class Group
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public Guid Institute { get; set; }
        public List<Guid> Visibles { get; set; }
        public SortedList<String, Graph> Graphs { get; set; }
        public SortedList<String, Sensor> Sensors { get; set; }
        public SortedList<String, Device> Devices { get; set; }

        public Group()
        {
            Visibles = new List<Guid>();
            Graphs = new SortedList<String, Graph>();
            Sensors = new SortedList<String, Sensor>();
            Devices = new SortedList<String, Device>();
        }

        public Group(GroupInfo group)
            : this()
        {
            this.Id = group.Id;
            this.Name = group.Name;
            this.Institute = group.Institute;
        }
    }
}