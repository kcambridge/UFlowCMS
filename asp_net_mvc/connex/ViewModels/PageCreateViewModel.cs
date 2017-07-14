using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class PageCreateViewModel
    {
        [StringLength(255)]
        [DisplayName("Page Name")]
        [Required]
        public string pageName{get; set;}
        [StringLength(255)]
        [DisplayName("URL")]
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public string url { get; set; }
        [StringLength(255)]
        [DisplayName("Display Name")]
        [Required]
        public string displayName { get; set; }
        [DisplayName("Default Page")]
        [Required]
        public bool defaultPage { get; set; }
        [DisplayName("Display In Menu")]
        [Required]
        public bool displayInMenu { get; set; }
        [DisplayName("Parent Page")]
        public int parentPageID { get; set; }
        public List<Page> availablePages;
        

        public PageCreateViewModel()
        {
            this.pageName = "";
            this.url = "";
            this.displayName = "";
            this.defaultPage = false;
            this.parentPageID = 0;
            this.displayInMenu = true;
            CONNEXDBEntities db = new CONNEXDBEntities();
            availablePages = (List<Page>) (from pg in db.Pages where pg.Archived == false select pg).ToList();
        }
        public PageCreateViewModel(string pageName, string url, string displayName, bool defaultPage, int parentPageID, bool displayInMenu)
        {
            this.pageName = pageName;
            this.url = url;
            this.displayName = displayName;
            this.defaultPage = defaultPage;
            this.parentPageID = parentPageID;
            this.displayInMenu = displayInMenu;
            CONNEXDBEntities db = new CONNEXDBEntities();
            availablePages = (List<Page>)(from pg in db.Pages where pg.Archived == false select pg).ToList();
        }

    }
}