using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class VideoGalleryEditViewModel
    {
        public int Page_ID { get; set; }
        public int Video_Gallery_ID { get; set; }
        [Required]
        [StringLength(255)]
        [Display(Name="Name")]
        public string Gallery_Name { get; set; }
        [Display(Name="Description")]
        public string Details { get; set; }
        public Boolean Archived { get; set; }
        public List<Video> videos;

        public VideoGalleryEditViewModel()
        {
            this.Page_ID = 0;
            this.Video_Gallery_ID = 0;
            this.Gallery_Name = "";
            this.Details = "";
            this.videos = new List<Video>();
            this.Archived = false;
        }

        public VideoGalleryEditViewModel(VideoGallery vidGal, int Page_ID)
        {
            this.Page_ID = Page_ID;
            this.Video_Gallery_ID = vidGal.Video_Gallery_ID;
            this.Gallery_Name = vidGal.Gallery_Name;
            this.Details = vidGal.Details;
            this.videos = (List<Video>)vidGal.Videos.ToList();
            this.Archived = vidGal.Archived;
        }
    }
}