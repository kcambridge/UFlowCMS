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
    
    public partial class Banner
    {
        public Banner()
        {
            this.Pages = new HashSet<Page>();
            this.BannerElements = new HashSet<BannerElement>();
        }
    
        public int Banner_ID { get; set; }
        public string Banner_Name { get; set; }
    
        public virtual ICollection<Page> Pages { get; set; }
        public virtual ICollection<BannerElement> BannerElements { get; set; }
    }
}
