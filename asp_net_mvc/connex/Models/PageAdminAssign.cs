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
    
    public partial class PageAdminAssign
    {
        public int Page_ID { get; set; }
        public int Admin_ID { get; set; }
        public Nullable<System.DateTime> Date_Added { get; set; }
        public string Added_By { get; set; }
    
        public virtual Admin Admin { get; set; }
        public virtual Page Page { get; set; }
    }
}