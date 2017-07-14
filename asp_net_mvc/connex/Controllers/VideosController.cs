using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using connex.Auth;
using connex.Models;

namespace connex.Controllers
{
    public class VideosController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();

        // GET: /Videos/
        public ActionResult Index()
        {
            var videos = db.Videos.Include(v => v.VideoGallery);
            return View(videos.ToList());
        }
        // GET: /Videos/View/5
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var vidID = (from res in db.Videos where res.Video_ID == id select res.Video_ID).FirstOrDefault();
            if (vidID == 0)
            {
                return HttpNotFound();
            }
            Video vid = db.Videos.Find(vidID);
            return View(vid);
        }
        // GET: /Videos/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Video video = db.Videos.Find(id);
        //    if (video == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(video);
        //}

        // GET: /Videos/Create
        [AuthContentManagers]
        public ActionResult Create()
        {
            ViewBag.Video_Gallery_ID = new SelectList(db.VideoGalleries, "Video_Gallery_ID", "Gallery_Name");
            return View();
        }

        // POST: /Videos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create([Bind(Include="Video_ID,File_Path,Thumb_Path,Date_Added,Added_By,Date_Archive,Archive_By,Archived,Video_Gallery_ID,Title_Text,Caption")] Video video)
        {
            if (ModelState.IsValid)
            {
                db.Videos.Add(video);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Video_Gallery_ID = new SelectList(db.VideoGalleries, "Video_Gallery_ID", "Gallery_Name", video.Video_Gallery_ID);
            return View(video);
        }

        // GET: /Videos/Edit/5
        [AuthContentManagers]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            ViewBag.Video_Gallery_ID = new SelectList(db.VideoGalleries, "Video_Gallery_ID", "Gallery_Name", video.Video_Gallery_ID);
            return View(video);
        }

        // POST: /Videos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit([Bind(Include="Video_ID,File_Path,Thumb_Path,Date_Added,Added_By,Date_Archive,Archive_By,Archived,Video_Gallery_ID,Title_Text,Caption")] Video video)
        {
            if (ModelState.IsValid)
            {
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Video_Gallery_ID = new SelectList(db.VideoGalleries, "Video_Gallery_ID", "Gallery_Name", video.Video_Gallery_ID);
            return View(video);
        }

        // GET: /Videos/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Video video = db.Videos.Find(id);
        //    if (video == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(video);
        //}

        //// POST: /Videos/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Video video = db.Videos.Find(id);
        //    db.Videos.Remove(video);
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
