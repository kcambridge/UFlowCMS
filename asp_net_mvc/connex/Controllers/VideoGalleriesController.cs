using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using connex.Auth;
using connex.Models;
using connex.ViewModels;

namespace connex.Controllers
{
    public class VideoGalleriesController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();

        // GET: /VideoGalleries/
        public ActionResult Index()
        {
            return View(db.VideoGalleries.ToList());
        }

        // GET: /VideoGalleries/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    VideoGallery videogallery = db.VideoGalleries.Find(id);
        //    if (videogallery == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(videogallery);
        //}

        // GET: /VideoGalleries/Create?Page_ID=8
        [AuthContentManagers]
        public ActionResult Create(int Page_ID)
        {
            VideoGalleryCreateViewModel viewMod = new VideoGalleryCreateViewModel();
            viewMod.Page_ID = Page_ID;
            return View(viewMod);
        }

        // POST: /VideoGalleries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create(string Gallery_Name, string Details, int Page_ID)
        {
            //if (ModelState.IsValid)
            //{
                VideoGallery videogallery = new VideoGallery();
                videogallery.Gallery_Name = Gallery_Name;
                videogallery.Details = Details;
                videogallery.Date_Added = DateTime.Now;
                videogallery.Added_By = User.Identity.Name.ToString();///TODO auth
                videogallery.Archived = false;
                db.VideoGalleries.Add(videogallery);
                db.SaveChanges();

                PageVideoGalleryAssign vidGalAssign = new PageVideoGalleryAssign();
                vidGalAssign.Page_ID = Page_ID;
                vidGalAssign.VideoGallery = videogallery;
                db.PageVideoGalleryAssigns.Add(vidGalAssign);
                db.SaveChanges();
                
                return RedirectToAction("Edit", new { Page_ID = Page_ID, Video_Gallery_ID = videogallery.Video_Gallery_ID });
            //}
            //else
            //{
            //    VideoGalleryCreateViewModel vidGalViewMod = new VideoGalleryCreateViewModel();
            //    vidGalViewMod.Gallery_Name = Gallery_Name;
            //    vidGalViewMod.Details = Details;
            //    vidGalViewMod.Page_ID = Page_ID;
            //    ModelState.AddModelError("", "Invalid Mode");
            //    return View(vidGalViewMod);
            //}
        }

        // GET: /VideoGalleries/Edit/5
        [AuthContentManagers]
        public ActionResult Edit(int Page_ID, int Video_Gallery_ID)
        {
            VideoGallery videogallery = db.VideoGalleries.Find(Video_Gallery_ID);
            if (videogallery == null)
            {
                return HttpNotFound();
            }
            VideoGalleryEditViewModel vidGalViewMod = new VideoGalleryEditViewModel(videogallery, Page_ID);
            return View(vidGalViewMod);
        }

