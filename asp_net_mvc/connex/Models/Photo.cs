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
    
    public partial class Photo
    {
        public int Photo_ID { get; set; }
        public string File_Path { get; set; }
        public string Thumb_Path { get; set; }
        public Nullable<System.DateTime> Date_Added { get; set; }
        public string Added_By { get; set; }
        public Nullable<System.DateTime> Date_Archive { get; set; }
        public string Archive_By { get; set; }
        public bool Archived { get; set; }
        public Nullable<int> Gallery_ID { get; set; }
    
        public virtual Gallery Gallery { get; set; }
    }
}
