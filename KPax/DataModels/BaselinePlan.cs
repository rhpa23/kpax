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
    
    public partial class BaselinePlan
    {
        public BaselinePlan()
        {
            this.BaselinePlanAudit = new HashSet<BaselinePlanAudit>();
            this.Audits = new HashSet<Audits>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime ReleaseDate { get; set; }
        public bool IsReleased { get; set; }
        public int ProjectId { get; set; }
        public bool RememberMe { get; set; }
        public Nullable<int> RememberMeDays { get; set; }
    
        public virtual Project Project { get; set; }
        public virtual ICollection<BaselinePlanAudit> BaselinePlanAudit { get; set; }
        public virtual ICollection<Audits> Audits { get; set; }
    }
}