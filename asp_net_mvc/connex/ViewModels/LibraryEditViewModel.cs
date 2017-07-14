using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;
namespace connex.ViewModels
{
    public class LibraryEditViewModel
    {

        public int Library_ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Title_Text { get; set; }
        public string Description { get; set; }
        public List<Document> documents;
        public int Page_ID { get; set; }
        [Required]
        public bool Archived { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public LibraryEditViewModel()
        {
            this.Library_ID = 0;
            this.Title_Text = "";
            this.Description = "";
            this.documents = new List<Document>();
            this.Page_ID = 0;
            this.Archived = false;
            this.Name = "";
        }
        public LibraryEditViewModel(Library lib, int Page_ID)
        {
            this.Library_ID = lib.Library_ID;
            this.Title_Text = lib.Title_Text;
            this.Description = lib.Description;
            this.documents = (List<Document>)lib.Documents.ToList();
            this.Page_ID = Page_ID;
            this.Archived = lib.Archived;
            this.Name = "";
        }
    }
}