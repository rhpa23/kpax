using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class BaselineFlowListViewModel
    {
        public Project Project { get; set; }
        public BaselineFlowViewModel BaselineFlow { get; set; }
        public IEnumerable<BaselineFlow> AvailableBaselineFlow { get; set; }
        public IEnumerable<BaselineFlow> SelectedBaselineFlow { get; set; }
        public PostedBaselineFlow PostedBaselineFlows { get; set; }
    }
}