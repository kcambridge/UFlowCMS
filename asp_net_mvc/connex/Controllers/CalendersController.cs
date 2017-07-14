using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using connex.Auth;
using connex.Models;
using connex.ViewModels;

namespace connex.Controllers
{
    public class CalendersController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        private static string CAL_IMG_BASE_ROOT = ConfigurationManager.AppSettings["CalenderImageUploadBaseRoot"];
        private static int CAL_IMG_HEIGHT = Convert.ToInt32(ConfigurationManager.AppSettings["CalenderImageHeight"]);
        private static int MAX_IMG_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["ImageMaxSize"]);

        // GET: /Calenders/
        public ActionResult Index()
        {
            return View(db.Calenders.ToList());
        }

        //// GET: /Calenders/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Calender calender = db.Calenders.Find(id);
        //    if (calender == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(calender);
        //}

        // GET: /Calenders/View/5
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calender calender = db.Calenders.Find(id);
            if (calender == null)
            {
                return HttpNotFound();
            }
            return View(calender);
        }

        // GET: /Calenders/Create
        [AuthContentManagers]
        public ActionResult Create(int Page_ID, bool? Return_Home)
        {
            CalenderCreateViewModel calViewMod = new CalenderCreateViewModel(Page_ID);
            if (Return_Home != null)
            {
                ViewBag.Return_Home = true;
                Page page = db.Pages.Find(Page_ID);
                ViewBag.Page_Url = page.URL;
            }
            return View(calViewMod);
        }

        // POST: /Calenders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create(string Calender_Name, int Page_ID, bool? Return_Home)
        {

            if (Return_Home != null)
            {
                ViewBag.Return_Home = true;
                Page page = db.Pages.Find(Page_ID);
                ViewBag.Page_Url = page.URL;
            }
            if (db.Calenders.Where(x => x.Calender_Name == Calender_Name).Count() > 0)
            {
                CalenderCreateViewModel calViewMod = new CalenderCreateViewModel(Page_ID);
                calViewMod.Calender_Name = Calender_Name;
                ModelState.AddModelError("", "A Calender with the name '" + Calender_Name + "' alredy exists.");
                return View(calViewMod);
            }
            else
            {
                Calender cal = new Calender();
                cal.Calender_Name = Calender_Name;
                cal.Date_Added = DateTime.Now;
                cal.Added_By = User.Identity.Name.ToString();///TODO: auth
                cal.Archived = false;
                db.Calenders.Add(cal);
                db.SaveChanges();

                PageCalenderAssign callAss = new PageCalenderAssign();
                callAss.Page_ID = Page_ID;
                callAss.Calender = cal;
                callAss.Archived = false;
                callAss.Date_Added = DateTime.Now;
                callAss.Added_By = User.Identity.Name.ToString();///TODO: auth
                db.PageCalenderAssigns.Add(callAss);
                db.SaveChanges();

                return RedirectToAction("Edit", new { Page_ID = Page_ID, Calender_ID = cal.Calender_ID, Return_Home = Return_Home});
            }
        }

        // GET: /Calenders/Edit/5
        [AuthContentManagers]
        public ActionResult Edit(int Page_ID, int Calender_ID, bool? Return_Home)
        {
            if (Return_Home != null)
            {
                ViewBag.Return_Home = true;
                Page page = db.Pages.Find(Page_ID);
                ViewBag.Page_Url = page.URL;
            }
            Calender calender = db.Calenders.Find(Calender_ID);
            if (calender == null)
            {
                return HttpNotFound();
            }
            CalenderEditViewModel calViewMod = new CalenderEditViewModel(Page_ID, calender);
            return View(calViewMod);
        }

        // POST: /Calenders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit(string Calender_Name, int Page_ID, int Calender_ID, bool Archived, bool? Return_Home)
        {
            if (Return_Home != null)
            {
                ViewBag.Return_Home = true;
                Page page = db.Pages.Find(Page_ID);
                ViewBag.Page_Url = page.URL;
            }
            if (ModelState.IsValid)
            {
                Calender calender = db.Calenders.Find(Calender_ID);
                calender.Calender_Name = Calender_Name;
                if (calender.Archived != Archived)
                {
                    calender.Archived = Archived;
                    calender.Date_Archived = DateTime.Now;
                    calender.Archived_By = User.Identity.Name.ToString();///TODO: auth
                }
                calender.Last_Updated = DateTime.Now;
                calender.Updated_By = User.Identity.Name.ToString();///TODO:auth
                db.SaveChanges();
                return RedirectToAction("Edit", new { Page_ID = Page_ID, Calender_ID = Calender_ID, Return_Home = Return_Home});
            }
            else
            {
                CalenderEditViewModel calViewMod = new CalenderEditViewModel();
                calViewMod.Page_ID = Page_ID;
                calViewMod.Calender_Name = Calender_Name;
                calViewMod.Calender_ID = Calender_ID;
                calViewMod.Archived = Archived;
                ModelState.AddModelError("", "Invalid Model State. Changes not saved.");
                return View(calViewMod);
            }
            
        }
        private string  UploadFile(HttpPostedFileBase file, string pathRoot, string fileNameNoEx, string ext)
        {
            string retName = fileNameNoEx;
            try
            {
                byte[] imageSize = new byte[file.ContentLength];
                file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                string filePath = Server.MapPath(pathRoot) + fileNameNoEx+ext;

                if (System.IO.File.Exists(filePath))
                {
                    int i = 1;
                    filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                    retName = fileNameNoEx + i.ToString();
                    while (System.IO.File.Exists(filePath))
                    {
                        i++;
                        filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                        retName = fileNameNoEx + i.ToString();
                    }

                }

                file.SaveAs(filePath);

            }
            catch (Exception ex)
            {

                throw;
            }
            return retName;

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddEvent(CalenderEditViewModel calViewMod, bool? Return_Home)
        {
            if (Return_Home != null)
            {
                ViewBag.Return_Home = true;
                Page page = db.Pages.Find(calViewMod.Page_ID);
                ViewBag.Page_Url = page.URL;
            }
            Event ev = new Event();
            string pathRoot = "~/" + CAL_IMG_BASE_ROOT;
            string dbRoot = CAL_IMG_BASE_ROOT;
            string pathThumbRoot = "~/" + CAL_IMG_BASE_ROOT+"thumbs/";
            string dbThumbRoot = CAL_IMG_BASE_ROOT+"thumbs/";
            

            if (!System.IO.Directory.Exists(Server.MapPath(pathRoot)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(pathRoot));
            }
            if (!System.IO.Directory.Exists(Server.MapPath(pathThumbRoot)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(pathThumbRoot));
            }

            HttpPostedFileBase file = Request.Files["Flyer"];
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    if (file.ContentLength <= MAX_IMG_SIZE)
                    {
                        DateTime start = Convert.ToDateTime(calViewMod.Start_Date + " " + calViewMod.Start_Time);
                        DateTime end = Convert.ToDateTime(calViewMod.End_Date + " " + calViewMod.End_Time);
                        if (start > end)
                        {
                            ModelState.AddModelError("", "The event start date/time cannot be after the envent end date/time");
                            return View(calViewMod);
                        }
                        else
                        {
                            string fileNameNoEx = Path.GetFileNameWithoutExtension(file.FileName);
                            string ext = Path.GetExtension(file.FileName).ToLower();
                            if (ext == ".jpg" || ext == ".png")
                            {
                                try
                                {
                                   fileNameNoEx = UploadFile(file, pathRoot, fileNameNoEx, ext);
                                    Image img = Image.FromFile(Server.MapPath(pathRoot) + file.FileName);
                                    string newFilePath = Server.MapPath(pathThumbRoot) + fileNameNoEx + ext;
                                    if (img.Height > CAL_IMG_HEIGHT)
                                    {
                                        int width = img.Width;
                                        int height = img.Height;
                                        double origDouble = Convert.ToDouble(height);
                                        double maxDouble = Convert.ToDouble(CAL_IMG_HEIGHT);
                                        double perc = 1 - ((origDouble - maxDouble) / origDouble);
                                        width = Convert.ToInt32(Convert.ToDouble(width) * perc);
                                        height = Convert.ToInt32(Convert.ToDouble(height) * perc);
                                        Size newSize = new Size(width, height);
                                        Image newImage = (Image)(new Bitmap(img, newSize));
                                        
                                        string retName = fileNameNoEx;
                                        if (System.IO.File.Exists(newFilePath))
                                        {
                                            int i = 1;
                                            newFilePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                                            retName = fileNameNoEx + i.ToString();
                                            while (System.IO.File.Exists(newFilePath))
                                            {
                                                i++;
                                                newFilePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                                                retName = fileNameNoEx + i.ToString();
                                            }
                                            fileNameNoEx = retName;
                                        }

                                        if (ext == ".jpg")
                                        {
                                            newImage.Save(newFilePath, ImageFormat.Jpeg);
                                        }
                                        else if (ext == ".png")
                                        {
                                            newImage.Save(newFilePath, ImageFormat.Png);
                                        }
                                        else
                                        {
                                            newImage.Save(newFilePath);
                                        }
                                    }
                                    else
                                    {
                                        if (ext == ".jpg")
                                        {
                                            img.Save(newFilePath, ImageFormat.Jpeg);
                                        }
                                        else if (ext == ".png")
                                        {
                                            img.Save(newFilePath, ImageFormat.Png);
                                        }
                                        else
                                        {
                                            img.Save(newFilePath);
                                        }
                                    }
                                }
                                catch (Exception EX)
                                {
                                    ModelState.AddModelError("", "Failed to upload file. " + EX.Message);
                                    return View(calViewMod);

                                }
                                Calender cal = db.Calenders.Find(calViewMod.Calender_ID);
                                ev.Event_Name = calViewMod.Event_Name;
                                ev.Description = calViewMod.Description;
                                ev.Location = calViewMod.Location;
                                ev.Start_Date_Time = start;
                                ev.End_Date_Time = end;
                                ev.Date_Added = DateTime.Now;
                                ev.Added_By = User.Identity.Name.ToString();///TODO:Auth
                                ev.Archived = false;
                                ev.Calender = cal;
                                ev.Image_Path = dbRoot + fileNameNoEx+ext;
                                ev.Thumb_Path = dbThumbRoot + fileNameNoEx+ext;
                                db.Events.Add(ev);
                                db.SaveChanges();
                                return RedirectToAction("Edit", new { Page_ID = calViewMod.Page_ID, Calender_ID = calViewMod.Calender_ID, Return_Homen = Return_Home});
                            }
                            else
                            {
                                ModelState.AddModelError("", "Only .png and .jpg images are supported");
                                return View(calViewMod);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Flyers/Images must be less than 10 MB");
                        return View(calViewMod);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "You must add a Flyer/Image");
                    return View(calViewMod);
                }
                
            }
            else
            {
                ModelState.AddModelError("", "You must add a Flyer/Image");
                return View(calViewMod);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveEvent(int Page_ID, int Calender_ID, int Event_ID, bool? Return_Home)
        {
            if (Return_Home != null)
            {
                ViewBag.Return_Home = true;
                Page page = db.Pages.Find(Page_ID);
                ViewBag.Page_Url = page.URL;
            }
            Event ev = db.Events.Find(Event_ID);
            ev.Archived = true;
            ev.Date_Archive = DateTime.Now;
            ev.Archive_By = User.Identity.Name.ToString();///TODO:auth
            db.SaveChanges();
            return RedirectToAction("Edit", new { Page_ID = Page_ID, Calender_ID = Calender_ID, Return_Home = Return_Home});
        }

        //// GET: /Calenders/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Calender calender = db.Calenders.Find(id);
        //    if (calender == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(calender);
        //}

        // POST: /Calenders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Calender calender = db.Calenders.Find(id);
        //    db.Calenders.Remove(calender);
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
