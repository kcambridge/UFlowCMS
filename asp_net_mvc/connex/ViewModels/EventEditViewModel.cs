using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class EventEditViewModel
    {
        public int Event_ID { get; set; }
        public int Calender_ID { get; set; }
        public int Page_ID { get; set; }
        public string Thumb_Path { get; set; }
        public string Image_Path { get; set; }
        [Required]
        [StringLength(255)]
        public string Event_Name { get; set; }
        [Required]
        public string Description { get; set; }
        [RegularExpression(@"^(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.](19|20)\d\d$", ErrorMessage = "Invalid date format")]
        [Required]
        public string Start_Date { get; set; }
        [RegularExpression(@"^(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.](19|20)\d\d$", ErrorMessage = "Invalid date format")]
        [Required]
        public string End_Date { get; set; }
        [Required]
        [RegularExpression(@"^(00|[0-9]|1[012]):[0-5][0-9][ ]{1}?((a|p)m|(A|P)M)$", ErrorMessage = "Invalid time format")]
        public string Start_Time { get; set; }
        [RegularExpression(@"^(00|[0-9]|1[012]):[0-5][0-9][ ]{1}?((a|p)m|(A|P)M)$", ErrorMessage = "Invalid time format")]
        [Required]
        public string End_Time { get; set; }
        [Required]
        [StringLength(255)]
        public string Location { get; set; }
        [Required]
        public bool Archived { get; set; }
        

        public EventEditViewModel()
        {
            this.Event_ID = 0;
            this.Calender_ID = 0;
            this.Page_ID = 0;
            this.Event_Name = "";
            this.Description = "";
            this.Location = "";
            this.Start_Date = "";
            this.Start_Time = "";
            this.End_Date = "";
            this.End_Time = "";
            this.Image_Path = "";
            this.Thumb_Path = "";
            this.Archived = false;
            
        }

        public EventEditViewModel(int Page_ID , Event ev)
        {
            this.Event_ID = ev.Event_ID;
            this.Calender_ID = ev.Calender_ID;
            this.Page_ID = Page_ID;
            this.Event_Name = ev.Event_Name;
            this.Description = ev.Description;
            this.Location = ev.Location;
            this.Start_Date = ev.Start_Date_Time.ToString("MM/dd/yyyy");
            this.Start_Time = ev.Start_Date_Time.ToString("hh:mm tt");
            this.End_Date = ev.End_Date_Time.ToString("MM/dd/yyyy");
            this.End_Time = ev.End_Date_Time.ToString("hh:mm tt");
            this.Image_Path = ev.Image_Path;
            this.Thumb_Path = ev.Thumb_Path;
            this.Archived = ev.Archived;
        }


    }
}