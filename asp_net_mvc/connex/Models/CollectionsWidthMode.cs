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
    
    public partial class CollectionsWidthMode
    {
        public CollectionsWidthMode()
        {
            this.Collections = new HashSet<Collection>();
        }
    
        public int Width_Mode_ID { get; set; }
        public string Width_Mode { get; set; }
        public Nullable<int> Percentage { get; set; }
    
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
