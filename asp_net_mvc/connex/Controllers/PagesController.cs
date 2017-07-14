using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using connex.Models;
using connex.ViewModels;
using System.IO;
using System.Configuration;
using connex.Auth;
using System.Net.Mail;

namespace connex.Controllers
{
    public class PagesController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        private static int MAX_IMG_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["ImageMaxSize"]);
        

        // GET: /Pages/
        public ActionResult Index()
        {
            var pageID = (from res in db.Pages where res.Is_Default == true select res.Page_ID).FirstOrDefault();
            //setting hide show controls variable to hide show edit controls based on user access
            string userName = User.Identity.Name;
            if (userName != "")
            {
                Admin currentUser = (Admin)db.Admins.Where(x => x.Network_ID == userName).FirstOrDefault();
                if (currentUser != null)
                {
                    if (currentUser.PageAdminAssigns.Any(x => x.Page_ID == pageID) || currentUser.Roles.Any(x => x.Role1 == "Admin"))
                    {
                        ViewBag.CanEdit = true;
                    }
                    else
                    {
                        ViewBag.CanEdit = false;
                    }
                }
                else
                {
                    ViewBag.CanEdit = false;
                }
            }
            else
            {
                ViewBag.CanEdit = false;
            }

            if (pageID == 0)
            {
                if (db.Pages.Count() > 0)
                {
                    return View("View", new PageViewModel(db.Pages.First()));
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {

                return View("View", new PageViewModel(db.Pages.Find(pageID))); 
            }

        }
        new public ActionResult View(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = id.ToLower();
            int pageID = db.Pages.Where(x => x.URL.ToLower() == id).FirstOrDefault().Page_ID;
            if (pageID == 0)
            {
                return HttpNotFound();
            }
            Page page = db.Pages.Find(pageID);
            if (page.Redirect)
            {
                if (page.Redirect_URL != null)
                {
                    return Redirect(page.Redirect_URL);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                PageViewModel pageViewModel = new PageViewModel(page);

                string userName = User.Identity.Name;
                if (userName != "")
                {
                    Admin currentUser = (Admin)db.Admins.Where(x => x.Network_ID == userName).FirstOrDefault();

                    if (currentUser != null)
                    {
                        if (currentUser.PageAdminAssigns.Any(x => x.Page_ID == page.Page_ID) && currentUser.Roles.Any(x => x.Role1 == "ContentManager") || currentUser.Roles.Any(x => x.Role1 == "Admin"))
                        {
                            ViewBag.CanEdit = true;
                        }
                        else
                        {
                            ViewBag.CanEdit = false;
                        }
                    }
                    else
                    {
                        ViewBag.CanEdit = false;
                    }
                }
                else
                {
                    ViewBag.CanEdit = false;
                }

                return View(pageViewModel);
            }
        }

        // GET: /Pages/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Page page = db.Pages.Find(id);
        //    if (page == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(page);
        //}

        // GET: /Pages/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name");
            PageCreateViewModel newPage = new PageCreateViewModel();
            return View(newPage);
        }

        // POST: /Pages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="pageName,url,displayName,defaultPage, parentPageID, displayInMenu")]PageCreateViewModel pageViewModel)
        {


            if (db.Pages.Where(x => x.URL.ToLower() == pageViewModel.url.ToLower()).Count() <= 0)
            {
                if (db.Pages.Where(x => x.Page_Name.ToLower() == pageViewModel.pageName.ToLower()).Count() <= 0)
                {
                    Page page = new Page();
                    page.Page_Name = pageViewModel.pageName;
                    page.URL = pageViewModel.url;
                    page.Title_Text = pageViewModel.displayName;
                    page.Is_Default = pageViewModel.defaultPage;
                    page.Display_In_Menu = pageViewModel.displayInMenu;
                    page.Redirect = false;
                    if (pageViewModel.parentPageID != 0)
                    {
                        page.Parent_Page_ID = pageViewModel.parentPageID;
                        page.Is_Top = false;
                        Page parentPage = (Page)(from p in db.Pages where p.Page_ID == pageViewModel.parentPageID select p).FirstOrDefault();
                        parentPage.Has_Children = true;
                    }
                    else
                    {
                        page.Is_Top = true;
                    }
                    page.Date_Added = DateTime.Now;
                    page.Added_By = User.Identity.Name.ToString();//TODO:need to add authentication
                    page.Sequence_No = 9999;//TODO need to allow for changing of page order for admins
                    db.Pages.Add(page);
                    db.SaveChanges();
                    return RedirectToAction("Edit", new { id = page.Page_ID });
                }
                else
                {
                    ModelState.AddModelError("", "A page with the Page Name '" + pageViewModel.pageName + "' already exists");
                    return View("Create", pageViewModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "A page with the URL '"+pageViewModel.url+"' already exists");
                return View("Create", pageViewModel);
            }
            
        }

        // GET: /Pages/Edit/5
        [AuthContentManagers]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = db.Pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Page_Url = page.URL;
            ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
            PageEditViewModel viewMod = new PageEditViewModel(page);
            return View(viewMod);
        }

        [AuthContentManagers]
        public ActionResult EditBannerElement(int Page_ID, int Element_ID)
        {
            BannerElement element = db.BannerElements.Find(Element_ID);
            if (element == null)
            {
                return HttpNotFound();
            }
            BannerElementEditViewModel elementViewMod = new BannerElementEditViewModel(element, Page_ID);
            return View("EditBannerElement", elementViewMod);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveBannerElement(string Element_ID, string Page_ID)
        {
            int elementID = Convert.ToInt32(Element_ID);
            BannerElement element = (BannerElement)(from be in db.BannerElements where be.Element_ID == elementID select be).FirstOrDefault();
            element.Archived = true;
            element.Archive_By = User.Identity.Name.ToString();///TODO: need authentication here
            element.Date_Archive = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = Page_ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddBannerElement(string Header_Text, string Summary, int? Banner_ID, int Page_ID, int? Content_Type_ID, int? Content_ID)
        {
            //you are here
            //check image size and validate that image was added
            HttpPostedFileBase file = Request.Files["BannerImage"];
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    if (!(file.ContentLength > MAX_IMG_SIZE))
                    {
                        byte[] imageSize = new byte[file.ContentLength];
                        file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                        string filePath = Server.MapPath("~/images/banners/") + file.FileName;
                        string fileName = file.FileName;
                        string fileNameNoEx = Path.GetFileNameWithoutExtension(file.FileName);
                        string ext = Path.GetExtension(file.FileName);
                        if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png")
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                int i = 1;
                                filePath = Server.MapPath("~/images/banners/") + fileNameNoEx + i.ToString() + ext;
                                while (System.IO.File.Exists(filePath))
                                {
                                    i++;
                                    filePath = Server.MapPath("~/images/banners/") + fileNameNoEx + i.ToString() + ext;
                                    fileName = fileNameNoEx + i.ToString() + ext;
                                }

                            }
                            file.SaveAs(filePath);
                            Page page = db.Pages.Find(Page_ID);
                            if (Banner_ID != null)
                            {
                                BannerElement newElement = new BannerElement();
                                newElement.Banner_ID = Banner_ID.Value;
                                newElement.Header_Text = Header_Text;
                                newElement.Summary = Summary;
                                newElement.Date_Added = DateTime.Now;
                                newElement.Added_By = User.Identity.Name.ToString();///TODO: Need authentication here
                                newElement.Archived = false;
                                newElement.Image_Path = "images/banners/" + fileName;
                                if (Content_Type_ID != null)//link to content if provided
                                {
                                    if (Content_Type_ID != 0)
                                    {
                                        newElement.Content_ID = Content_ID;
                                        newElement.Content_Type_ID = Content_Type_ID;
                                    }
                                } 
                                db.BannerElements.Add(newElement);
                                db.SaveChanges();
                            }
                            else
                            {
                                Banner newBanner = new Banner();
                                string bannerName = page.Page_Name + "Banner";
                                if (((List<Banner>)(from b in db.Banners where b.Banner_Name == bannerName select b).ToList()).Count() > 0)
                                {
                                    int nameCnt = 1;
                                    string nameStrng = bannerName + nameCnt.ToString();
                                    while (((List<Banner>)(from b in db.Banners where b.Banner_Name == nameStrng select b).ToList()).Count() > 0)
                                    {
                                        nameCnt++;
                                        nameStrng = bannerName + nameCnt.ToString();
                                    }
                                    bannerName = nameStrng;
                                }
                                newBanner.Banner_Name = bannerName;
                                db.Banners.Add(newBanner);
                                db.SaveChanges();

                                page.Banner = newBanner;

                                BannerElement newElement = new BannerElement();
                                newElement.Banner_ID = newBanner.Banner_ID;
                                newElement.Header_Text = Header_Text;
                                newElement.Summary = Summary;
                                newElement.Date_Added = DateTime.Now;
                                newElement.Added_By = User.Identity.Name.ToString();///TODO: Need authentication here
                                newElement.Archived = false;
                                newElement.Image_Path = "images/banners/" + fileName;
                                if (Content_Type_ID != null)//link to content if provided
                                {
                                    if (Content_Type_ID != 0)
                                    {
                                        newElement.Content_ID = Content_ID;
                                        newElement.Content_Type_ID = Content_Type_ID;
                                    }
                                } 
                                db.BannerElements.Add(newElement);
                                db.SaveChanges();



                            }
                            return RedirectToAction("Edit", new { id = Page_ID });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Only .png and .jpg images are supported");
                            Page page = db.Pages.Find(Page_ID);
                            if (page == null)
                            {
                                return HttpNotFound();
                            }
                            ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
                            PageEditViewModel viewMod = new PageEditViewModel(page);
                            return View("Edit", viewMod);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your Banner Image must be less than 10 MB");
                        Page page = db.Pages.Find(Page_ID);
                        if (page == null)
                        {
                            return HttpNotFound();
                        }
                        ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
                        PageEditViewModel viewMod = new PageEditViewModel(page);
                        return View("Edit", viewMod);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "You must select a banner image");
                    Page page = db.Pages.Find(Page_ID);
                    if (page == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
                    PageEditViewModel viewMod = new PageEditViewModel(page);
                    return View("Edit", viewMod);
                }
            }
            else
            {
                ModelState.AddModelError("", "You must select a banner image");
                Page page = db.Pages.Find(Page_ID);
                if (page == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
                PageEditViewModel viewMod = new PageEditViewModel(page);
                return View("Edit", viewMod);
            }
            
        }

        // POST: /Pages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit(PageEditViewModel pageViewMod)
        {
            Page page = db.Pages.Find(pageViewMod.Page_ID);
            if (db.Pages.Where(x => x.URL.ToLower() == pageViewMod.URL.ToLower() && x.Page_ID != pageViewMod.Page_ID).Count() <= 0)
            {
                if (db.Pages.Where(x => x.Page_Name.ToLower() == pageViewMod.Page_Name.ToLower() && x.Page_ID != pageViewMod.Page_ID).Count() <= 0)
                {
                    
                    //if (ModelState.IsValid)
                    //{
                    page.Page_Name = pageViewMod.Page_Name;
                    page.URL = pageViewMod.URL;
                    page.Title_Text = pageViewMod.Title_Text;
                    if (pageViewMod.Is_Default)
                    {
                        List<Page> pages = (List<Page>)db.Pages.Where(x => x.Page_ID != page.Page_ID).ToList();
                        foreach (Page p in pages)
                        {
                            p.Is_Default = false;
                        }

                    }
                    page.Is_Default = pageViewMod.Is_Default;
                    page.Display_In_Menu = pageViewMod.Display_In_Menu;
                    page.Redirect = pageViewMod.Redirect;
                    page.Redirect_URL = pageViewMod.Redirect_URL;
                    page.Allow_Feedback = pageViewMod.Allow_Feedback;
                    if (page.Allow_Feedback)
                    {
                        if (pageViewMod.Recipients_Temp == null)
                        {
                            pageViewMod.Recipients_Temp = "";
                        }
                        string[] recipients = pageViewMod.Recipients_Temp.Split('|');
                        int pageID = page.Page_ID;
                        List<NotificationRecipent> existingRecip = db.NotificationRecipents.Where(x => x.PageRecipientAssigns.Any( y => y.Page_ID ==pageID)).ToList();
                        foreach (NotificationRecipent rec in existingRecip)
                        {
                            bool onPage = false;
                            foreach (string recip in recipients)
                            {
                                string[] parts = recip.Split(',');
                                if (parts.Length == 3)
                                {
                                    if (rec.Recipient_Name == parts[1] && rec.Recipient_Email == parts[2])
                                    {
                                        onPage = true;
                                        break;
                                    }
                                }
                            }
                            if (!onPage)
                            {
                                int recID = rec.Recipient_ID;
                                int page_ID = page.Page_ID;
                                PageRecipientAssign recDelTemp = db.PageRecipientAssigns.Where(x => x.Page_ID == page_ID && x.Recipient_ID == recID).FirstOrDefault();
                                db.PageRecipientAssigns.Remove(recDelTemp);
                                if (db.PageRecipientAssigns.Where(x => x.Recipient_ID == recID && x.Page_ID != page_ID).Count() == 0)
                                {
                                    NotificationRecipent delTemp = db.NotificationRecipents.Where(x => x.Recipient_ID == recID).FirstOrDefault();
                                    db.NotificationRecipents.Remove(delTemp);
                                }
                                
                            }
                        }
                        foreach (string recip in recipients)
                        {
                            string[] parts = recip.Split(',');
                            if (parts.Length == 3)
                            {
                                string recName = parts[1];
                                string recEmail = parts[2];
                                if (existingRecip.Where(x => x.Recipient_Name == recName && x.Recipient_Email == recEmail).Count() == 0)
                                {
                                    if (db.NotificationRecipents.Where(x => x.Recipient_Email == recEmail && x.Recipient_Name == recName).Count() > 0)
                                    {
                                        NotificationRecipent tempRec = db.NotificationRecipents.Where(x => x.Recipient_Name == recName && x.Recipient_Email == recEmail).FirstOrDefault();
                                        
                                        PageRecipientAssign recAss = new PageRecipientAssign();
                                        recAss.Page = page;
                                        recAss.NotificationRecipent = tempRec;
                                        recAss.Date_Added = DateTime.Now;
                                        recAss.Added_By = User.Identity.Name.ToString();
                                        db.PageRecipientAssigns.Add(recAss);
                                    }
                                    else
                                    {
                                        NotificationRecipent newRec = new NotificationRecipent();
                                        newRec.Recipient_Name = recName;
                                        newRec.Recipient_Email = recEmail;
                                        newRec.Date_Added = DateTime.Now;
                                        newRec.Added_By = User.Identity.Name.ToString();
                                        db.NotificationRecipents.Add(newRec);

                                        PageRecipientAssign recAss = new PageRecipientAssign();
                                        recAss.Page = page;
                                        recAss.NotificationRecipent = newRec;
                                        recAss.Date_Added = DateTime.Now;
                                        recAss.Added_By = User.Identity.Name.ToString();
                                        db.PageRecipientAssigns.Add(recAss);
                                    }
                                }
                            }
                        }

                    }
                    if (pageViewMod.Parent_Page_ID != null)
                    {
                        page.Is_Top = false;
                        Page parentPage = db.Pages.Find(pageViewMod.Parent_Page_ID);
                        parentPage.Has_Children = true;
                    }
                    else if (pageViewMod.Parent_Page_ID == null && page.Parent_Page_ID != null)
                    {
                        if (db.Pages.Where(x => x.Page_ID != page.Page_ID && x.Parent_Page_ID == page.Parent_Page_ID).Count() == 0)
                        {
                            Page parentPage = db.Pages.Find(page.Parent_Page_ID);
                            parentPage.Has_Children = false;
                            db.SaveChanges();
                        }
                        page.Is_Top = true;
                    }
                    page.Parent_Page_ID = pageViewMod.Parent_Page_ID;
                    page.Date_Modified = DateTime.Now;
                    page.Modified_By = User.Identity.Name;
                    db.SaveChanges();
                    ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
                    return RedirectToAction("Edit", new { id = page.Page_ID });
                }
                else
                {
                    ModelState.AddModelError("", "A Page with the Page Name '"+pageViewMod.Page_Name+"' already exists");
                    ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
                    return View("Edit",pageViewMod);
                }
            }
            else
            {
                ModelState.AddModelError("", "A Page with the URL '" + pageViewMod.URL + "' already exists");
                ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
                return View("Edit",pageViewMod);
            }
            //}
            //ModelState.AddModelError("", "An unexpectced error occurred. Invalid Model State. Please try again.");
            //return View(pageViewMod);
        }
        ///TODO: Image and Video Gallery Removal
        ///
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveImageGallery(int Page_ID, int Gallery_ID)
        {
            PageGalleryAssign gal = db.PageGalleryAssigns.Where(x => x.Gallery_ID == Gallery_ID && x.Page_ID == Page_ID).FirstOrDefault();
            if (gal == null)
            {
                return HttpNotFound();
            }
            gal.Archive_By = User.Identity.Name.ToString();///TODO: auth
            gal.Date_Archive = DateTime.Now;
            gal.Archived = true;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = Page_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveVideoGallery(int Page_ID, int Video_Gallery_ID)
        {
            PageVideoGalleryAssign gal = (PageVideoGalleryAssign)db.PageVideoGalleryAssigns.Where(x => x.Video_Gallery_ID == Video_Gallery_ID && x.Page_ID == Page_ID).FirstOrDefault();
            if (gal == null)
            {
                return HttpNotFound();
            }
            gal.Archive_By = User.Identity.Name.ToString();///TODO: auth
            gal.Date_Archive = DateTime.Now;
            gal.Archived = true;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = Page_ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveCollection(int Page_ID, int Collection_ID)
        {
            PageCollectionAssign col = (PageCollectionAssign)db.PageCollectionAssigns.Where(x => x.Collection_ID == Collection_ID && x.Page_ID == Page_ID).FirstOrDefault();
            if (col == null)
            {
                return HttpNotFound();
            }
            col.Archive_By = User.Identity.Name.ToString();///TODO: auth
            col.Date_Archive = DateTime.Now;
            col.Archived = true;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = Page_ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveLibrary(int Page_ID, int Library_ID)
        {
            PageLibraryAssign lib = (PageLibraryAssign)db.PageLibraryAssigns.Where(x => x.Library_ID == Library_ID && x.Page_ID == Page_ID).FirstOrDefault();
            if (lib == null)
            {
                return HttpNotFound();
            }
            lib.Archive_By = User.Identity.Name.ToString();///TODO: auth
            lib.Date_Archive = DateTime.Now;
            lib.Archived = true;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = Page_ID });
        }

        [HttpPost]
        public ActionResult SendFeedback(int Page_ID, string Feedback_Name, string Feedback_Message)
        {
            Page page = db.Pages.Find(Page_ID);
            if (page == null)
            {
                return HttpNotFound();
            }
            if (Feedback_Message.Trim() != "")
            {
                if (Feedback_Name == "Enter Name (Optional)")
                {
                    Feedback_Name = "Annonymous";
                }
                Feedback newFeedback = new Feedback();
                newFeedback.Name = Feedback_Name;
                newFeedback.Feedback_Details = Feedback_Message;
                newFeedback.Date_Submitted = DateTime.Now;
                newFeedback.Page_ID = Page_ID;
                db.Feedbacks.Add(newFeedback);
                db.SaveChanges();
                List<NotificationRecipent> recipients = db.NotificationRecipents.Where(x => x.PageRecipientAssigns.Any(y => y.Page_ID == Page_ID)).ToList();
                
                if (recipients.Count() > 0)
                {
                    try
                    {
                        MailMessage mail = new MailMessage();
                        foreach (NotificationRecipent rec in recipients)
                        {
                            mail.To.Add(rec.Recipient_Email);
                        }
                        mail.From = new MailAddress("TT-ApplicationDevelopment" + ConfigurationManager.AppSettings["EmailDomain"]);
                        mail.Subject = "Connex Feedback : " + page.Page_Name;
                        mail.Body = "<html><head></head><body>Good Day,<br /><br />A user has submitted feedback for the " + page.Page_Name + " Page.<br /><br /><b>Name: </b>" + newFeedback.Name + "<br /><br /><b>Feedback:</b><br />" + newFeedback.Feedback_Details + "</body></html>";
                        mail.IsBodyHtml = true;
                        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["EmailServer"]);
                        client.Send(mail);
                    }
                    catch (Exception)
                    {
                            ///TODO: Need to log failure somwhere, or queue and re-send. Also external email addresses fail so need to find a solution for tha
                        
                    }
                    
                }

                return RedirectToAction("View", new { id = page.URL });
            }
            else
            {
                ModelState.AddModelError("", "You must enter a 'Feedback Message'");
                PageViewModel pageViewMod = new PageViewModel(page);
                pageViewMod.Feedback_Name = Feedback_Name;
                pageViewMod.Feedback_Message = Feedback_Message;
                string userName = User.Identity.Name;
                if (userName != "")
                {
                    Admin currentUser = (Admin)db.Admins.Where(x => x.Network_ID == userName).FirstOrDefault();

                    if (currentUser != null)
                    {
                        if (currentUser.PageAdminAssigns.Any(x => x.Page_ID == page.Page_ID) && currentUser.Roles.Any(x => x.Role1 == "ContentManager") || currentUser.Roles.Any(x => x.Role1 == "Admin"))
                        {
                            ViewBag.CanEdit = true;
                        }
                        else
                        {
                            ViewBag.CanEdit = false;
                        }
                    }
                    else
                    {
                        ViewBag.CanEdit = false;
                    }
                }
                else
                {
                    ViewBag.CanEdit = false;
                }
                return View("View", pageViewMod);
            }
            
        }
        //// GET: /Pages/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Page page = db.Pages.Find(id);
        //    if (page == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(page);
        //}
        
        // POST: /Pages/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Page page = db.Pages.Find(id);
        //    db.Pages.Remove(page);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
