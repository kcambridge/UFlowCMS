using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class QuickLinkListCreateViewModel
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public int Page_ID { get; set; }

        public QuickLinkListCreateViewModel()
        {
            this.Name = "";
            this.Page_ID = 0;
        }

        public QuickLinkListCreateViewModel(QuickLinkList list, int Page_ID)
        {
            this.Name = list.Name;
            this.Page_ID = Page_ID;
        }
    }
}