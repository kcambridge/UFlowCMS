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
    
    public partial class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
            this.Sections = new HashSet<Section>();
        }
    
        public int Post_ID { get; set; }
        public string Header { get; set; }
        public string Thumb_Path { get; set; }
        public int Collection_ID { get; set; }
        public Nullable<System.DateTime> Date_Added { get; set; }
        public string Added_By { get; set; }
        public Nullable<System.DateTime> Date_Archive { get; set; }
        public string Archive_By { get; set; }
        public Nullable<System.DateTime> Last_Updated { get; set; }
        public string Updated_By { get; set; }
        public bool Archived { get; set; }
        public string Summary { get; set; }
        public bool Allow_Comments { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
        public virtual Collection Collection { get; set; }
    }
}
