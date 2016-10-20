using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class BaselineFlowProcessViewModel
    {
        public IEnumerable<BaselineFlow> BaselineFlow { get; set; }

        public BaselinePlan BaselinePlan { get; set; }
    }
}