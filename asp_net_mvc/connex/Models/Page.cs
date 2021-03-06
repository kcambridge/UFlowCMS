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
    
    public partial class Page
    {
        public Page()
        {
            this.PageAdminAssigns = new HashSet<PageAdminAssign>();
            this.PageCalenderAssigns = new HashSet<PageCalenderAssign>();
            this.PageCollectionAssigns = new HashSet<PageCollectionAssign>();
            this.PageGalleryAssigns = new HashSet<PageGalleryAssign>();
            this.PageLibraryAssigns = new HashSet<PageLibraryAssign>();
            this.PageVideoGalleryAssigns = new HashSet<PageVideoGalleryAssign>();
            this.Feedbacks = new HashSet<Feedback>();
            this.PageQuickLinkListsAssigns = new HashSet<PageQuickLinkListsAssign>();
            this.PageRecipientAssigns = new HashSet<PageRecipientAssign>();
        }
    
        public int Page_ID { get; set; }
        public string Page_Name { get; set; }
        public string URL { get; set; }
        public string Title_Text { get; set; }
        public Nullable<int> Banner_ID { get; set; }
        public bool Is_Default { get; set; }
        public Nullable<int> Parent_Page_ID { get; set; }
        public bool Is_Top { get; set; }
        public bool Archived { get; set; }
        public string Archived_By { get; set; }
        public Nullable<System.DateTime> Date_Archived { get; set; }
        public Nullable<System.DateTime> Date_Modified { get; set; }
        public string Modified_By { get; set; }
        public bool Has_Children { get; set; }
        public Nullable<System.DateTime> Date_Added { get; set; }
        public string Added_By { get; set; }
        public int Sequence_No { get; set; }
        public bool Display_In_Menu { get; set; }
        public bool Redirect { get; set; }
        public string Redirect_URL { get; set; }
        public bool Allow_Feedback { get; set; }
    
        public virtual Banner Banner { get; set; }
        public virtual ICollection<PageAdminAssign> PageAdminAssigns { get; set; }
        public virtual ICollection<PageCalenderAssign> PageCalenderAssigns { get; set; }
        public virtual ICollection<PageCollectionAssign> PageCollectionAssigns { get; set; }
        public virtual ICollection<PageGalleryAssign> PageGalleryAssigns { get; set; }
        public virtual ICollection<PageLibraryAssign> PageLibraryAssigns { get; set; }
        public virtual ICollection<PageVideoGalleryAssign> PageVideoGalleryAssigns { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<PageQuickLinkListsAssign> PageQuickLinkListsAssigns { get; set; }
        public virtual ICollection<PageRecipientAssign> PageRecipientAssigns { get; set; }
    }
}
