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
using connex.ViewModels;

namespace connex.Controllers
{
    public class CollectionsController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();

        // GET: /Collections/
        public ActionResult Index()
        {
            var collections = db.Collections.Include(c => c.CollectionsDisplayMode).Include(c => c.CollectionsWidthMode).Include(c => c.WidthMode);
            return View(collections.ToList());
        }

        // GET: /Collections/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Collection collection = db.Collections.Find(id);
        //    if (collection == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(collection);
        //}

        // GET: /Collections/Create
        [AuthContentManagers]
        public ActionResult Create(int Page_ID, bool? Return_Home)
        {
            CollectionCreateViewModel collViewMod = new CollectionCreateViewModel(Page_ID);
            ViewBag.Display_Mode_ID = new SelectList(db.CollectionsDisplayModes, "Display_Mode_ID", "Display_Mode");
            ViewBag.Return_Home = Return_Home;
            //ViewBag.Width_Mode_ID = new SelectList(db.CollectionsWidthModes, "Width_Mode_ID", "Width_Mode");
            //ViewBag.Width_Mode_ID = new SelectList(db.WidthModes, "Width_Mode_ID", "Width_Mode");
            return View(collViewMod);
        }

        // POST: /Collections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create([Bind(Include="Collection_Title,Collection_Description,Display_Mode_ID,Page_ID")] CollectionCreateViewModel collectionViewMod, bool? Return_Home)
        {
            Collection collection = new Collection();
            collection.Collection_Description = collectionViewMod.Collection_Description;
            collection.Collection_Title = collectionViewMod.Collection_Title;
            collection.Display_Mode_ID = collectionViewMod.Display_Mode_ID;
            collection.Archived = false;
            db.Collections.Add(collection);
            db.SaveChanges();

            PageCollectionAssign ass = new PageCollectionAssign();
            ass.Collection = collection;
            ass.Page_ID = collectionViewMod.Page_ID;
            ass.Archived = false;
            ass.Date_Added = DateTime.Now;
            ass.Added_By = User.Identity.Name.ToString();
            ass.Sequence_No = 9999;
            db.PageCollectionAssigns.Add(ass);
            db.SaveChanges();
            
            return RedirectToAction("Edit", new { Collection_ID = collection.Collection_ID, Page_ID = collectionViewMod.Page_ID });
        }

        // GET: /Collections/Edit/5
        [AuthContentManagers]
        public ActionResult Edit(int Collection_ID, int Page_ID, bool? Return_Home)
        {
            Collection collection = db.Collections.Find(Collection_ID);
            CollectionEditViewModel collViewMod = new CollectionEditViewModel(collection, Page_ID);
            if (collection == null)
            {
                return HttpNotFound();
            }
            ViewBag.Display_Mode_ID = new SelectList(db.CollectionsDisplayModes, "Display_Mode_ID", "Display_Mode", collection.Display_Mode_ID);
            //ViewBag.Width_Mode_ID = new SelectList(db.CollectionsWidthModes, "Width_Mode_ID", "Width_Mode", collection.Width_Mode_ID);
            //ViewBag.Width_Mode_ID = new SelectList(db.WidthModes, "Width_Mode_ID", "Width_Mode", collection.Width_Mode_ID);
            return View(collViewMod);
        }

        // POST: /Collections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit(CollectionEditViewModel collViewMod, bool? Return_Home)
        {
            Collection collection = db.Collections.Find(collViewMod.Collection_ID);
            collection.Collection_Title = collViewMod.Collection_Title;
            collection.Collection_Description = collViewMod.Collection_Description;
            collection.Archived = collViewMod.Archived;
            collection.Display_Mode_ID = collViewMod.Display_Mode_ID;
            db.SaveChanges();
            collViewMod.posts = collection.Posts.ToList();
            ViewBag.Return_Home = Return_Home;
            ViewBag.Display_Mode_ID = new SelectList(db.CollectionsDisplayModes, "Display_Mode_ID", "Display_Mode", collection.Display_Mode_ID);
            //ViewBag.Width_Mode_ID = new SelectList(db.CollectionsWidthModes, "Width_Mode_ID", "Width_Mode", collection.Width_Mode_ID);
            //ViewBag.Width_Mode_ID = new SelectList(db.WidthModes, "Width_Mode_ID", "Width_Mode", collection.Width_Mode_ID);
            return View(collViewMod);
        }

        // GET: /Collections/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Collection collection = db.Collections.Find(id);
        //    if (collection == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(collection);
        //}

        // POST: /Collections/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Collection collection = db.Collections.Find(id);
        //    db.Collections.Remove(collection);
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
