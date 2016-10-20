using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class AuditReportViewModel
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Audit")]
        public Audits Audit { get; set; }

        public List<AuditReport> AuditReports;

        public virtual BaselinePlan BaselinePlan { get; set; }
        public virtual Project Project { get; set; }

        public List<AuditReportResults> MyResults { get; set; }
    }
}