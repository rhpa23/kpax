using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class BaselinePlanViewModel
    {
        [Required]
        [Display(Name = "Baseline Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Release date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime ReleaseDate { get; set; }

        [Required]
        [Display(Name = "Is this released?")]
        public bool IsReleased { get; set; }

        [Required]
        [Display(Name = "Project ")]
        public Project Project { get; set; }

        [Required]
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        [Display(Name = "Days to remember me")]
        public Nullable<int> RememberMeDays { get; set; }
    }
}