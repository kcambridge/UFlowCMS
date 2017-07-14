using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using connex.Models;

namespace connex.Views
{
    public class HomeController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();

        // GET: /Home/
        //public ActionResult Index()
        //{
        //    var pages = db.Pages.Include(p => p.Banner);
        //    return View(pages.ToList());
        //}

        //// GET: /Home/Details/5
        ////public ActionResult Details(int? id)
        ////{
        ////    if (id == null)
        ////    {
        ////        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        ////    }
        ////    Page page = db.Pages.Find(id);
        ////    if (page == null)
        ////    {
        ////        return HttpNotFound();
        ////    }
        ////    return View(page);
        ////}

        //// GET: /Home/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name");
        //    return View();
        //}

        //// POST: /Home/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="Page_ID,Page_Name,URL,Title_Text,Banner_ID,Is_Default")] Page page)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Pages.Add(page);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
        //    return View(page);
        //}

        //// GET: /Home/Edit/5
        //public ActionResult Edit(int? id)
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
        //    ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
        //    return View(page);
        //}

        //// POST: /Home/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="Page_ID,Page_Name,URL,Title_Text,Banner_ID,Is_Default")] Page page)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(page).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Banner_ID = new SelectList(db.Banners, "Banner_ID", "Banner_Name", page.Banner_ID);
        //    return View(page);
        //}

        //// GET: /Home/Delete/5
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

        //// POST: /Home/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Page page = db.Pages.Find(id);
        //    db.Pages.Remove(page);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
