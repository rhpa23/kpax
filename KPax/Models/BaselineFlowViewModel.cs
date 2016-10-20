using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class BaselineFlowViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project ")]
        public Project Project { get; set; }

        [Required]
        [Display(Name = "Baseline  ")]
        public BaselineFlow BaselineFlow { get; set; }

        public string Status { get; set; }
    }
}