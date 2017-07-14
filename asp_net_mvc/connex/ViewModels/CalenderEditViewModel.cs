using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class CalenderEditViewModel
    {
        [Required]
        [StringLength(255)]
        [Display(Name="Name")]
        public string Calender_Name { get; set; }
        [Required]
        public bool Archived { get; set; }
        public int Calender_ID { get; set; }
        public int Page_ID { get; set; }
        public List<Event> upcommingEvents;
        public List<Event> pastEvents;
        
        //Used for validation of new events
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



        public CalenderEditViewModel()
        {
            this.Calender_Name = "";
            this.Archived = false;
            this.Calender_ID = 0;
            this.Page_ID = 0;
            this.upcommingEvents = new List<Event>();
            this.pastEvents = new List<Event>();
            this.Event_Name = "";
            this.Description = "";
            this.Location = "";
            this.Start_Date = "";
            this.Start_Time = "";
            this.End_Date = "";
            this.End_Time = "";
        }
        public CalenderEditViewModel(int Page_ID, Calender calender)
        {
            DateTime now = DateTime.Now;
            this.Calender_Name = calender.Calender_Name;
            this.Calender_ID = calender.Calender_ID;
            this.Archived = calender.Archived;
            this.Page_ID = Page_ID;
            this.upcommingEvents = (List<Event>)calender.Events.Where(x => x.Archived == false && x.End_Date_Time >= now).OrderBy(x => x.End_Date_Time).ToList();
            this.pastEvents = (List<Event>)calender.Events.Where(x => x.Archived == false && x.End_Date_Time < now).OrderByDescending(x => x.End_Date_Time).ToList();
            this.Event_Name = "";
            this.Description = "";
            this.Location = "";
            this.Start_Date = "";
            this.Start_Time = "";
            this.End_Date = "";
            this.End_Time = "";
        }
    }
}