using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class PostEditViewModel
    {

        public int Collection_ID { get; set; }
        public int Page_ID { get; set; }


        public int Post_ID { get; set; }
        [StringLength(255)]
        public string Thumb_Path { get; set; }
        [StringLength(255)]
        public string Header { get; set; }
        public string Summary { get; set; }
        [Required]
        public bool Archived { get; set; }
        [Required]
        [Display(Name = "Allow Comments")]
        public bool Allow_Comments { get; set; }
        [Required]
        public List<Section> sections;
        public List<MediaType> availableMediaTypes;
        public List<WidthMode> availableWidthModes;

        public PostEditViewModel()
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            this.availableMediaTypes = (List<MediaType>)(from mt in db.MediaTypes select mt).ToList();
            this.availableWidthModes = (List<WidthMode>)(from wm in db.WidthModes select wm).ToList();
            this.Post_ID = 0;
            this.Collection_ID = 0;
            this.Thumb_Path = "";
            this.Header = "";
            this.Summary = "";
            this.Page_ID = 0;
            this.Archived = false;
            this.Allow_Comments = false;
            this.sections = new List<Section>();
        }

        public PostEditViewModel(Post post, int Page_ID)
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            this.availableMediaTypes = (List<MediaType>)(from mt in db.MediaTypes select mt).ToList();
            this.availableWidthModes = (List<WidthMode>)(from wm in db.WidthModes select wm).ToList();
            this.Post_ID = post.Post_ID;
            this.Collection_ID = post.Collection_ID;
            this.Thumb_Path = post.Thumb_Path;
            this.Header = post.Header;
            this.Summary = post.Summary;
            this.Page_ID = Page_ID;
            this.Archived = post.Archived;
            this.Allow_Comments = post.Allow_Comments;
            this.sections = (List<Section>)post.Sections.ToList();
        }
    }
}