using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODCenter.Base
{
    public class Graph
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public Guid Institute { get; set; }
        public SortedList<String, Sensor> Sensors { get; set; }

        public Graph()
        {
            Sensors = new SortedList<String, Sensor>();
        }
    }
}