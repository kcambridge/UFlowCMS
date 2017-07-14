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
    public class EventsController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        private static string CAL_IMG_BASE_ROOT = ConfigurationManager.AppSettings["CalenderImageUploadBaseRoot"];
        private static int CAL_IMG_HEIGHT = Convert.ToInt32(ConfigurationManager.AppSettings["CalenderImageHeight"]);
        private static int MAX_IMG_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["ImageMaxSize"]);

        // GET: /Events/
        public ActionResult Index()
        {
            var events = db.Events;
            return View(events.ToList());
        }

        // GET: /Events/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Event @event = db.Events.Find(id);
        //    if (@event == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(@event);
        //}
        // GET: /Events/View/5
        public ActionResult View(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        //// GET: /Events/Create
        //[AuthContentManagers]
        //public ActionResult Create()
        //{
        //    ViewBag.Calender_ID = new SelectList(db.Calenders, "Calender_ID", "Calender_Name");
        //    return View();
        //}

        //// POST: /Events/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthContentManagers]
        //public ActionResult Create([Bind(Include="Event_ID,Event_Name,Description,Thumb_Path,Image_Path,Calender_ID,Date_Added,Added_By,Last_Updated,Updated_By,Date_Archive,Archive_By,Archived,Start_Date_Time,End_Date_Time,Location")] Event @event)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Events.Add(@event);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Calender_ID = new SelectList(db.Calenders, "Calender_ID", "Calender_Name", @event.Calender_ID);
        //    return View(@event);
        //}

        // GET: /Events/Edit/?Event_ID=3&Page_ID=4
        [AuthContentManagers]
        public ActionResult Edit(int Page_ID, int Event_ID, bool? Return_Home)
        {
            if (Return_Home != null)
            {
                ViewBag.Return_Home = true;
                Page page = db.Pages.Find(Page_ID);
                ViewBag.Page_Url = page.URL;
            }
            Event ev = db.Events.Find(Event_ID);
            if (ev == null)
            {
                return HttpNotFound();
            }
            EventEditViewModel viewMod = new EventEditViewModel(Page_ID, ev);
            return View(viewMod);
        }
        private void UploadFile(HttpPostedFileBase file, string pathRoot, string fileNameNoEx, string ext)
        {
            ///TODO: Need to handle changes to file name at upper levels
            try
            {
                byte[] imageSize = new byte[file.ContentLength];
                file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                string filePath = Server.MapPath(pathRoot) + file.FileName;

                if (System.IO.File.Exists(filePath))
                {
                    int i = 1;
                    filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                    while (System.IO.File.Exists(filePath))
                    {
                        i++;
                        filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                    }

                }

                file.SaveAs(filePath);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        // POST: /Events/Edit/?Event_ID=5&Page_ID=3
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit(int Page_ID, int Calender_ID, int Event_ID, string Event_Name, string Description, string Start_Date, string Start_Time, string End_Date, string End_Time, string Location, bool Archived, string Thumb_Path, string Image_Path, bool? Return_Home)
        {
            EventEditViewModel eventViewMod = new EventEditViewModel();
            eventViewMod.Page_ID = Page_ID;
            eventViewMod.Calender_ID = Calender_ID;
            eventViewMod.Event_ID = Event_ID;
            eventViewMod.Event_Name = Event_Name;
            eventViewMod.Description = Description;
            eventViewMod.Start_Date = Start_Date;
            eventViewMod.Start_Time = Start_Time;
            eventViewMod.End_Date = End_Date;
            eventViewMod.End_Time = End_Time;
            eventViewMod.Archived = Archived;
            eventViewMod.Location = Location;
            eventViewMod.Thumb_Path = Thumb_Path;
            eventViewMod.Image_Path = Image_Path;
            if (ModelState.IsValid)
            {
                DateTime start = Convert.ToDateTime(Start_Date + " " + Start_Time);
                DateTime end = Convert.ToDateTime(End_Date + " " + End_Time);
                if (start > end)
                {
                    ModelState.AddModelError("", "Start Date/Time cannot be after End Date/Time");
                    return View(eventViewMod);
                }
                else
                {
                    Event ev = db.Events.Find(Event_ID);
                    if (Request.Files["Flyer"] != null)
                    {
                        string pathRoot = "~/" + CAL_IMG_BASE_ROOT;
                        string dbRoot = CAL_IMG_BASE_ROOT;
                        string pathThumbRoot = "~/" + CAL_IMG_BASE_ROOT + "thumbs/";
                        string dbThumbRoot = CAL_IMG_BASE_ROOT + "thumbs/";

                        if (!System.IO.Directory.Exists(Server.MapPath(pathRoot)))
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath(pathRoot));
                        }
                        if (!System.IO.Directory.Exists(Server.MapPath(pathThumbRoot)))
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath(pathThumbRoot));
                        }

                        HttpPostedFileBase file = Request.Files["Flyer"];
                        if (file.ContentLength >  0)
                        {
                            if (file.ContentLength <= MAX_IMG_SIZE)
                            {
                                string fileNameNoEx = Path.GetFileNameWithoutExtension(file.FileName);
                                string ext = Path.GetExtension(file.FileName).ToLower();
                                if (ext == ".jpg" || ext == ".png")
                                {
                                    try
                                    {
                                        UploadFile(file, pathRoot, fileNameNoEx, ext);
                                        Image img = Image.FromFile(Server.MapPath(pathRoot) + file.FileName);
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
                                            if (ext == ".jpg")
                                            {
                                                newImage.Save(Server.MapPath(pathThumbRoot) + fileNameNoEx + ext, ImageFormat.Jpeg);
                                            }
                                            else if (ext == ".png")
                                            {
                                                newImage.Save(Server.MapPath(pathThumbRoot) + fileNameNoEx + ext, ImageFormat.Png);
                                            }
                                            else
                                            {
                                                newImage.Save(Server.MapPath(pathThumbRoot) + fileNameNoEx + ext);
                                            }
                                        }
                                        else
                                        {
                                            if (ext == ".jpg")
                                            {
                                                img.Save(Server.MapPath(pathThumbRoot) + fileNameNoEx + ext, ImageFormat.Jpeg);
                                            }
                                            else if (ext == ".png")
                                            {
                                                img.Save(Server.MapPath(pathThumbRoot) + fileNameNoEx + ext, ImageFormat.Png);
                                            }
                                            else
                                            {
                                                img.Save(Server.MapPath(pathThumbRoot) + fileNameNoEx + ext);
                                            }
                                        }
                                        ev.Image_Path = dbRoot + file.FileName;
                                        ev.Thumb_Path = dbThumbRoot + file.FileName;
                                    }
                                    catch (Exception EX)
                                    {
                                        ModelState.AddModelError("", "Failed to upload file. " + EX.Message);
                                        return View(eventViewMod);

                                    }
                                    
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Only .png and .jpg images are supported");
                                    return View(eventViewMod);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Flyers/Images must be less than 10 MB");
                                return View(eventViewMod);
                            }
                        }
                        
                    }

                    ev.Start_Date_Time = start;
                    ev.End_Date_Time = end;
                    ev.Event_Name = Event_Name;
                    ev.Description = Description;
                    ev.Location = Location;
                    ev.Last_Updated = DateTime.Now;
                    ev.Updated_By = User.Identity.Name.ToString();///TODO:auth


                    if (ev.Archived != Archived)
                    {
                        ev.Archived = Archived;
                        ev.Archive_By = User.Identity.Name.ToString();///TODO:auth
                        ev.Date_Archive = DateTime.Now;
                    }

                    db.SaveChanges();
                    return RedirectToAction("Edit", "Calenders", new { Page_ID = Page_ID, Calender_ID = Calender_ID, Return_Home = Return_Home});
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Model State.");
                return View(eventViewMod);
            }
            
            
        }

        // GET: /Events/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Event @event = db.Events.Find(id);
        //    if (@event == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(@event);
        //}

        // POST: /Events/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Event @event = db.Events.Find(id);
        //    db.Events.Remove(@event);
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
