using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class LibraryCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title_Text { get; set; }
        public string Description { get; set; }
        public int Page_ID { get; set; }

        public LibraryCreateViewModel()
        {
            this.Title_Text = "";
            this.Description = "";
            this.Page_ID = 0;
        }
        public LibraryCreateViewModel(Library lib, int Page_ID)
        {
            this.Title_Text = lib.Title_Text;
            this.Description = lib.Description;
            this.Page_ID = Page_ID;
        }
    }
}