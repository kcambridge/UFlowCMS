//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace connex.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Admin
    {
        public Admin()
        {
            this.PageAdminAssigns = new HashSet<PageAdminAssign>();
            this.Roles = new HashSet<Role>();
        }
    
        public int Admin_ID { get; set; }
        public string Network_ID { get; set; }
        public string Full_Name { get; set; }
        public Nullable<System.DateTime> Date_Added { get; set; }
        public string Added_By { get; set; }
    
        public virtual ICollection<PageAdminAssign> PageAdminAssigns { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
