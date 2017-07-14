using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;
namespace connex.ViewModels
{
    public class PageEditViewModel
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        public int Page_ID{get;set;}
        
        [StringLength(255)]
        [DisplayName("Page Name")]
        [Required]
        
        public string Page_Name{get; set;}
        
        [StringLength(255)]
        [DisplayName("Display Name")]
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public string URL { get; set; }

        [StringLength(255)]
        [DisplayName("Title Text")]
        [Required]
        public string Title_Text { get; set; }
        
        [DisplayName("Default Page")]
        public bool Is_Default { get; set; }

        [DisplayName("Display In Menu")]
        public bool Display_In_Menu { get; set; }
        
        [DisplayName("Parent Page")]
        public int? Parent_Page_ID { get; set; }

        public bool Redirect { get; set; }

        [DisplayName("Redirect URL")]
        public string Redirect_URL { get; set; }

        [DisplayName("Allow Feedback")]
        public bool Allow_Feedback { get; set; }

        public string Recipients_Temp { get; set; }
        
        public Banner banner;

        public List<Collection> collections;
        public List<Gallery> galleries;
        public List<Library> libraries;
        public List<Calender> calenders;
        public List<VideoGallery> videoGalleries;
        public List<QuickLinkList> quickLinkLists;
        public List<Page> availablePages;
        public List<Banner> availableBanners;
        public List<ContentType> availableContentTypes;
        public List<NotificationRecipent> recipients;
        public List<Feedback> allFeedback;

        private List<PageCollectionAssign> assColls;
        private List<PageCalenderAssign> assCals;
        private List<PageVideoGalleryAssign> assVidGals;
        private List<PageGalleryAssign> assPhotoGals;
        private List<PageLibraryAssign> assLibs;
        private List<PageQuickLinkListsAssign> assQuickLinkLists;
        private List<PageRecipientAssign> assRecipients;
        

        public PageEditViewModel()
        {
            this.Page_ID = 0;
            this.Page_Name = "";
            this.URL = "";
            this.Title_Text = "";
            this.Is_Default = false;
            this.Display_In_Menu = true;
            this.Parent_Page_ID = 0;
            this.Redirect = false;
            this.Redirect_URL = "";
            this.Allow_Feedback = false;
            this.Recipients_Temp = "";

            this.banner = new Banner();

            this.collections = new List<Collection>();
            this.galleries = new List<Gallery>();
            this.libraries = new List<Library>();
            this.calenders = new List<Calender>();
            this.videoGalleries = new List<VideoGallery>();
            this.quickLinkLists = new List<QuickLinkList>();
            this.availablePages = new List<Page>();
            this.recipients = new List<NotificationRecipent>();
            this.allFeedback = new List<Feedback>();

            this.assColls = new List<PageCollectionAssign>();
            this.assCals = new List<PageCalenderAssign>();
            this.assVidGals = new List<PageVideoGalleryAssign>();
            this.assLibs = new List<PageLibraryAssign>();
            this.assPhotoGals = new List<PageGalleryAssign>();
            this.assQuickLinkLists = new List<PageQuickLinkListsAssign>();
            this.assRecipients = new List<PageRecipientAssign>();

            availablePages = (List<Page>)(from pg in db.Pages where pg.Archived == false select pg).ToList();
            availableBanners = (List<Banner>)(from b in db.Banners select b).ToList();
            availableContentTypes = (List<ContentType>)(from c in db.ContentTypes select c).ToList();
        }
        public PageEditViewModel(Page page)
        {
            this.Page_ID = page.Page_ID;
            this.Page_Name = page.Page_Name;
            this.URL = page.URL;
            this.Title_Text = page.Title_Text;
            this.Is_Default = page.Is_Default;
            this.Parent_Page_ID = page.Parent_Page_ID;
            this.banner = page.Banner;
            this.Display_In_Menu = page.Display_In_Menu;
            this.Redirect = page.Redirect;
            this.Redirect_URL = page.Redirect_URL;
            this.Allow_Feedback = page.Allow_Feedback;
            this.Recipients_Temp = "";
            this.allFeedback = page.Feedbacks.ToList();

            this.collections = new List<Collection>();
            this.galleries = new List<Gallery>();
            this.libraries = new List<Library>();
            this.calenders = new List<Calender>();
            this.videoGalleries = new List<VideoGallery>();
            this.availablePages = new List<Page>();
            this.recipients = new List<NotificationRecipent>();
            this.quickLinkLists = new List<QuickLinkList>();
            

            this.assColls = page.PageCollectionAssigns.Where(x => x.Archived == false).ToList();
            this.assVidGals = page.PageVideoGalleryAssigns.Where(x => x.Archived == false).ToList();
            this.assCals = page.PageCalenderAssigns.Where(x => x.Archived == false).ToList();
            this.assLibs = page.PageLibraryAssigns.Where(x => x.Archived == false).ToList();
            this.assPhotoGals = page.PageGalleryAssigns.Where(x => x.Archived == false).ToList();
            this.assQuickLinkLists = page.PageQuickLinkListsAssigns.Where(x => x.Archived == false).ToList();
            this.assRecipients = page.PageRecipientAssigns.ToList();

            availablePages = (List<Page>)(from pg in db.Pages where pg.Archived == false select pg).ToList();
            availableBanners = (List<Banner>)(from b in db.Banners select b).ToList();
            availableContentTypes = (List<ContentType>)(from c in db.ContentTypes select c).ToList();

            foreach (var collIDAss in assColls.OrderBy(x => x.Sequence_No))
            {
                Collection curColl = (from res in db.Collections where res.Collection_ID == collIDAss.Collection_ID select res).FirstOrDefault();
                this.collections.Add(curColl);
            }
            foreach (var calIDAss in this.assCals)
            {
                Calender curCal = (from res in db.Calenders where res.Calender_ID == calIDAss.Calender_ID select res).FirstOrDefault();
                this.calenders.Add(curCal);
            }
            foreach (var vidGalIDAss in this.assVidGals)
            {
                VideoGallery curVidGal = (from res in db.VideoGalleries where res.Video_Gallery_ID == vidGalIDAss.Video_Gallery_ID select res).FirstOrDefault();
                this.videoGalleries.Add(curVidGal);
            }
            foreach (var libIDAss in this.assLibs)
            {
                Library curLib = (from res in db.Libraries where res.Library_ID == libIDAss.Library_ID select res).FirstOrDefault();
                this.libraries.Add(curLib);
            }
            foreach (var photoGalIDAss in this.assPhotoGals)
            {
                Gallery photoGal = (from res in db.Galleries where res.Gallery_ID == photoGalIDAss.Gallery_ID select res).FirstOrDefault();
                this.galleries.Add(photoGal);
            }
            foreach (var quickLinkListIDAss in this.assQuickLinkLists)
            {
                QuickLinkList qList = (from res in db.QuickLinkLists where res.Link_List_ID == quickLinkListIDAss.Link_List_ID select res).FirstOrDefault();
                this.quickLinkLists.Add(qList);
            }
            foreach (var recipientsIDAss in this.assRecipients)
            {
                NotificationRecipent rec = (from res in db.NotificationRecipents where res.Recipient_ID == recipientsIDAss.Recipient_ID select res).FirstOrDefault();
                this.recipients.Add(rec);
                this.Recipients_Temp += rec.Recipient_ID+","+rec.Recipient_Name+","+rec.Recipient_Email+"|";
            }
            if (this.Recipients_Temp.Length > 0)
            {
                this.Recipients_Temp = this.Recipients_Temp.Substring(0, this.Recipients_Temp.Length - 1);
            }
        }
    }
}