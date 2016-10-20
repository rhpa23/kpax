using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KPax.Util
{
    public class TeamCityBuildTO
    {
        public int Number { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public DateTime StartDate { get; set; }
        public string StatusText { get; set; }
    }
}