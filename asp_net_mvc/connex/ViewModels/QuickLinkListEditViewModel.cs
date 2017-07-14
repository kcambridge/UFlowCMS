using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class QuickLinkListEditViewModel
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public int Page_ID { get; set; }
        public int Link_List_ID { get; set; }
        [Required]
        public bool Archived { get; set; }

        public List<QuickLink> quickLinks;

        public QuickLinkListEditViewModel()
        {
            this.Name = "";
            this.Page_ID = 0;
            this.Archived = false;
            this.quickLinks = new List<QuickLink>();
            this.Link_List_ID = 0;
        }

        public QuickLinkListEditViewModel(QuickLinkList list, int Page_ID)
        {
            this.Name = list.Name;
            this.Archived = list.Archived;
            this.Page_ID = Page_ID;
            this.quickLinks = list.QuickLinks.ToList();
            this.Link_List_ID = list.Link_List_ID;
        }
    }
}