using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KPax.DataModels;

namespace KPax.Models
{
    public class TeamCityViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "URI")]
        public string URI { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Project")]
        public Project Project { get; set; }

        [Required]
        [Display(Name = "Build id")]
        public string BuildId { get; set; }

        [NotMapped]
        public SelectList ProjectList { get; set; }

        [NotMapped]
        public SelectList BuildIdsList { get; set; } 
    }
}