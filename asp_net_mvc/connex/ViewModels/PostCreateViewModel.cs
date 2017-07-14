using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class PostCreateViewModel
    {
        [Required]
        [DisplayName("Post Title")]
        [StringLength(255)]
        public string Header { get; set; }
        public string Thumb_Path { get; set; }
        public int Collection_ID { get; set; }
        public int Page_ID { get; set; }
        [DisplayName("Summary")]
        public string Summary { get; set; }

        public List<Section> sections;

        public PostCreateViewModel()
        {
            this.Header = "";
            this.Thumb_Path = "";
            this.Collection_ID = 0;
            this.Page_ID = 0;
            this.Summary = "";
            this.sections = new List<Section>();
        }
        public PostCreateViewModel(Post post, int Page_ID)
        {
            this.Header = post.Header;
            this.Thumb_Path = post.Thumb_Path;
            this.Collection_ID = post.Collection_ID;
            this.Page_ID = Page_ID;
            this.Summary = post.Summary;
            this.sections = (List<Section>)post.Sections.ToList();
        }
    }
}