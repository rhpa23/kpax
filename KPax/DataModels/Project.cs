//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KPax.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class Project
    {
        public Project()
        {
            this.BaselineFlowProject = new HashSet<BaselineFlowProject>();
            this.BaselinePlan = new HashSet<BaselinePlan>();
            this.GitSetup = new HashSet<GitSetup>();
            this.ProjectUser = new HashSet<ProjectUser>();
            this.TeamCitySetup = new HashSet<TeamCitySetup>();
            this.Audits = new HashSet<Audits>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Acronym { get; set; }
        public string TemplateEmail { get; set; }
        public string EmailCM { get; set; }
    
        public virtual ICollection<BaselineFlowProject> BaselineFlowProject { get; set; }
        public virtual ICollection<BaselinePlan> BaselinePlan { get; set; }
        public virtual ICollection<GitSetup> GitSetup { get; set; }
        public virtual ICollection<ProjectUser> ProjectUser { get; set; }
        public virtual ICollection<TeamCitySetup> TeamCitySetup { get; set; }
        public virtual ICollection<Audits> Audits { get; set; }
    }
}
