using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class BaselinePlanAuditViewModel
    {
        public int Id { get; set; }

        public int BaselinePlanId { get; set; }
        public string[] Values { get; set; }

        public bool Verified { get; set; }

        public List<BaselineAudit> BaselineAudits { get; set; }

        public List<BaselineAudit> SelectedBaselineAudits { get; set; }
    }
}