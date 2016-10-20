using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class AuditsViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Required]
        [Display(Name = "Number")]
        public int ReportNumber { get; set; }

        [Required]
        [Display(Name = "Scheduled date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime ScheduledDate { get; set; }

        [Display(Name = "Performed date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> PerformedDate { get; set; }

        [Required]
        [Display(Name = "Auditor name")]
        public string Auditor { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "Bsaseline audit")]
        public Nullable<int> BaselineId { get; set; }

        public virtual BaselinePlan BaselinePlan { get; set; }
        public virtual Project Project { get; set; }

    }
}