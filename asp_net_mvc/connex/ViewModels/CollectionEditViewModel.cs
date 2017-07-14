using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class CollectionEditViewModel
    {
        [StringLength(255)]
        [Required]
        [DisplayName("Title")]
        public string Collection_Title { get; set; }
        [StringLength(255)]
        [Required]
        [DisplayName("Description")]
        public string Collection_Description { get; set; }
        [Required]
        [DisplayName("Display Mode")]
        public int Display_Mode_ID { get; set; }
        public int Page_ID { get; set; }
        [Required]
        [DisplayName("Archived")]
        public bool Archived { get; set; }
        public int Collection_ID { get; set; }
        public List<CollectionsDisplayMode> availableDisplayModes;
        public List<Post> posts;

        public CollectionEditViewModel()
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            this.Collection_Description = "";
            this.Collection_Title = "";
            this.Display_Mode_ID = 0;
            this.Page_ID = 0;
            this.Archived = false;
            this.posts = new List<Post>();
            this.Collection_ID = 0;
            this.availableDisplayModes = (List<CollectionsDisplayMode>)db.CollectionsDisplayModes.ToList();
        }

        public CollectionEditViewModel(Collection coll, int Page_ID)
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            this.Collection_Description = coll.Collection_Description;
            this.Collection_Title = coll.Collection_Title;
            this.Display_Mode_ID = coll.Display_Mode_ID;
            this.Page_ID = Page_ID;
            this.Archived = coll.Archived;
            this.posts = (List<Post>)coll.Posts.ToList();
            this.Collection_ID = coll.Collection_ID;
            this.availableDisplayModes = (List<CollectionsDisplayMode>)db.CollectionsDisplayModes.ToList();
        }

    }
}