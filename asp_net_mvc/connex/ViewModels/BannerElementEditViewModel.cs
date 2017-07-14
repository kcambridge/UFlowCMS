using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class BannerElementEditViewModel
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        public int Page_ID { get; set; }
        public int Banner_ID { get; set; }
        public int Element_ID { get; set; }
        [StringLength(255)]
        [DisplayName("Title Text")]
        public string Header_Text { get; set; }
        [StringLength(255)]
        public string Summary { get; set; }
        public int? Content_ID { get; set; }
        public int? Content_Type_ID { get; set; }
        public List<ContentType> availableContentTypes;

        public BannerElementEditViewModel()
        {
            this.Page_ID = 0;
            this.Banner_ID = 0;
            this.Element_ID = 0;
            this.Header_Text = "";
            this.Summary = "";
            this.Content_ID = 0;
            this.Content_Type_ID = 0;
            availableContentTypes = (List<ContentType>)(from c in db.ContentTypes select c).ToList();
        }

        public BannerElementEditViewModel(BannerElement element, int Page_ID)
        {
            this.Banner_ID = element.Banner_ID;
            this.Element_ID = element.Element_ID;
            this.Page_ID = Page_ID;
            this.Header_Text = element.Header_Text;
            this.Summary = element.Summary;
            this.Content_Type_ID = element.Content_Type_ID;
            this.Content_ID = element.Content_ID;
            availableContentTypes = (List<ContentType>)(from c in db.ContentTypes select c).ToList();
        }
    }
}