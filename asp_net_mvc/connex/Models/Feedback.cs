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
    
    public partial class Feedback
    {
        public int Feedback_ID { get; set; }
        public string Feedback_Details { get; set; }
        public System.DateTime Date_Submitted { get; set; }
        public int Page_ID { get; set; }
        public string Name { get; set; }
    
        public virtual Page Page { get; set; }
    }
}
