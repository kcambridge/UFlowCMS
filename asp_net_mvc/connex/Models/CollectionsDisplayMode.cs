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
    
    public partial class CollectionsDisplayMode
    {
        public CollectionsDisplayMode()
        {
            this.Collections = new HashSet<Collection>();
        }
    
        public int Display_Mode_ID { get; set; }
        public string Display_Mode { get; set; }
    
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
