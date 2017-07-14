using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ViewModels
{
    public class PageViewModel
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        public Page thisPage;
        private List<PageCollectionAssign> assColls;
        private List<PageCalenderAssign> assCals;
        private List<PageVideoGalleryAssign> assVidGals;
        private List<PageGalleryAssign> assPhotoGals;
        private List<PageLibraryAssign> assLibs;
        public List<Collection> collections;
        public List<Calender> calenders;
        public List<VideoGallery> videoGalleries;
        public List<Gallery> photoGalleries;
        public List<Library> libraries;
        public List<QuickLinkList> quickLinkLists;
        private List<PageQuickLinkListsAssign> assQuickLinkLists;


        public string Feedback_Name { get; set; }
        [Required]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "A Minimum of 5 characters required")]
        public string Feedback_Message { get; set; }

        public PageViewModel()
        {
            this.thisPage = null;
            
            this.assColls = new List<PageCollectionAssign>();
            this.assCals = new List<PageCalenderAssign>();
            this.assVidGals = new List<PageVideoGalleryAssign>();
            this.assLibs = new List<PageLibraryAssign>();
            this.assPhotoGals = new List<PageGalleryAssign>();


            this.collections = new List<Collection>();
            this.calenders = new List<Calender>();
            this.videoGalleries = new List<VideoGallery>();
            this.libraries = new List<Library>();
            this.photoGalleries = new List<Gallery>();
            this.quickLinkLists = new List<QuickLinkList>();
            this.assQuickLinkLists = new List<PageQuickLinkListsAssign>();
            this.Feedback_Message = "";
            this.Feedback_Name = "";

        }
        public PageViewModel(Page page)
        {
            this.thisPage = page;
            this.Feedback_Message = "";
            this.Feedback_Name = "";
            if (this.thisPage.Banner != null)
            {
                this.thisPage.Banner.BannerElements = page.Banner.BannerElements.Where(x => x.Archived == false).ToList();
            }
            this.assColls = this.thisPage.PageCollectionAssigns.Where(x => x.Archived == false).ToList();
            this.assVidGals = this.thisPage.PageVideoGalleryAssigns.Where(x => x.Archived == false).ToList();
            this.assCals = this.thisPage.PageCalenderAssigns.Where(x => x.Archived == false).ToList();
            this.assLibs = this.thisPage.PageLibraryAssigns.Where(x => x.Archived == false).ToList();
            this.assPhotoGals = this.thisPage.PageGalleryAssigns.Where(x => x.Archived == false).ToList();
            this.assQuickLinkLists = this.thisPage.PageQuickLinkListsAssigns.Where(x => x.Archived == false).ToList();
            
            this.collections = new List<Collection>();
            this.calenders = new List<Calender>();
            this.videoGalleries = new List<VideoGallery>();
            this.libraries = new List<Library>();
            this.photoGalleries = new List<Gallery>();
            this.quickLinkLists = new List<QuickLinkList>();
            
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
                this.photoGalleries.Add(photoGal);
            }
            foreach (var quickLinkListIDAss in this.assQuickLinkLists)
            {
                QuickLinkList qList = (from res in db.QuickLinkLists where res.Link_List_ID == quickLinkListIDAss.Link_List_ID select res).FirstOrDefault();
                this.quickLinkLists.Add(qList);
            }
            
        }
        

        
    }
}