        // POST: /VideoGalleries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit([Bind(Include="Video_Gallery_ID,Gallery_Name,Archived,Details,Page_ID")] VideoGalleryEditViewModel vidGalViewMod)
        {
            if (ModelState.IsValid)
            {
                VideoGallery videogallery = db.VideoGalleries.Find(vidGalViewMod.Video_Gallery_ID);
                if (videogallery != null)
                {
                    videogallery.Gallery_Name = vidGalViewMod.Gallery_Name;
                    videogallery.Details = vidGalViewMod.Details;
                    if (videogallery.Archived != vidGalViewMod.Archived)
                    {
                        videogallery.Archived = vidGalViewMod.Archived;
                        videogallery.Date_Archive = DateTime.Now;
                        videogallery.Archive_By = User.Identity.Name.ToString(); ///TODO: auth
                        
                    }
                    videogallery.Last_Updated = DateTime.Now;
                    videogallery.Updated_By = User.Identity.Name.ToString();///TODO: Auth
                    db.SaveChanges();

                    return RedirectToAction("Edit", new { Page_ID = vidGalViewMod.Page_ID, Video_Gallery_ID = vidGalViewMod.Video_Gallery_ID});
                }
                else
                {
                    ModelState.AddModelError("", "No Video Gallery found with the ID '"+vidGalViewMod.Video_Gallery_ID+"'");
                    return View(vidGalViewMod);
                }
            }
            return View(vidGalViewMod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveVideo(int Page_ID, int Video_Gallery_ID, int Video_ID)
        {
            Video vid = db.Videos.Find(Video_ID);
            if (vid != null)
            {
                vid.Archived = true;
                vid.Date_Archive = DateTime.Now;
                vid.Archive_By = User.Identity.Name.ToString();///TODO: auth
                db.SaveChanges();
                return RedirectToAction("Edit", new { Page_ID = Page_ID, Video_Gallery_ID = Video_Gallery_ID });
            }
            else
            {
                return HttpNotFound();
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddVideo(int Page_ID, int Video_Gallery_ID, string Title_Text, string Caption)
        {
            VideoGallery vidGal = db.VideoGalleries.Find(Video_Gallery_ID);
            VideoGalleryEditViewModel vidGalViewMod = new VideoGalleryEditViewModel(vidGal, Page_ID);

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            string dirName = rgx.Replace(vidGal.Gallery_Name, "");

            string pathRoot = "~/video/galleries/" + dirName + "/";
            string dbRoot = "video/galleries/" + dirName + "/";

            string ImgpathRoot = "~/images/videos/" + dirName + "/";
            string ImgdbRoot = "images/videos/" + dirName + "/";

            if (!System.IO.Directory.Exists(Server.MapPath(pathRoot)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(pathRoot));
            }
            if (!System.IO.Directory.Exists(Server.MapPath(ImgpathRoot)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(ImgpathRoot));
            }
            if (Request.Files["WMVFile"] != null && Request.Files["MP4File"] != null && Request.Files["BannerImage"] != null)
            {
                
                HttpPostedFileBase wmvFile = Request.Files["WMVFile"];
                HttpPostedFileBase mp4File = Request.Files["MP4File"];
                HttpPostedFileBase imgFile = Request.Files["BannerImage"];
                if(wmvFile.ContentLength <= 0)
                {
                    ModelState.AddModelError("", "You must include a 'WMV File'");
                    return View("Edit", vidGalViewMod);
                }
                else if (mp4File.ContentLength <= 0)
                {
                    ModelState.AddModelError("", "You must include a 'MP4 File'");
                    return View("Edit", vidGalViewMod);
                }
                else if (imgFile.ContentLength <= 0)
                {
                    ModelState.AddModelError("", "You must include a 'Preview Image'");
                    return View("Edit", vidGalViewMod);
                }
                else
                {
                    string WMVfileName = wmvFile.FileName;
                    string WMVfileNameNoEx = Path.GetFileNameWithoutExtension(wmvFile.FileName);
                    string WMVext = Path.GetExtension(wmvFile.FileName).ToLower();

                    string MP4fileName = mp4File.FileName;
                    string MP4fileNameNoEx = Path.GetFileNameWithoutExtension(mp4File.FileName);
                    string MP4ext = Path.GetExtension(mp4File.FileName).ToLower();

                    string ImgfileName = imgFile.FileName;
                    string ImgfileNameNoEx = Path.GetFileNameWithoutExtension(imgFile.FileName);
                    string Imgext = Path.GetExtension(imgFile.FileName).ToLower();

                    if (WMVext != ".wmv")
                    {
                        ModelState.AddModelError("", "The 'WMV File' must be of type .wmv");
                        return View("Edit", vidGalViewMod);
                    }
                    else if (MP4ext != ".mp4")
                    {
                        ModelState.AddModelError("", "The 'WMV File' must be of type .wmv");
                        return View("Edit", vidGalViewMod);
                    }
                    else if (Imgext != ".jpg" && Imgext != ".png")
                    {
                        ModelState.AddModelError("", "The 'Preview Image' must be of type .jpg or .png");
                        return View("Edit",vidGalViewMod);
                    }
                    else
                    {
                       WMVfileNameNoEx =  UploadFile(wmvFile, pathRoot, WMVfileNameNoEx, WMVext);
                       WMVfileNameNoEx =  UploadFile(mp4File, pathRoot, WMVfileNameNoEx, MP4ext);
                       ImgfileNameNoEx =  UploadFile(imgFile, ImgpathRoot, ImgfileNameNoEx, Imgext);

                        Video vid = new Video();
                        vid.Date_Added = DateTime.Now;
                        vid.Added_By = User.Identity.Name.ToString();///TODO: auth
                        vid.Archived = false;
                        vid.Title_Text = Title_Text;
                        vid.Caption = Caption;
                        vid.VideoGallery = vidGal;
                        vid.Thumb_Path = ImgdbRoot + ImgfileNameNoEx+Imgext;
                        vid.File_Path = dbRoot + WMVfileNameNoEx;
                        db.Videos.Add(vid);
                        db.SaveChanges();
                        return RedirectToAction("Edit", new { Page_ID = Page_ID, Video_Gallery_ID = Video_Gallery_ID });

                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "You must  select each of the following files: WMV File, MP4 File, Preview Image");
                return View("Edit", vidGalViewMod);
            }

            
        }

        private string UploadFile(HttpPostedFileBase file, string pathRoot, string fileNameNoEx, string ext)
        {
            string retName = fileNameNoEx;
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
            return retName;
        }
        
        // GET: /VideoGalleries/Delete/5

        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    VideoGallery videogallery = db.VideoGalleries.Find(id);
        //    if (videogallery == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(videogallery);
        //}

        //// POST: /VideoGalleries/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    VideoGallery videogallery = db.VideoGalleries.Find(id);
        //    db.VideoGalleries.Remove(videogallery);
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
