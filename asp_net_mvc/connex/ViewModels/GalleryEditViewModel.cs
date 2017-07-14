using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class GalleryEditViewModel
    {
        public int Page_ID { get; set; }
        public int Gallery_ID { get; set; }
        [Required]
        [StringLength(255)]
        [Display(Name="Gallery Name")]
        public string Gallery_Name { get; set; }
        public List<Photo> photos;
        public bool Archived { get; set; }

        public GalleryEditViewModel()
        {
            this.Page_ID = 0;
            this.Gallery_ID = 0;
            this.Gallery_Name = "";
            this.photos = new List<Photo>();
            this.Archived = false;
        }

        public GalleryEditViewModel(int Page_ID, Gallery gal)
        {
            this.Page_ID = Page_ID;
            this.Gallery_ID = gal.Gallery_ID;
            this.Gallery_Name = gal.Gallery_Name;
            this.photos = (List<Photo>)gal.Photos.ToList();
            this.Archived = gal.Archived;
        }
    }
}