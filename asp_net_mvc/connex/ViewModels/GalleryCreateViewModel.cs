using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace connex.ViewModels
{
    public class GalleryCreateViewModel
    {
        [Required]
        [StringLength(255)]
        [Display(Name="Name")]
        public string Gallery_Name { get; set; }
        public int Page_ID { get; set; }

        public GalleryCreateViewModel()
        {
            this.Gallery_Name = "";
            this.Page_ID = 0;
        }
        public GalleryCreateViewModel(int Page_ID)
        {
            this.Page_ID = Page_ID;
            this.Gallery_Name = "";
        }

    }
}