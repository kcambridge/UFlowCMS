using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class VideoGalleryCreateViewModel
    {
        [Required]
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Gallery_Name { get; set; }
        [Display(Name="Description")]
        public string Details { get; set; }
        public int Page_ID { get; set; }

        public VideoGalleryCreateViewModel()
        {
            this.Gallery_Name = "";
            this.Details = "";
            this.Page_ID = 0;
        }

        public VideoGalleryCreateViewModel(VideoGallery vidGal, int Page_ID)
        {
            this.Gallery_Name = vidGal.Gallery_Name;
            this.Details = this.Details;
        }
    }
}