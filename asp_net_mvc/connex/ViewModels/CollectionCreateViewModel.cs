using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class CollectionCreateViewModel
    {
        [StringLength(255)]
        [Required]
        [DisplayName("Title")]
        public string Collection_Title { get; set; }
        [StringLength(255)]
        [Required]
        [DisplayName("Description")]
        public string Collection_Description { get; set; }
        [Required]
        [DisplayName("Display Mode")]
        public int Display_Mode_ID { get; set; }
        public int Page_ID { get; set; }
        public List<CollectionsDisplayMode> availableDisplayModes;

        public CollectionCreateViewModel()
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            this.Collection_Title = "";
            this.Collection_Description = "";
            this.Display_Mode_ID = 2;
            this.Page_ID = 0;
            this.availableDisplayModes = (List<CollectionsDisplayMode>)db.CollectionsDisplayModes.ToList();
        }
        public CollectionCreateViewModel(int Page_ID)
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            this.Collection_Title = "";
            this.Collection_Description = "";
            this.Display_Mode_ID = 2;
            this.Page_ID = Page_ID;
            this.availableDisplayModes = (List<CollectionsDisplayMode>)db.CollectionsDisplayModes.ToList();
        }

    }
}