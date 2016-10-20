using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class HomeViewModel
    {
        [Display(Name = "Project ")]
        public Project Project { get; set; }
    }
}