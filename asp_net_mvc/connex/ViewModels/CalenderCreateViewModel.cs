using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace connex.ViewModels
{
    public class CalenderCreateViewModel
    {
        [Required]
        [StringLength(255)]
        [Display(Name="Name")]
        public string Calender_Name { get; set; }
        public int Page_ID { get; set; }

        public CalenderCreateViewModel()
        {
            this.Calender_Name = "";
            this.Page_ID = 0;
        }
        public CalenderCreateViewModel(int Page_ID)
        {
            this.Page_ID = Page_ID;
            this.Calender_Name = "";
        }
    }
}