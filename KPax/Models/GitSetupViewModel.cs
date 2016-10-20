using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KPax.DataModels;

namespace KPax.Models
{
    public class GitSetupViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Local repository path")]
        public string RepositoryPath { get; set; }

        [Required]
        [Display(Name = "Project")]
        public Project Project { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}